using Music.ViewModels;
using NoteReader;
using NoteReaderInterface;
using SharedLibraries;
using SharedLibraries.PageViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AppCore
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	//public partial class MainWindow : Window
	//{
	//	public MainWindow()
	//	{
	//		InitializeComponent();
	//	}
	//}
	public partial class MainWindow : Window, INotifyPropertyChanged, IMidiDeviceListener
	{
		private SheetViewModel _sheetVm;

		public SheetViewModel SheetVm
		{
			get { return _sheetVm; }
			set { _sheetVm = value; }
		}

		private NavigationPaneViewModel _navigationVm;
		public NavigationPaneViewModel NavigationViewModel
		{
			get => _navigationVm;
			set
			{
				_navigationVm = value;
				OnPropertyChanged();
			}
		}


		internal IMidiRepository MidiRepo { get; set; }
		internal MidiPublisherChordifyer KeyReader { get; }

		private SettingsViewModel _settingsViewModel;
		public SettingsViewModel SettingsViewModel
		{
			get => _settingsViewModel;
			set
			{
				_settingsViewModel = value;
				OnPropertyChanged();
			}
		}
		private bool _insertDevice;
		private INoteListener _listener;

		public bool HasMidiDevice
		{
			get { return _insertDevice; }
			set { _insertDevice = value; OnPropertyChanged(); }
		}

		public MainWindow()
		{
			InitializeComponent();

			DataContext = this;
			SheetVm = new SheetViewModel();

			MidiRepo = new MidiDeviceRepository();
			SettingsViewModel = new SettingsViewModel(MidiRepo, this);

			NavigationViewModel = new NavigationPaneViewModel();
			NavigationViewModel.NavigationSource.Add(SheetVm);
			NavigationViewModel.SelectedSource = SheetVm;

			HasMidiDevice = true;
			WaitForMidiDevice();

			//Closed += MainWindow_Closed;
		}

		private async void WaitForMidiDevice()
		{
			int counter = 0;
			int max = 10;
			while (!FoundMidiDevice())
			{
				await Task.Delay(200);
				counter++;
				if (counter > max)
					break;
			}

			if (counter >= max)
			{
				//var chordifyer = MidiPublisherChordifyer.CreateChordsFromMidiNotes(this);
				//chordifyer.PublishNotesToListener(SheetVm);
			}
			else
			{
				//Todo set the selected midi device to the stored one if there is a stored device
				HasMidiDevice = FoundMidiDevice();
			}
		}

		public bool FoundMidiDevice()
		{
			return MidiRepo != null && MidiRepo.AvailableDevices.Count > 0;
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}


		public void OnDeviceSelected(INotePublisher selectedPublisher)
		{
			var chordifyer = MidiPublisherChordifyer.CreateChordsFromMidiNotes(selectedPublisher);
			chordifyer.PublishNotesToListener(SheetVm);
			RememberPublisher(selectedPublisher);
		}

		private void RememberPublisher(INotePublisher selectedPublisher)
		{
			File.WriteAllText("./stored", selectedPublisher.UniqueIdentifier);
		}
	}
}

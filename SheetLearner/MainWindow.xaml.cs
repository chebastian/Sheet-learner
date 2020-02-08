using Music.ViewModels;
using NoteReader;
using NoteReaderInterface;
using SharedLibraries;
using SharedLibraries.PageViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XTestMan.Views.Music.NoteReader;

namespace XTestMan
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged, IMidiDeviceListener, INotePublisher
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

		public string UniqueIdentifier => "MainWindow";

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

			Closed += MainWindow_Closed;
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
				var chordifyer = MidiPublisherChordifyer.CreateChordsFromMidiNotes(this);
				chordifyer.PublishNotesToListener(SheetVm);
			}
			else
			{
				HasMidiDevice = FoundMidiDevice();
			}
		}

		public bool FoundMidiDevice()
		{
			return MidiRepo.AvailableDevices.Count > 0;
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

		private void root_KeyDown(object sender, KeyEventArgs e)
		{
			var key = KeyboardNoteReader.MapToKey(e.Key);
			if (key >= 0)
			{
				_listener.OnNotePressed(key);
			}
		}

		private void root_KeyUp(object sender, KeyEventArgs e)
		{
			var key = KeyboardNoteReader.MapToKey(e.Key);
			if (key >= 0)
			{
				_listener.OnNoteReleased(key);
			}
		}

		public void OnDeviceSelected(INotePublisher selectedPublisher)
		{
			var chordifyer = MidiPublisherChordifyer.CreateChordsFromMidiNotes(selectedPublisher);
			chordifyer.PublishNotesToListener(SheetVm);
		}

		public void Register(INoteListener listener)
		{
			_listener = listener;
		}


		public void Unregister(INoteListener listener)
		{
			throw new NotImplementedException();
		}
	}
}

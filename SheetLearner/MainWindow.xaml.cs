using NoteModel;
using NoteReader;
using Prism.Events;
using SharedLibraries;
using SharedLibraries.PageViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XTestMan.Views.Music;
using XTestMan.Views.Music.NoteReader;

namespace XTestMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            SettingsViewModel = new SettingsViewModel(MidiRepo,this);

            NavigationViewModel = new NavigationPaneViewModel();
            NavigationViewModel.NavigationSource.Add(SheetVm);
            NavigationViewModel.SelectedSource = SheetVm;

            HasMidiDevice = true;
            WaitForMidiDevice();

            Closed += MainWindow_Closed;
        }

        private async void WaitForMidiDevice()
        {
            while (!FoundMidiDevice())
            {
                await Task.Delay(200);
            }

            HasMidiDevice = FoundMidiDevice();
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
                KeyReader.OnNotePressed(key);
            }
        }

        private void root_KeyUp(object sender, KeyEventArgs e)
        {
            var key = KeyboardNoteReader.MapToKey(e.Key);
            if (key >= 0)
            {
                KeyReader.OnNoteReleased(key);
            }
        }

        public void OnDeviceSelected(INotePublisher selectedPublisher)
        {
            var chordifyer = MidiPublisherChordifyer.CreateChordsFromMidiNotes(selectedPublisher);
            chordifyer.PublishNotesToListener(SheetVm);
        } 
    }
}

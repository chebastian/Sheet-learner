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
    public partial class MainWindow : Window, INotifyPropertyChanged, IMidiPublisher, IMidiDeviceListener
    {
        //private NoteReader reader; 
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
        internal INoteReader KeyReader { get; }

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

            KeyReader = new MidiKeyReader(this, SheetVm);
            MidiRepo = new NAudioRepo(new NAudioMidiPublisher(this));
            SettingsViewModel = new SettingsViewModel(MidiRepo,this);

            NavigationViewModel = new NavigationPaneViewModel();
            NavigationViewModel.NavigationSource.Add(SheetVm);

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
            MidiRepo.SelectDefaultDevice();
        }

        public bool FoundMidiDevice()
        {
            return MidiRepo.AvailableDevices.Count > 0;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //reader.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler MidiDeviceChanged;
        public event EventHandler<MidiKeyEventArgs> OnKeyPressed;
        public event EventHandler<MidiKeyEventArgs> OnKeyReleased;

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
                OnKeyPressed(this, new MidiKeyEventArgs() { KeyInOctave = key });
            }
        }

        private void root_KeyUp(object sender, KeyEventArgs e)
        {
            var key = KeyboardNoteReader.MapToKey(e.Key);
            if (key >= 0)
            {
                OnKeyReleased(this, new MidiKeyEventArgs() { KeyInOctave = key });
            }
        }

        public void OnDeviceSelected(string name)
        {

        }

        public void OnDeviceSelected(IMidiPublisher name)
        {
            SheetVm.OnKeyPressed
        }
    }
}

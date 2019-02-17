using NoteModel;
using NoteReader;
using Prism.Events;
using SharedLibraries;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
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


        private INoteReader _midi; 
        private bool _insertDevice;

        internal KeyboardNoteReader KeyReader { get; }

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

            NavigationViewModel = new NavigationPaneViewModel();
            NavigationViewModel.NavigationSource.Add(SheetVm);
            _midi = new MidiReader(SheetVm, SheetVm);
            SheetVm.AvailableDevices = new System.Collections.ObjectModel.ObservableCollection<string>( _midi.AvailableDevices );


            KeyReader = new KeyboardNoteReader(SheetVm,SheetVm);

            HasMidiDevice = true;
            WaitForMidiDevice();

            Closed += MainWindow_Closed;
        }

        private async void WaitForMidiDevice()
        {
            while(!FoundMidiDevice())
            {
                await Task.Delay(200); 
            }

            HasMidiDevice = FoundMidiDevice();
            _midi.SelectDefaultDevice();
        }

        public bool FoundMidiDevice()
        {
            return _midi.AvailableDevices.Count > 0;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //reader.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged; 

        protected void OnPropertyChanged( [CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged; 
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void root_KeyDown(object sender, KeyEventArgs e)
        {
            KeyReader.KeyPressedCommand.Execute(e.Key);
        }

        private void root_KeyUp(object sender, KeyEventArgs e)
        {
            KeyReader.KeyUpCommand.Execute(e.Key); 
        }
    }
}

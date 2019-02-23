using MVVMHelpers;
using NoteModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibraries.PageViewModels
{
    public class SettingsViewModel: ViewModelBase
    {
        private String _selectedDevice;

        private IMidiRepository _midi;
        private IMidiDeviceListener _listener;

        public IMidiRepository MidiRepo
        {
            get { return _midi; }
            set { _midi = value; OnPropertyChanged(); }
        }


        public String ActiveDevice
        {
            get { return _selectedDevice; }
            set {
                _selectedDevice = value;
                var pub = _midi.GetPublisherWithName(ActiveDevice);
                _listener.OnDeviceSelected(pub);
                OnPropertyChanged();
            }
        }

        public EventHandler MidiDeviceChanged { get; set; }

        public SettingsViewModel(IMidiRepository midi, IMidiDeviceListener listner)
        {
            _midi = midi;
            _listener = listner;
        } 
    }
}

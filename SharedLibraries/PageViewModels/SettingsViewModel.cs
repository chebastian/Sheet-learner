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
        public ObservableCollection<String> AvailableDevices { get; set; }

        private String _selectedDevice; 
        public String SelectedDevice
        {
            get { return _selectedDevice; }
            set { _selectedDevice = value; MidiDeviceChanged(this,new MidiListenerEventArgs() { SelectedDevice = _selectedDevice }); }
        }

        public EventHandler MidiDeviceChanged { get; set; }
    }
}

using NoteReaderInterface;
using Sefe.Utils.MVVM;
using System;
using System.IO;

namespace SharedLibraries.PageViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		private String _selectedDevice;

		private IMidiRepository _midi;
		private IMidiDeviceListener _listener;

		public IMidiRepository MidiRepo
		{
			get { return _midi; }
			set { _midi = value; OnPropertyChanged(); }
		}

		private string GetLastStoredDevice()
		{
			var res = "";

			if(File.Exists("./stored"))
			{
                res = File.ReadAllText("./stored"); 
			}

			return res;
		}

		public String ActiveDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				var pub = _midi.GetPublisherWithName(ActiveDevice);

				if(pub != null)
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

		public void SetLastSelectedDevice()
		{
			ActiveDevice = GetLastStoredDevice();
		}
	}
}

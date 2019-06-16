using Midi.Devices;
using NoteReaderInterface;
using System.Collections.Generic;
using System.Linq;

namespace NoteReader
{
	public class MidiDeviceRepository : IMidiRepository
	{
		public MidiDeviceRepository()
		{
			AvailableDevices = DeviceManager.InputDevices.Select(x => x.Name).ToList();
		}

		private INotePublisher CurrentPublisher { get; set; }
		public List<string> AvailableDevices { get; set; }

		public INotePublisher GetCurrentPublisher()
		{
			return CurrentPublisher;
		}

		public INotePublisher GetPublisherWithName(string name)
		{
			var theDevice = DeviceManager.InputDevices.Where(x => x.Name == name);
			if (theDevice.Any())
			{
				if (CurrentPublisher != null)
				{
					CurrentPublisher.Unregister(null);
				}

				var publisher = new NAudioMidiPublisher(theDevice.First());
				CurrentPublisher = publisher;
			}

			return CurrentPublisher;
		}
	}
}

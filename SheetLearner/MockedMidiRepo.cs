using System.Collections.Generic;
using NoteModel;

namespace XTestMan
{
    internal class MockedMidiRepo : IMidiRepository
    {
        public List<string> AvailableDevices { get; set; }

        public void SelectDefaultDevice()
        {
        }

        public void SelectDeviceWithName(string name)
        {
            throw new System.NotImplementedException();
        }

        public IMidiPublisher GetCurrentPublisher()
        {
            return null;
        }

        public IMidiPublisher GetPublisherWithName(string name)
        {
            return null;
        }

        public MockedMidiRepo()
        {
            AvailableDevices = new List<string> { "Keyboard" };
        }
    }
}
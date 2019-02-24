using Midi;
using NoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteReader
{
    public class NAudioRepo : IMidiRepository
    {
        private string _name;

        public NAudioRepo(IMidiPublisher publisher)
        {
            Publisher = publisher;
            AvailableDevices = InputDevice.InstalledDevices.Select(x => x.Name).ToList(); 
        }

        public IMidiPublisher Publisher { get; }
        public List<string> AvailableDevices { get; set; }

        public IMidiPublisher GetCurrentPublisher()
        {
            return Publisher;
        }

        public IMidiPublisher GetPublisherWithName(string name)
        {
            (Publisher as NAudioMidiPublisher).SelectDeviceWithName(name);
            return Publisher;
        }

        public void SelectDefaultDevice()
        {
        }

        public void SelectDeviceWithName(string name) 
        {

        }
    }

    public class NAudioMidiPublisher : IMidiPublisher

    {
        private List<INoteListener> _listeners;

        public List<int> NotesPressed { get; }


        public InputDevice SelectedDevice { get; private set; }

        public NAudioMidiPublisher()
        {
            NotesPressed = new List<int>();
        }

        public bool SelectDeviceWithName(String deviceName)
        {
            var devices = InputDevice.InstalledDevices.Any(X => X.Name == deviceName);
            if(!devices) 
                return false;

            var device = InputDevice.InstalledDevices.First(x => deviceName.Equals(x.Name));
            if (SelectedDevice != null)
            {
                SelectedDevice.NoteOn -= _callback;
            }

            SelectedDevice = device;
            if(!device.IsOpen)
                device.Open();
            device.NoteOn += _callback;
            device.NoteOff += OnNoteOffCallback;
            if(!device.IsReceiving)
                device.StartReceiving(null);

            return true; 
        }

        private void OnNoteOffCallback(NoteOffMessage msg)
        {
            foreach(var listener in _listeners)
            {
                listener.OnNoteReleased(msg.Pitch.PositionInOctave());
            }
        }

        private void _callback(NoteOnMessage msg)
        {
            foreach(var listener in _listeners)
            {
                listener.OnNotePressed(msg.Pitch.PositionInOctave());
            }
        }

        public void Register(INoteListener listener)
        {
            _listeners = _listeners ?? new List<INoteListener>();
            _listeners.Add(listener);
        }
    }
}

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
        public NAudioRepo()
        {
            AvailableDevices = InputDevice.InstalledDevices.Select(x => x.Name).ToList(); 
        }

        private IMidiPublisher CurrentPublisher { get; set; }
        public List<string> AvailableDevices { get; set; }

        public IMidiPublisher GetCurrentPublisher()
        {
            return CurrentPublisher;
        }

        public IMidiPublisher GetPublisherWithName(string name)
        {
            var theDevice = InputDevice.InstalledDevices.Where(x => x.Name == name);
            if(theDevice.Any())
            {
                if(CurrentPublisher != null)
                {
                    CurrentPublisher.Unregister(null);
                }

                var publisher = new NAudioMidiPublisher(theDevice.First());
                CurrentPublisher = publisher;
            }

            return CurrentPublisher;
        } 
    }

    public class NAudioMidiPublisher : IMidiPublisher
    {
        public NAudioMidiPublisher(InputDevice device)
        {
            _device = device;
        }

        private List<INoteListener> _listeners;
        private InputDevice _device;

        public List<int> NotesPressed { get; } 

        public NAudioMidiPublisher()
        {
            NotesPressed = new List<int>();
        }

        public bool RegisterMidiListeners()
        { 
            if(!_device.IsOpen)
                _device.Open();
            _device.NoteOn += _callback;
            _device.NoteOff += OnNoteOffCallback;
            if(!_device.IsReceiving)
                _device.StartReceiving(null);

            return true; 
        }

        public void Unregister(INoteListener listener)
        {
            _device.NoteOn -= _callback;
            _device.NoteOff -= OnNoteOffCallback; 
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
            RegisterMidiListeners();
            _listeners = _listeners ?? new List<INoteListener>();
            _listeners.Add(listener);
        } 
    }
}

using Midi;
using NoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteReader
{
    public class NAudioMidiPublisher : INotePublisher
    {
        public NAudioMidiPublisher(InputDevice device)
        {
            _device = device;
        }

        private List<INoteListener> _listeners;
        private InputDevice _device;

        public NAudioMidiPublisher()
        {
        }

        public bool RegisterMidiListeners()
        { 
            if(!_device.IsOpen && !_device.IsReceiving)
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

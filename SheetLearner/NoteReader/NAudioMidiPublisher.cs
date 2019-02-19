using Midi;
using NoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteReader
{
    public class NAudioMidiPublisher : IMidiPublisher
    {
        public List<int> NotesPressed { get; }


        public InputDevice SelectedDevice { get; private set; }

        public event EventHandler MidiDeviceChanged;
        public event EventHandler<MidiKeyEventArgs> OnKeyPressed;
        public event EventHandler<MidiKeyEventArgs> OnKeyReleased;

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
            OnKeyReleased(this, new MidiKeyEventArgs() { KeyInOctave = msg.Pitch.PositionInOctave() }); 
        }

        private void _callback(NoteOnMessage msg)
        {
            OnKeyPressed(this, new MidiKeyEventArgs() { KeyInOctave = msg.Pitch.PositionInOctave() }); 
        } 
    }
}

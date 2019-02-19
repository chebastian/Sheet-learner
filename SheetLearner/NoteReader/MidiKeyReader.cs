using NoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteReader
{
    public class MidiKeyReader : INoteReader
    {
        private IMidiPublisher _midi;
        private INoteListener _listener;

        public MidiKeyReader(IMidiPublisher midi,INoteListener listener)
        {
            _midi = midi;
            _listener = listener;
            _midi.OnKeyReleased += _midi_OnKeyReleased;
            _midi.OnKeyPressed += _midi_OnKeyPressed;
            NotesPressed = new List<int>();
            AvailableDevices = new List<string> { "Midi simulating Keyboard" };
        }

        public List<int> NotesPressed { get; set; }
        public List<string> AvailableDevices { get; set; }

        public void SelectDefaultDevice()
        {
        }

        private void _midi_OnKeyPressed(object sender, MidiKeyEventArgs e)
        {
            var idx = e.KeyInOctave;
            NotesPressed.Add(idx);

            if (NotesPressed.Count > 1) 
                _listener.OnNotesPressed(NotesPressed);
            else
                _listener.OnNotePressed(idx);
        }

        private void _midi_OnKeyReleased(object sender, MidiKeyEventArgs e)
        {
            if (NotesPressed.Contains(e.KeyInOctave))
                NotesPressed.Remove(e.KeyInOctave);
        }
    }
}

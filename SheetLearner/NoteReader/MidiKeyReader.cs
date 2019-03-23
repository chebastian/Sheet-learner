using NoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteReader
{
    public class NoteListenerChordifyer :  IMidiPublisher, INoteListener
    {
        private IMidiPublisher _midi;
        private INoteListener _listener;
        private List<INoteListener> _listeners;

        public NoteListenerChordifyer(IMidiPublisher midi)
        {
            _midi = midi;
            _midi.Register(this);
            NotesPressed = new List<int>();
            AvailableDevices = new List<string> { "Midi simulating Keyboard" };
        }

        public List<int> NotesPressed { get; set; }
        public List<string> AvailableDevices { get; set; }
 
        public void Register(INoteListener listener)
        {
            _listener = listener;
        }
 
        private void _midi_OnKeyReleased(object sender, MidiKeyEventArgs e)
        {
            if (NotesPressed.Contains(e.KeyInOctave))
                NotesPressed.Remove(e.KeyInOctave);
        }

        public void OnNotePressed(int note)
        {
            NotesPressed.Add(note);

            if (NotesPressed.Count > 1)
                _listener.OnNotesPressed(NotesPressed);
            else
                _listener.OnNotePressed(note); 
        }

        public void OnNotesPressed(List<int> notes)
        {
        }

        public void OnNoteReleased(int note)
        {
            if (NotesPressed.Contains(note))
                NotesPressed.Remove(note);
        }

        public void Unregister(INoteListener listener)
        {
        }
    }
}

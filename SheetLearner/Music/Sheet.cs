using SheetLearner.Music;
using System.Collections.Generic;
using System.Linq;

namespace SheetLearner.Music
{
    public enum Clef
    {
        Bass,
        Treble
    }


    public class Sheet
    {
        public static Clef Bass = Clef.Bass;
        public static Clef Treble = Clef.Treble;
        public Clef ActiveClef { get; set; }

        public Dictionary<Note, int> map;

        public static List<Note> GetNonLedgerNotesInClef(Clef clef)
        {
			var notes = Notes.NotesInClef(clef);
            return notes.Skip(2).Take(notes.Count - 4).ToList();
        } 

        public void SwitchClef(Clef clef)
        {
            ActiveClef = clef;
            //RemapClef();
        }

        public Sheet(Clef clef)
        {
            ActiveClef = clef;
            SwitchClef(ActiveClef);
        }

        public static Dictionary<Note, int> NoteMapForClef(Clef clef)
        {
            var map = new Dictionary<Note, int>();

            var notes = GetNonLedgerNotesInClef(clef);
            var count = 0;
            foreach (var note in notes)
            {
                map.Add(note, count);
                count++;
            }
            return map;
        }

        public int GetNoteValueInClef(Clef myClef, Note note)
        {
            var comparer = new Note(note);
            if (note.IsSharp)
            {
                comparer = new Note(note.Root);
            }

            var map = NoteMapForClef(myClef);
            map.TryGetValue(comparer, out int value);
            return value;
        }
    }
}
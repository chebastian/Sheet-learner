using System;
using System.Collections.Generic;
using System.Linq;
using XTestMan.Views.Music.NoteReader;

namespace XTestMan.Views.Music
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
        public List<List<Note>> Notes { get; set; }
        public Clef ActiveClef { get; set; }

        public Dictionary<Note, int> map;

        public static List<Note> GetNonLedgerNotesInClef(Clef clef)
        {
            var notes = GetNotesInActiveClef(clef);
            return notes.Skip(2).Take(notes.Count - 4).ToList();
        }

        public static List<Note> GetNotesInActiveClef(Clef clef)
        {
            if (clef == Clef.Bass)
                return NotesFactory.BassNote;

            return NotesFactory.TrebleNote;
        }

        public void SwitchClef(Clef clef)
        {
            ActiveClef = clef;
            //RemapClef();
        }
 
        public Sheet(Clef clef)
        {
            ActiveClef = clef;
            Notes = new List<List<Note>>();
            SwitchClef(ActiveClef);
        }

        public static Dictionary<Note,int> NoteMapForClef(Clef clef)
        { 
            var map = new Dictionary<Note, int>();

            var notes = GetNonLedgerNotesInClef(clef);
            var count = 0;
            foreach(var note in notes)
            {
                map.Add(note,count);
                count++;
            }
            return map;
        }
 
        public int GetNoteValueInClef(Clef myClef, Note note)
        {
            var comparer = new Note(note);
            if(note.IsSharp)
            {
                comparer = new Note(note.Root); 
            }

            var map = NoteMapForClef(myClef);
            map.TryGetValue(comparer,out int value);
            return value;
        } 
    }
}
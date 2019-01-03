using System;
using System.Collections.Generic;
using System.Linq;

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
            return GetNotesInClef(clef);
        }

        public void SwitchClef(Clef clef)
        {
            ActiveClef = clef;
            RemapClef();
        }

        public static List<Note> GetNotesInClef(Clef c)
        {
            var str = String.Empty;
            if (c == Clef.Bass)
                str= "efgabcdEFGABC";
            else
                str= "cdefgabCDEFGA";

            str =new string(str.Reverse().ToArray());
            return Note.NotesFromString(str);
        } 

        //public bool IsTopLedger(Note n, Clef clef)
        //{
        //    var topLedgerNotes = String.Empty;

        //    if (clef.Equals(Clef.Treble))
        //        topLedgerNotes = "GA";
        //    else
        //        topLedgerNotes = "BC";

        //    return topLedgerNotes.Contains(n.Id);
        //}

        //public bool IsBottomLedger(Note n, Clef clef)
        //{
        //    var bottomLedgerNotes = String.Empty;

        //    if (clef.Equals(Clef.Bass))
        //        bottomLedgerNotes = "ef";
        //    else
        //        bottomLedgerNotes = "cd";

        //    return bottomLedgerNotes.Contains(n.Id);
        //}

        public Sheet(Clef clef)
        {
            ActiveClef = clef;
            Notes = new List<List<Note>>();
            //map = new Dictionary<string, int>();

            ////var notes = GetNotesInClef(clef);
            //var notes = GetNonLedgerNotesInClef();
            //var count = 0;
            //foreach(var note in notes)
            //{
            //    map.Add(note.ToString(), count);
            //    count++;
            //}

            //count = 0;
            //if(ActiveClef.Equals(Clef.Bass))
            //{
            //    map.Add("e", 1);
            //    map.Add("f", 0);
            //    map.Add("B", 1);
            //    map.Add("C", 0);
            //}

            //if(ActiveClef.Equals(Clef.Treble))
            //{
            //    map.Add("c", 1);
            //    map.Add("d", 0);
            //    map.Add("G", 1);
            //    map.Add("A", 0);
            //}
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

            count = 0;
            if(clef.Equals(Clef.Bass))
            {
                map.Add(NotesFactory.E1, 1);
                map.Add(NotesFactory.F1, 0);
                map.Add(NotesFactory.B2, 1);
                map.Add(NotesFactory.C2, 0);
            }

            if(clef.Equals(Clef.Treble))
            {
                map.Add(NotesFactory.C1, 1);
                map.Add(NotesFactory.D1, 0);
                map.Add(NotesFactory.G2, 1);
                map.Add(NotesFactory.A2, 0); 
            }
            return map;
        }

        private void RemapClef() 
        {
            map = new Dictionary<Note, int>();

            //var notes = GetNotesInClef(clef);
            var notes = GetNonLedgerNotesInClef(ActiveClef);
            var count = 0;
            foreach(var note in notes)
            {
                map.Add(note,count);
                count++;
            }

            count = 0;
            if(ActiveClef.Equals(Clef.Bass))
            {
                map.Add(NotesFactory.E1, 1);
                map.Add(NotesFactory.F1, 0);
                map.Add(NotesFactory.B2, 1);
                map.Add(NotesFactory.C2, 0);
            }

            if(ActiveClef.Equals(Clef.Treble))
            {
                map.Add(NotesFactory.C1, 1);
                map.Add(NotesFactory.D1, 0);
                map.Add(NotesFactory.G2, 1);
                map.Add(NotesFactory.A2, 0);
            } 
        }

        internal void AddNote(Note note)
        {
            var notes = new List<Note>();
            int c = GetNoteValueInClef(ActiveClef, note);
            note.Value = c;

            var len = GetNotesInClef(ActiveClef).Count;
            notes.Add(note);
            Notes.Add(notes);
        }

        public int GetNoteValueInClef(Clef myClef, Note note)
        {
            var map = NoteMapForClef(myClef);
            map.TryGetValue(note,out int value);
            return value;
        }

        internal void AddChord(List<Note> list)
        {
            var max = list.Max<Note>(x => GetNoteValueInClef(ActiveClef, x));
            var notes = new List<Note>();
            var newnotes = Enumerable.Repeat<Note>(new Note(),max+1).ToList();
            newnotes.AddRange(Enumerable.Repeat<Note>(new Note(), GetNotesInClef(ActiveClef).Count - max));

            foreach(var note in list)
            {
                note.Value = GetNoteValueInClef(ActiveClef, note);
                newnotes[note.Value] = note;
            }

            Notes.Add(newnotes); 
        }
    }
}
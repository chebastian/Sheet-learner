using MVVMHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace XTestMan.Views.Music
{
    public static class NotesFactory
    {
        public static List<Note> C_Octave => Note.NotesFromString("cdefgAB");

        public static Note C1 = new Note("c");
        public static Note D1 = new Note("d");
        public static Note E1 = new Note("e");
        public static Note F1 = new Note("f");
        public static Note G1 = new Note("g");
        public static Note A1 = new Note("a");
        public static Note B1 = new Note("b");
        public static Note C2 = new Note("C");
        public static Note D2 = new Note("D");
        public static Note E2 = new Note("E");
        public static Note F2 = new Note("F");
        public static Note G2 = new Note("G");
        public static Note A2 = new Note("A");
        public static Note B2 = new Note("B");
        public static Note C3 = new Note("C");
    }

    [DebuggerDisplay("{_note} : {Value}")]
    public class Note : ViewModelBase
    { 
        public Note()
        {
            Value = 0;
            _note = String.Empty;
        }

        public Note(String note)
        {
            _note = note;
        }

        public Note(Note note)
        {
            _note = note._note;
            Value = note.Value;
        }

        public static List<Note> Triad(String a, string b, string c)
        {
            return new List<Note>() { new Note(a), new Note(b), new Note(c) }; 
        }

        public bool IsEmpty()
        {
            return String.IsNullOrWhiteSpace(_note); 
        }


        protected int _val;
        protected String _note;

        public bool Show { get => _note.Length > 0; }
        public String Id { get => _note; }

        public int Value
        {
            get { return _val; }
            set { _val = value; OnPropertyChanged(); }
        } 

        public static List<Note> NotesFromString(String notes)
        {
            return notes.ToCharArray().Select(x => new Note(x.ToString())).ToList();
        }

        public override bool Equals(object obj)
        {
            var note = obj as Note;
            return note != null && _note == note._note;
        }

        public override int GetHashCode()
        {
            var hashCode = 1925529221;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_note);
            return hashCode;
        }
    }
}
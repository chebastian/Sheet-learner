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

        public static string AllIdentifiers => "c,c#,d,d#,e,f,f#,g,g#,a,a#,b";
        public static List<Note> AllNotes => AllIdentifiers.Split(',').Select(x => new Note(x)).ToList();


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

        public static List<Note> BassLowerLedger => new List<Note>()
        {
            E1,
            F1
        };

        public static List<Note> TrebbleLowerLedger => new List<Note>()
        {
            C1,D1
        };

        public static List<Note> TrebbleUpperLedger => new List<Note>()
        {
            G2,A2
        };

        public static List<Note> BassUpperLedger => new List<Note>()
        {
            B2,C2
        };



}

[DebuggerDisplay("{_note} : {Value}")]
    public class Note : ViewModelBase
    { 
        public Note()
        {
            //Value = 0;
            _note = String.Empty;
        }

        public Note(String note)
        {
            _note = note;
            IsSharp = note.Contains("#");
        }

        public Note(Note note)
        {
            _note = note._note;
            _isSharp = note.IsSharp;
            //Value = note.Value;
        }

        public static List<Note> Triad(String a, string b, string c)
        {
            return new List<Note>() { new Note(a), new Note(b), new Note(c) }; 
        }

        public Note Sharped()
        {
            //var index = NotesFactory.AllNotes.IndexOf(this);

            var index = NotesFactory.AllIdentifiers.ToUpper().Split(',').ToList().IndexOf(this.Id.ToUpper());
            index = (index+1) % NotesFactory.AllNotes.Count;
            return new Note(NotesFactory.AllNotes[index]);
        }

        public Note Flattened()
        {
            var index = NotesFactory.AllNotes.IndexOf(this);
            index = (index-1) % NotesFactory.AllNotes.Count;
            return new Note(NotesFactory.AllNotes[index]);
        }

        public bool IsEmpty()
        {
            return String.IsNullOrWhiteSpace(_note); 
        }

        public Note Root
        {
            get
            {
                return IsSharp ? Flattened() : this;
            }
        }


        protected int _val;
        protected String _note;
        private bool _isSharp;
        private bool _isFlat;

        public bool Show { get => _note.Length > 0; }
        public String Id { get => _note; }

        //public int Value
        //{
        //    get { return _val; }
        //    set { _val = value; OnPropertyChanged(); }
        //}

        public bool IsSharp
        {
            get { return _isSharp; }
            set { _isSharp = value; OnPropertyChanged(); }
        }

        public bool IsFlat
        {
            get { return _isFlat; }
            set { _isFlat = value; OnPropertyChanged(); }
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
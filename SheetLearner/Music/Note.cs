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
        public static Note C3 = new Note("C1");
        public static Note D3 = new Note("D1");
        public static Note E3 = new Note("E1");
        public static Note F3 = new Note("F1");
        public static Note G3 = new Note("G1");
        public static Note A3 = new Note("A1");
        public static Note B3 = new Note("B1");
        public static List<Note> Notes => new List<Note>
        {
            A1,B1,C1,D1,E1,F1,G1,
            A2,B2,C2,D2,E2,F2,G2,
            A3,B3,C3,D3,E3,F3,G3,
        };

        public static List<Note> BassNotes => new List<Note>
        {
            A1,B1,C1,D1,E1,F1,G1,
            A2,B2,C2,D2,E2,F2,G2,
            A3,B3,C3,D3,E3,F3,G3,
        };

        public static List<Note> TrebleNotes => new List<Note>
        {
            F1, G1, A1, B1, C1, D1, E1,
            F2, G2, A2, B2, C2, D2, E2,
            F3, G3, A3, B3, C3, D3, E3,
        };

        public static Note GetInterval(Note n, int interval, Clef clef)
        {
            var source = clef == Clef.Bass ? BassNotes : TrebleNotes;
            source = source.Select(x => x).ToList(); // TODO make a deep copy

            var idx = source.IndexOf(n);
            var maxIdx = Math.Min(idx + interval, source.Count - 1);
            return source.ElementAt(maxIdx);
        }




        private static List<Note> NotesInRange(List<Note> range, Note min, Note max)
        {
            var indexMin = range.IndexOf(min);
            var indexMax = range.IndexOf(max);

            if (indexMax < 0 || indexMin < 0)
                return new List<Note>();

            var noteMin = Math.Min(indexMin, indexMax);
            var noteMax = Math.Max(indexMin, indexMax);
            return range.GetRange(noteMin, (noteMax + 1) - noteMin).ToList();
        }

        private static List<Note> WhereMatch(List<Note> notes, List<Note> match)
        {
            return notes.Where(x => match.Contains(x)).ToList();
        }

        public static List<Note> GetNotesInLedger(Note note, Clef clef)
        {
            var result = new List<Note>();

            if (clef == Clef.Bass)
            {
                var bottom = new List<Note>() { A1, C1, E1 };
                var top = new List<Note>() { C3, E3, G3 };

                var topNotes = NotesInRange(BassNotes, G3, C3);
                var topLedger = NotesInRange(topNotes, note, C3);

                var bottomNotes = NotesInRange(BassNotes, A1, E1);
                var bottomLedger = NotesInRange(bottomNotes, note, E1);

                result = bottomLedger.Any() ? WhereMatch(bottomLedger,bottom) : WhereMatch(topLedger,top);
            }
            else if (clef == Clef.Treble)
            {
                var bottom = new List<Note>() { F1, A1, C1 };
                var top = new List<Note>() { A3, C3, E3 };

                var topNotes = NotesInRange(TrebleNotes, A3, E3);
                var topLedger = NotesInRange(topNotes, note, A3);

                var bottomNotes = NotesInRange(TrebleNotes, F1, C1);
                var bottomLedger = NotesInRange(bottomNotes, note, C1);

                result = bottomLedger.Any() ? WhereMatch(bottomLedger,bottom) : WhereMatch(topLedger,top);
            } 

            return result;
        }

        public static int NumberOfLedgerLines(Note note, Clef clef)
        {
            var count = 0;
            if (clef == Clef.Bass)
            {
                var ledgerDict = new Dictionary<Note, int>()
                {
                    //bottom
                    {A1,3},
                    {B1,2},
                    {C1,2},
                    {D1,1},
                    {E1,1},
                    {F1,0},

                    //Top
                    {B3,0},
                    {C3,1},
                    {D3,1},
                    {E3,2},
                    {F3,2},
                    {G3,3},
                };

                if (ledgerDict.TryGetValue(note, out var bassCount))
                {
                    return bassCount;
                }
            }

            if (clef == Clef.Treble)
            {
                var ledgerDict = new Dictionary<Note, int>()
                {
                    //F1, G1, A1, B1, C1, D1, E1,
                    //bottom
                    {F1,3},
                    {G1,2},
                    {A1,2},
                    {B1,1},
                    {C1,1},
                    {D1,0},

                    //Top F3, G3, A3, B3, C3, D3, E3,
                    {F3,0},
                    {G3,0},
                    {A3,1},
                    {B3,1},
                    {C3,2},
                    {D3,2},
                    {E3,3},
                };

                if (ledgerDict.TryGetValue(note, out var bassCount))
                {
                    return bassCount;
                }
            }

            return 0;
        }

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
            //var index = NotesFactory.AllIdentifiers.ToUpper().Split(',').ToList().IndexOf(this.Id.ToUpper());
            //index = (index + 1) % NotesFactory.AllNotes.Count;
            //return new Note(NotesFactory.AllNotes[index]);
            return new Note(_note) { IsSharp = true };
        }

        public Note Flattened()
        {
            var index = NotesFactory.AllNotes.IndexOf(this);
            index = (index - 1) % NotesFactory.AllNotes.Count;
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

        public bool Equals(Note note)
        {
            var idx = NotesFactory.TrebleNotes.IndexOf(note) % 8;
            var myidx = NotesFactory.TrebleNotes.IndexOf(this) % 8;
            return idx == myidx;
        }

        public override int GetHashCode()
        {
            var hashCode = 1925529221;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_note);
            return hashCode;
        }
    }
}
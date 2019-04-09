using MVVMHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTestMan.Views.Music;

namespace SheetLearner.Music.ViewModels
{
    public class NotesLedgerViewModel : ViewModelBase
    {

        private int _x;
        public int X
        {
            get => _x;
            set
            {
                _x = value;
                OnPropertyChanged();
            }
        }


        private int _y;
        public int Y
        {
            get => _y;
            set
            {
                _y = value;
                OnPropertyChanged();
            }
        }


        private int _topLedgerCount;
        public int TopLedgerCount
        {
            get => _topLedgerCount;
            set
            {
                _topLedgerCount = value;
                OnPropertyChanged();
            }
        }


        private int _bottomLedgerCount;
        public int BottomLedgercount
        {
            get => _bottomLedgerCount;
            set
            {
                _bottomLedgerCount = value;
                OnPropertyChanged();
            }
        } 
    }

    public class NoteViewModel : ViewModelBase
    {
 
        public NoteViewModel(Note note)
        {
            Note = note;
            Played = false;
        }


        private bool _isLedger;
        public bool IsLedger
        {
            get => _isLedger;
            set
            {
                _isLedger = value;
                OnPropertyChanged();
            }
        } 

        private bool _played;

        public Note Note { get; }

        public bool Played
        {
            get => _played;
            set
            { 
                _played = value;
                OnPropertyChanged();
            }
        }

        private int _x;
        public int X
        {
            get => _x;
            set
            {
                _x = value;
                OnPropertyChanged();
            }
        }


        private int _y;
        public int Y
        {
            get => _y;
            set
            {
                _y = value;
                OnPropertyChanged();
            }
        } 
    }

    public class ClefViewModel : ViewModelBase
    {
        public List<NoteSection> Groups;
        public List<NoteSection> Ledger;

        public ObservableCollection<NoteViewModel> Notes
        {
            get;
            set;
        }
        public int NoteWidth { get; set; } = 16;
        public Clef ActiveClef { get; private set; }
        public ObservableCollection<NotesLedgerViewModel> Lines { get;  set; }
        public ObservableCollection<NoteViewModel> NotesInLedger { get; set; }

        public ClefViewModel(Clef clef)
        {
            Groups = new List<NoteSection>();
            Notes = new ObservableCollection<NoteViewModel>();
            NotesInLedger = new ObservableCollection<NoteViewModel>();

            ActiveClef = clef;

            var notes = new List<Note>
            {
                NotesFactory.C1,
                NotesFactory.D1,
                NotesFactory.E1,
                NotesFactory.E2,
                NotesFactory.C2,
                NotesFactory.C3,
            };

            var two = new List<Note>
            {
                NotesFactory.F1,
                NotesFactory.E2
            };

            //AddNoteGroup(notes);
            //AddNoteGroup(two);
        }

        public void AddNoteGroup(List<Note> notes)
        {
            var notesInSection = new List<NoteViewModel>();

            var xoffset = 1 + Groups.Count * NoteWidth;
            var left = false;
            var last = -1;
            foreach(var note in notes.OrderBy(x => x.Id))
            {
                var ypos = NoteToPisitionInClef(note, ActiveClef); 
                var newNote = new NoteViewModel(note) { X = xoffset, Y = 6 * ypos };

                newNote.IsLedger = ypos % 2 == 1;

                var is2nd = last - ypos == 1;
                if(is2nd)
                {
                    left = !left;
                    newNote.X += left ? 4 : 0;
                }
                last = ypos;

                Notes.Add(newNote);
                notesInSection.Add(newNote);
            } 
            Groups.Add(new NoteSection(notesInSection));

            NotesInLedger = NotesInLedger ?? new ObservableCollection<NoteViewModel>();

            var ledger = CreateTrailingLinesForSection(new NoteSection(notesInSection));
            foreach(var note in ledger)
            {
                var ypos = NoteToPisitionInClef(note.Note, ActiveClef); 
                NotesInLedger.Add(new NoteViewModel(note.Note) { X = xoffset, Y = 6 * ypos });
            }

            Ledger = Ledger ?? new List<NoteSection>();
            Ledger.Add(new NoteSection(ledger)); 
        }

        public int GetNumberOfLedgerLinesInNote(Note note)
        {
            return NotesFactory.NumberOfLedgerLines(note, ActiveClef);
        }

        private List<NoteViewModel> CreateTrailingLinesForSection(NoteSection noteSection)
        {
            if (noteSection.Notes.Count <= 0)
                return new List<NoteViewModel>();
 
            Lines = Lines ?? new ObservableCollection<NotesLedgerViewModel>();

            var order = noteSection.Notes.Select(x => new { idx = NoteToPisitionInClef(x.Note,ActiveClef), val = x }).OrderBy(x => x.idx).ToList();
            var minNote = order.First().val.Note;
            var maxNote = order.Last().val.Note;

            var linesToFill = new List<NoteViewModel>();

            linesToFill.AddRange(NotesFactory.GetLineNotesInLedger(minNote, ActiveClef).Select(x => new NoteViewModel(x)));
            linesToFill.AddRange(NotesFactory.GetLineNotesInLedger(maxNote, ActiveClef).Select(x => new NoteViewModel(x)));

            return linesToFill;
        }

        private int NoteToPisitionInClef(Note note, Clef clef)
        {
            var idx = clef == Clef.Bass ? NotesFactory.BassNote.IndexOf(note) : NotesFactory.TrebleNote.IndexOf(note);
            return NotesFactory.BassNote.Count - idx;
        }

        private List<int> NotesToPositionsInClef(List<Note> note, Clef clef)
        {
            return note.Select(x => NoteToPisitionInClef(x,clef)).ToList();
        }
    }
}

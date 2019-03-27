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

        public ObservableCollection<NoteViewModel> Notes
        {
            get;
            set;
        }
        public int NoteWidth { get; set; } = 16;
        public Clef ActiveClef { get; private set; }
        public ObservableCollection<NotesLedgerViewModel> Lines { get; private set; }

        public ClefViewModel(Clef clef)
        {
            Groups = new List<NoteSection>();
            Notes = new ObservableCollection<NoteViewModel>();
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
            var vms = new List<NoteViewModel>();

            var xoffset = 1 + Groups.Count * NoteWidth;
            var index = 0;
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
                index++;
                last = ypos;

                Notes.Add(newNote);
                vms.Add(newNote);
            } 

            Groups.Add(new NoteSection(vms));

            AddLedgerLines(new NoteSection(vms));

        }

        public int GetNumberOfLedgerLinesInNote(Note note)
        {
            return NotesFactory.NumberOfLedgerLines(note, ActiveClef);
        }

        private void AddLedgerLines(NoteSection noteSection)
        {
            if (noteSection.Notes.Count <= 0)
                return;

            var notes = ActiveClef == Clef.Bass ? NotesFactory.BassNote : NotesFactory.TrebleNote;
            var order = noteSection.Notes.Select(x => new { idx = NoteToPisitionInClef(x.Note,ActiveClef), val = x }).OrderBy(x => x.idx);
            var minNote = order.First();
            var maxNote = order.Last();

            Lines = Lines ?? new ObservableCollection<NotesLedgerViewModel>();
            Lines.Add(new NotesLedgerViewModel()
            {
                X = Lines.Count * NoteWidth,
                TopLedgerCount = NotesFactory.TopLedgerLines(minNote.val.Note, ActiveClef),
                BottomLedgercount = NotesFactory.BottomLedgerLines(maxNote.val.Note, ActiveClef)
            });
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

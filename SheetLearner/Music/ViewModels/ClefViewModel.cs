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
    public class NoteViewModel : ViewModelBase
    {
 
        public NoteViewModel(Note note)
        {
            Note = note;
            Played = false;
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
        }

        public void AddNoteGroup(List<Note> notes)
        {
            var notesInSection = new List<NoteViewModel>();

            var xoffset = 1 + Groups.Count * NoteWidth;
            var left = false;
            var lastNotePosition = -1;
            foreach(var note in notes.OrderBy(x => x.Id))
            {
                var currentNoteY = NoteToPisitionInClef(note, ActiveClef); 
                var newNote = new NoteViewModel(note)
                {
                    X = xoffset,
                    Y = 6 * currentNoteY,
                }; 

                var isDirectlyAboveLastNote = lastNotePosition - currentNoteY == 1;
                if(isDirectlyAboveLastNote)
                {
                    left = !left;  // If more than one note is in succesion it should go left, right left
                    newNote.X += left ? 4 : 0;
                }
                else
                    left = false;

                lastNotePosition = currentNoteY; 
                Notes.Add(newNote);
                notesInSection.Add(newNote);
            } 
            Groups.Add(new NoteSection(notesInSection));

            NotesInLedger = NotesInLedger ?? new ObservableCollection<NoteViewModel>();

            var ledger = CreateTrailingLinesForSection(new NoteSection(notesInSection));

            ledger.ForEach( note => NotesInLedger.Add(
                    new NoteViewModel(note.Note)
                    {
                        X = xoffset,
                        Y = 6 * NoteToPisitionInClef(note.Note,ActiveClef)
                    })
                );
        }

        public int GetNumberOfLedgerLinesInNote(Note note)
        {
            return NotesFactory.NumberOfLedgerLines(note, ActiveClef);
        }

        private List<NoteViewModel> CreateTrailingLinesForSection(NoteSection noteSection)
        {
            if (noteSection.Notes.Count <= 0)
                return new List<NoteViewModel>();

            var linesToFill = new List<NoteViewModel>();

            linesToFill.AddRange(NotesFactory.GetNotesInLedger(noteSection.HighestNote, ActiveClef).Select(x => new NoteViewModel(x)));
            linesToFill.AddRange(NotesFactory.GetNotesInLedger(noteSection.LowestNote, ActiveClef).Select(x => new NoteViewModel(x)));

            return linesToFill;
        }

        private int NoteToPisitionInClef(Note note, Clef clef)
        {
            var idx = clef == Clef.Bass ? NotesFactory.BassNote.IndexOf(note) : NotesFactory.TrebleNote.IndexOf(note);
            return NotesFactory.BassNote.Count - idx;
        } 
    }
}

using MVVMHelpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SheetLearner.Music;

namespace SheetLearner.Music.ViewModels
{
    public class NoteViewModel : ViewModelBase
    {

        public NoteViewModel() { }

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

    public class SharpNote : NoteViewModel
    {
        public SharpNote(Note note) : base(note)
        {
        }
    }

    public class ClefViewModel : ViewModelBase
    {
        public List<NoteSection> Sections;

        public ObservableCollection<NoteViewModel> NotesInClef
        {
            get;
            set;
        }
        public int NoteWidth { get; set; } = 16;
        public Clef ActiveClef { get; private set; }
        public ObservableCollection<NoteViewModel> NotesInLedger { get; set; }
        public int NudgeWidth { get; private set; } = 4;

        public ClefViewModel(Clef clef)
        {
            Sections = new List<NoteSection>();
            NotesInLedger = new ObservableCollection<NoteViewModel>();
            NotesInClef = new ObservableCollection<NoteViewModel>();

            ActiveClef = clef;
        }


        public void AddSection(NoteSection section,int left)
        {
            var notesInSection = new List<NoteViewModel>();

            var nudgeToFit = false;
            foreach (var note in section.AllNotes.OrderBy(x => x.Id))
            {
                var currentNoteY = NoteToPisitionInClef(note, ActiveClef);
                var newNote = CreateNoteAtPosition(note, left, 6 * currentNoteY);

                if (NotesInClef.Any() && notesInSection.Any())
                    CorrectPositionWhenAboveLastNote(NotesInClef.Last(), newNote, nudgeToFit);

                NotesInClef.Add(newNote);
                notesInSection.Add(newNote);
            }

            Sections.Add(new NoteSection(notesInSection));

            AddLedgerLines(new NoteSection(notesInSection), left);
        }

        private void AddLedgerLines(NoteSection section, int xoffset)
        {
            CreateTrailingLinesForSection(section)
            .ForEach(note => NotesInLedger.Add(
                   new NoteViewModel(note.Note)
                   {
                       X = xoffset,
                       Y = 6 * NoteToPisitionInClef(note.Note, ActiveClef)
                   })
                );
        }

        private NoteViewModel CreateNoteAtPosition(Note note, int x, int y)
        {
            if (note.IsSharp)
                return new SharpNote(note) { X = x, Y = y };

            return new NoteViewModel(note) { X = x, Y = y };
        }

        public int Right()
        {
            if (Sections.Any())
                return Sections.Sum(section => CalculateWidth(section) );

            return 0;
        }
        private int CalculateWidth(NoteSection x)
        {
            if (x.Notes == null)
                return NoteWidth;

            return x.Notes.Any(note => note.Note.IsSharp || note.Note.IsFlat) ? NoteWidth * 2 : NoteWidth;
        }

        private void CorrectPositionWhenAboveLastNote(NoteViewModel noteViewModel, NoteViewModel newNote, bool nudgeToFit)
        {
            var isDirectlyAboveLastNote = NoteToPisitionInClef(noteViewModel.Note, ActiveClef) - NoteToPisitionInClef(newNote.Note, ActiveClef) == 1;
            if (isDirectlyAboveLastNote)
            {
                nudgeToFit = !nudgeToFit;
                newNote.X += nudgeToFit ? NudgeWidth : 0;
            }
            else
                nudgeToFit = false;
        }

        //public int GetNumberOfLedgerLinesInNote(Note note)
        //{
        //    return Notes.NumberOfLedgerLines(note, ActiveClef);
        //}

        private List<NoteViewModel> CreateTrailingLinesForSection(NoteSection noteSection)
        {
            if (noteSection.Notes.Count <= 0)
                return new List<NoteViewModel>();

            var linesToFill = new List<NoteViewModel>();

            linesToFill.AddRange(Notes.GetNotesInLedger(noteSection.HighestNote, ActiveClef).Select(x => new NoteViewModel(x)));
            linesToFill.AddRange(Notes.GetNotesInLedger(noteSection.LowestNote, ActiveClef).Select(x => new NoteViewModel(x)));

            return linesToFill;
        }

        private int NoteToPisitionInClef(Note note, Clef clef)
        {
            var idx = clef == Clef.Bass ? Notes.BassNotes.IndexOf(note) : Notes.TrebleNotes.IndexOf(note);
            return Notes.BassNotes.Count - idx;
        }
    }
}

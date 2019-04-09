using MVVMHelpers;
using SheetLearner.Music.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XTestMan.Views.Music
{
    public class NoteSection : ViewModelBase
    {
        private List<Note> _section;
        public virtual List<Note> Section { get => _section; 
            set
            {
                _section = value;
                OnPropertyChanged(); 
            }
        }

        public List<Note> AllNotes { get; set; }

        public NoteSection()
        {
            Notes = new List<NoteViewModel>();
            BottomLedger = Enumerable.Repeat<LedgerNote>(new LedgerNote(new Note(), false), 2).ToList();
            TopLedger = Enumerable.Repeat<LedgerNote>(new LedgerNote(new Note(), false), 2).ToList();
            AllNotes = new List<Note>();
        } 

        public NoteSection(List<NoteViewModel> notes)
        {
            Notes = notes;
        }

        public static NoteSection EmptySection()
        {
            return new NoteSection();
        }

        public bool IsAllPlayed()
        {
            return Notes.All(x => x.Played);
        }

        public void SetAllPlayed()
        {
            Notes.ForEach(x => x.Played = true);
        }

        public Note HighestNote
        {
            get
            {
                return  Notes.OrderBy(x => NotesFactory.BassNote.IndexOf(x.Note)).First().Note;
            }
        }

        public Note LowestNote
        {
            get
            {
                return  Notes.OrderBy(x => NotesFactory.BassNote.IndexOf(x.Note)).Last().Note;
            }
        }

        public static NoteSection CreateSectionFromNotes(List<Note> bar, Clef clef, Sheet sheet)
        {
            var section = new NoteSection();
            var notes = new List<Note>();
            notes.AddRange(Enumerable.Repeat<Note>(new Note(), NotesFactory.BassNote.Count));
            section.Section = notes;
            return section; 
        } 

        private List<LedgerNote> _topLedger;
        public List<LedgerNote> TopLedger { get => _topLedger; 
            set {
                _topLedger = value;
                OnPropertyChanged();
            }
        }

        private List<LedgerNote> _bottomLedger;

        public List<LedgerNote> BottomLedger
        {
            get { return _bottomLedger; }
            set { _bottomLedger = value; OnPropertyChanged(); }
        }

        public List<NoteViewModel> Notes { get; set; }
    }

}
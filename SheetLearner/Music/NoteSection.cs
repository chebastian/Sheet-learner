using MVVMHelpers;
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
                BottomLedger = Enumerable.Repeat<LedgerNote>(new LedgerNote(new Note(),false),2).ToList();
                TopLedger = Enumerable.Repeat<LedgerNote>(new LedgerNote(new Note(),false),2).ToList();
            AllNotes = new List<Note>();
        }

        public static NoteSection EmptySection()
        {
            return new NoteSection();
        }

        public static NoteSection CreateSectionFromNotes(List<Note> bar, Clef clef, Sheet sheet)
        {
            var section = new NoteSection();

            var possible = Sheet.GetNonLedgerNotesInClef(clef);
            var notes = new List<Note>();
            notes.AddRange(Enumerable.Repeat<Note>(new Note(), possible.Count));

            foreach(var note in bar)
            {
                var c = sheet.GetNoteValueInClef(clef, note);
                note.Value = c; 

                if (IsTopLedger(note,clef))
                    section.AddToTopLedger(note);
                else if (IsBottomLedger(note,clef))
                    section.AddToBottomLedger(note);
                else 
                    notes[c] = note;

                section.AllNotes.Add(note);
            }


            section.Section = notes;
            return section;
        }

        private static bool IsTopLedger(Note n, Clef clef)
        {
            var topLedgerNotes = String.Empty;

            if (clef.Equals(Clef.Treble))
                topLedgerNotes = "GA";
            else
                topLedgerNotes = "BC";

            return topLedgerNotes.Contains(n.Id);
        }

        private static bool IsBottomLedger(Note n, Clef clef)
        {
            var bottomLedgerNotes = String.Empty;

            if (clef.Equals(Clef.Bass))
                bottomLedgerNotes = "ef";
            else
                bottomLedgerNotes = "cd";

            return bottomLedgerNotes.Contains(n.Id);
        }

        private void AddToBottomLedger(Note note)
        {
            if (BottomLedger == null)
                BottomLedger = Enumerable.Repeat<LedgerNote>(new LedgerNote(note,false),2).ToList();

 
            BottomLedger[note.Value] = new LedgerNote(note, note.Value%2 > 0);
        }

        private void AddToTopLedger(Note note)
        {
            if (TopLedger == null)
                BottomLedger = Enumerable.Repeat<LedgerNote>(new LedgerNote(note,false),2).ToList();


            TopLedger[note.Value] = new LedgerNote(note, note.Value % 2 == 0);
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


    }

}
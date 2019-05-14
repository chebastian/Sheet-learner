using MVVMHelpers;
using SheetLearner.Music;
using SheetLearner.Music.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using XTestMan.Views.Music;

namespace SheetLearner.Music
{
    public class NoteSection : ViewModelBase
    {

        public List<Note> AllNotes { get; set; }

        public NoteSection()
        {
            Notes = new List<NoteViewModel>();
            BottomLedger = Enumerable.Repeat(new LedgerNote(new Note(), false), 2).ToList();
            TopLedger = Enumerable.Repeat(new LedgerNote(new Note(), false), 2).ToList();
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
                return Notes.OrderBy(x => Music.Notes.BassNotes.IndexOf(x.Note)).First().Note;
            }
        }

        public Note LowestNote
        {
            get
            {
                return Notes.OrderBy(x => Music.Notes.BassNotes.IndexOf(x.Note)).Last().Note;
            }
        }

        private List<LedgerNote> _topLedger;
        public List<LedgerNote> TopLedger
        {
            get => _topLedger;
            set
            {
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

	public class ChordSection : NoteSection
	{
		public enum NoteExtreme
		{
			Top,
			Bottom
		}

		public enum RelationToMid
		{
			Above,Below
		}

		public ChordSection()
		{
		}

		public ChordSection(List<NoteViewModel> notes) : base(notes)
		{
		} 

		public bool EqualFromMid(Clef clef)
		{
			return HighestNote.DistanceToMidPointAbs(clef) == LowestNote.DistanceToMidPointAbs(clef);
		}

		public NoteExtreme FartherFromMid(Clef clef)
		{
			return HighestNote.DistanceToMidPointAbs(clef) > LowestNote.DistanceToMidPointAbs(clef) ? NoteExtreme.Top : NoteExtreme.Bottom;
		}

		public Relation MajorityRelation(Clef clef)
		{
			var relations = Notes.Select(x => x.Note.RelationToMidpoint(clef));
			if (relations.Where(x => x == Relation.Higher).ToList().Count > relations.Where(x => x == Relation.Lower).ToList().Count)
				return Relation.Higher;

			return Relation.Lower;
		}
	}

}
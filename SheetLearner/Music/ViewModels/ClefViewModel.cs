using MVVMHelpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SheetLearner.Music;
using System;
using System.Threading.Tasks;

namespace SheetLearner.Music.ViewModels
{
	public class NoteViewModel : ViewModelBase
	{

		public int StemEnd { get; set; }

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

		public int StemX { get; internal set; }
		public int StemY { get; internal set; }
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
		public int NudgeWidth { get; private set; } = 8;

		public ClefViewModel(Clef clef)
		{
			Sections = new List<NoteSection>();
			NotesInLedger = new ObservableCollection<NoteViewModel>();
			NotesInClef = new ObservableCollection<NoteViewModel>();

			ActiveClef = clef;
		}

		public async Task AddSection(NoteSection section, int left)
		{
			var notesInSection = new List<NoteViewModel>();

			var nudgeToFit = false;
			foreach (var note in section.AllNotes.OrderBy(x => x.Id))
			{
				var currentNoteY = NotesIndexInClef(note);
				var newNote = CreateNoteAtIndex(note, left, currentNoteY);

				if (NotesInClef.Any() && notesInSection.Any())
					CorrectPositionWhenAboveLastNote(NotesInClef.Last(), newNote, nudgeToFit);

				NotesInClef.Add(newNote);
				notesInSection.Add(newNote);
			}

			if (notesInSection.Count > 1)
				Sections.Add(new ChordSection(notesInSection));
			else
				Sections.Add(new NoteSection(notesInSection));

			AddLedgerLines(Sections.Last(), left);
			AddNoteStems(Sections.Last());
		}

		internal void ClearNotes()
		{
			Sections.Clear();
			NotesInClef.Clear();
			NotesInLedger.Clear();
		}

		private void AddNoteStems(NoteSection section)
		{
			if (section is ChordSection)
			{
				AddNoteStems(section as ChordSection);
				return;
			}

			foreach (var note in section.Notes)
			{
				var octave = note.Note;
				var relation = note.Note.RelationToMidpoint(ActiveClef);
				octave = relation != Relation.Lower ? note.Note.OctaveDown(ActiveClef) :
													note.Note.OctaveUp(ActiveClef);

				if (Notes.IsOuterLedger(note.Note, ActiveClef))
					octave = Notes.Midpoint(ActiveClef);

				var offsets = relation != Relation.Lower ?
					new { x = 3, y = 3, noteIndexCorrection = 0 } :
					new { x = 14, y = 6, noteIndexCorrection = -2 };

				//Adds correction when mid
				if (octave == Notes.Midpoint(ActiveClef))
					offsets = new { x = offsets.x, y = offsets.y, noteIndexCorrection = -1 };

				var ocUp = (NotesIndexInClef(octave) - offsets.noteIndexCorrection) * 6;
				note.StemEnd = ocUp;
				note.StemY = note.Y + offsets.y;
				note.StemX = note.X + offsets.x;
			}
		}

		internal class Stem
		{
			public enum Horizontal
			{
				Left,
				Right,
				Mid
			}

			public enum Direction
			{
				Up,
				Down
			}

			public int PosX()
			{
				if (HorizontalOrientaion == Horizontal.Mid)
					return 14;
				else if (HorizontalOrientaion == Horizontal.Left)
					return 3;

				return 14;
			}

			public int Start()
			{
				var offsets = StemDirection == Direction.Up ?
					new { x = 3, y = 3, noteIndexCorrection = 0 } :
					new { x = 14, y = 6, noteIndexCorrection = -2 };

				return offsets.y;
			}

			public Horizontal HorizontalOrientaion { get; set; }
			public Direction StemDirection { get; set; }

			public int Top { get; set; }
			public int Bottom { get; set; }
			public int Left { get; set; }
		}

		private Stem GetStemForNote(NoteViewModel vnote)
		{
			var note = vnote.Note;
			var octave = note;
			var relation = note.RelationToMidpoint(ActiveClef);
			octave = relation != Relation.Lower ? note.OctaveDown(ActiveClef) :
												note.OctaveUp(ActiveClef);

			if (Notes.IsOuterLedger(note, ActiveClef))
				octave = Notes.Midpoint(ActiveClef);

			var offsets = relation != Relation.Lower ?
				new { x = 3, y = 3, noteIndexCorrection = 0 } :
				new { x = 14, y = 6, noteIndexCorrection = -2 };

			//Adds correction when mid
			if (octave == Notes.Midpoint(ActiveClef))
				offsets = new { x = offsets.x, y = offsets.y, noteIndexCorrection = -1 };

			var ocUp = (NotesIndexInClef(octave) - offsets.noteIndexCorrection) * 6;

			var stem = new Stem();
			stem.Bottom = ocUp;
			stem.Top = vnote.Y + offsets.y;
			stem.Left = vnote.X + offsets.x;

			return stem;

		}



		private void AddNoteStems(ChordSection chord)
		{
			var stem = new Stem();
			var stems = new List<Stem>();

			var isDown = false;
			var offsetX = 0;

			if (chord.EqualFromMid(ActiveClef))
			{
				stem.StemDirection = Stem.Direction.Down;
			}
			else
			{
				stem.StemDirection = chord.MajorityRelation(ActiveClef) == Relation.Lower ? Stem.Direction.Up : Stem.Direction.Down;
			}

			if (chord.HasInterval(ChordSection.Interval.Second, ActiveClef))
			{
				stem.HorizontalOrientaion = Stem.Horizontal.Mid;
			}
			else
			{
				stem.HorizontalOrientaion = stem.StemDirection == Stem.Direction.Down ? Stem.Horizontal.Left : Stem.Horizontal.Right;
			}

			////TODO remove hack for chords with second intervals
			//if (chord.HasInterval(ChordSection.Interval.Second, ActiveClef))
			//{
			//	var note = chord.FirstNoteInSecond(ActiveClef);
			//	var target = stem.StemDirection == Stem.Direction.Down ? note.Note.OctaveDown(ActiveClef) : note.Note.OctaveUp(ActiveClef);
			//	var pos = NoteToPisitionInClef(target);
			//	var len = 6 * 6;
			//	note.StemX = note.X + stem.PosX();
			//	note.StemY = note.Y + stem.Start();
			//	note.StemEnd = note.StemY + (stem.StemDirection == Stem.Direction.Up ? -len : len);
			//	return;
			//}

			//TODO remove this in favor for calculating high and low based on direction of stem
			var outerNote = stem.StemDirection == Stem.Direction.Down ? chord.Lowest(ActiveClef) : chord.Highest(ActiveClef);
			{
				var endWith = stem.StemDirection == Stem.Direction.Down ? outerNote.Note.OctaveDown(ActiveClef) : outerNote.Note.OctaveUp(ActiveClef);
				var lst = GetYPos(endWith,stem);
				var innerNote = stem.StemDirection == Stem.Direction.Down ? chord.Highest(ActiveClef) : chord.Lowest(ActiveClef);

				outerNote.StemX = outerNote.X + stem.PosX();
				outerNote.StemY = outerNote.Y + stem.Start();
				outerNote.StemEnd = lst;

				innerNote.StemX = outerNote.X + stem.PosX();
				innerNote.StemY = innerNote.Y + stem.Start();
				innerNote.StemEnd = outerNote.StemY;
				return;
			}
		}

		private void AddLedgerLines(NoteSection section, int xoffset)
		{
			CreateTrailingLinesForSection(section)
			.ForEach(note => NotesInLedger.Add(
				   new NoteViewModel(note.Note)
				   {
					   X = xoffset,
					   Y = 6 * NotesIndexInClef(note.Note)
				   })
				);
		}

		private NoteViewModel CreateNoteAtIndex(Note note, int x, int y)
		{
			var midNote = Notes.Midpoint(ActiveClef);
			var midIndex = NotesIndexInClef(midNote);
			var dd = Notes.DistanceFromMid(note, ActiveClef);

			if (note.IsSharp)
				return new SharpNote(note) { X = x, Y = y };

			return new NoteViewModel(note) { X = x, Y = dd * 6};
		}

		public int Right()
		{
			if (Sections.Any())
				return Sections.Sum(section => CalculateWidth(section));

			return 0;
		}

		private int CalculateWidth(NoteSection x)
		{
			int noteDist = (int)(NoteWidth * 1.5);
			if (x.Notes == null)
				return noteDist;

			return x.Notes.Any(note => note.Note.IsSharp || note.Note.IsFlat) ? noteDist * 2 : noteDist;
		}

		private void CorrectPositionWhenAboveLastNote(NoteViewModel noteViewModel, NoteViewModel newNote, bool nudgeToFit)
		{
			var isDirectlyAboveLastNote = NotesIndexInClef(noteViewModel.Note) - NotesIndexInClef(newNote.Note) == 1;
			if (isDirectlyAboveLastNote)
			{
				nudgeToFit = !nudgeToFit;
				newNote.X += nudgeToFit ? NudgeWidth : 0;
			}
			else
				nudgeToFit = false;
		}
 
		private List<NoteViewModel> CreateTrailingLinesForSection(NoteSection noteSection)
		{
			if (noteSection.Notes.Count <= 0)
				return new List<NoteViewModel>();

			var linesToFill = new List<NoteViewModel>();

			linesToFill.AddRange(Notes.GetNotesInLedger(noteSection.HighestNote, ActiveClef).Select(x => new NoteViewModel(x)));
			linesToFill.AddRange(Notes.GetNotesInLedger(noteSection.LowestNote, ActiveClef).Select(x => new NoteViewModel(x)));

			return linesToFill;
		}

		private int GetYPos(Note note, Stem stem)
		{
			var n = NotesIndexInClef(note);
			var offset = stem.StemDirection == Stem.Direction.Down ? -2 : 2;
			return 6 * (n + offset);
		}

		private int NotesIndexInClef(Note note)
		{
			var midNote = Notes.Midpoint(ActiveClef);
			var dd = Notes.DistanceFromMid(note, ActiveClef);
			return dd;
		}
	}
}

using MVVMHelpers;
using SheetLearner.Music;
using SheetLearner.Music.Models;
using SheetLearner.Music.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Music.ViewModels
{

	public class ClefViewModel : ViewModelBase
	{
		public List<NoteSection> Sections;
		private List<NoteViewModel> _createdNotes;

		public ObservableCollection<NoteViewModel> NotesInClef
		{
			get;
			set;
		}
		public int NoteWidth { get; } = 16;
		public Clef ActiveClef { get; private set; }
		public ObservableCollection<NoteViewModel> NotesInLedger { get; set; }
		public int NudgeWidth { get; } = 12;
		public static int NoteHeight { get; } = 6;

		public ClefViewModel(Clef clef)
		{
			Sections = new List<NoteSection>();
			NotesInLedger = new ObservableCollection<NoteViewModel>();
			NotesInClef = new ObservableCollection<NoteViewModel>();
			_createdNotes = new List<NoteViewModel>();

			ActiveClef = clef;
		}

		public void AddSection(NoteSection section, int left)
		{
			var notesInSection = new List<NoteViewModel>();
			foreach (var note in section.AllNotes.OrderBy(x => x.Id))
			{
				var currentNoteY = NotesIndexInClef(note);
				var newNote = CreateNoteAtIndex(note, left, currentNoteY);
				_createdNotes.Add(newNote);
				notesInSection.Add(newNote);
			}

			if (notesInSection.Count > 1)
			{
				var chord = new ChordSection(notesInSection);
				Sections.Add(chord);
				CorrectPositionsOfChordNotes(chord);
			}
			else
				Sections.Add(new NoteSection(notesInSection));

			AddLedgerLines(Sections.Last(), left);
			AddNoteStems(Sections.Last());
		}

		public async void AddToRender()
		{
			foreach (var note in _createdNotes.ToList())
			{
				NotesInClef.Add(note);
				await Task.Delay(2);
			}

			_createdNotes.Clear();
		}


		private void CorrectPositionsOfChordNotes(ChordSection chord)
		{
			if (chord.HasInterval(ChordSection.Interval.Second, ActiveClef))
			{
				var first = chord.FirstNoteInSecond(ActiveClef);
				var notesFromBottom = chord.Notes.OrderBy(x => Notes.NotesInClef(ActiveClef).IndexOf(x.Note)).ToList();
				var idx = notesFromBottom.IndexOf(first);

				var snd = notesFromBottom[(idx + 1) % chord.Notes.Count];
				snd.X += NudgeWidth;
				notesFromBottom.Remove(snd);
				notesFromBottom.Remove(first);

				var amountToDisplace = idx > 0 ? NudgeWidth : 0;

				foreach (var note in notesFromBottom)
				{
					note.X += amountToDisplace;
				}
			}
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
					new { x = 14, y = NoteHeight, noteIndexCorrection = -2 };

				//Adds correction when mid
				if (octave == Notes.Midpoint(ActiveClef))
					offsets = new { offsets.x, offsets.y, noteIndexCorrection = -1 };

				var ocUp = (NotesIndexInClef(octave) - offsets.noteIndexCorrection) * NoteHeight;
				note.StemEnd = ocUp;
				note.StemY = note.Y + offsets.y;
				note.StemX = note.X + offsets.x;
			}
		}

		private Stem CreateStemForChord(ChordSection chord)
		{
			var stem = new Stem();

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

			return stem;

		}

		private void AddNoteStems(ChordSection chord)
		{
			var stem = CreateStemForChord(chord);
			var outerNote = stem.StemDirection == Stem.Direction.Down ? chord.Lowest(ActiveClef) : chord.Highest(ActiveClef);
			var connectingNote = stem.StemDirection == Stem.Direction.Down ? chord.Highest(ActiveClef) : chord.Lowest(ActiveClef);

			//TODO fix, this fixes offset when stem is going upp ,which causes it to reach to far since it starts at top of note
			int GetHeightCorrectedWithOffset(Stem xstem)
			{
				var idx = xstem.StemDirection == Stem.Direction.Down ? 8 : 6;
				return idx * NoteHeight;
			}

			if (chord.HasInterval(ChordSection.Interval.Second, ActiveClef))
				connectingNote.StemX = chord.FirstNoteInSecond(ActiveClef).X + stem.PosX();
			else
				connectingNote.StemX = connectingNote.X + stem.PosX();

			connectingNote.StemY = connectingNote.Y + stem.Start();
			connectingNote.StemEnd = outerNote.Y;

			outerNote.StemX = connectingNote.StemX;
			outerNote.StemY = outerNote.Y;

			var length = Math.Abs(NotesIndexInClef(outerNote.Note)) > 5 ? 3 * NoteHeight : GetHeightCorrectedWithOffset(stem);
			if (ChordIsOutsideStaff(ActiveClef, chord))
			{
				var idx = Math.Abs(NotesIndexInClef(outerNote.Note) - 1);
				length = idx * NoteHeight;
			}

			outerNote.StemEnd = outerNote.StemY + (stem.StemDirection == Stem.Direction.Down ? length : -length);
		}

		private void AddLedgerLines(NoteSection section, int xoffset)
		{
			CreateTrailingLinesForSection(section)
			.ForEach(note => NotesInLedger.Add(
				   new NoteViewModel(note.Note)
				   {
					   X = xoffset,
					   Y = NoteHeight * NotesIndexInClef(note.Note)
				   })
				);
		}

		private NoteViewModel CreateNoteAtIndex(Note note, int x, int y)
		{
			var midNote = Notes.Midpoint(ActiveClef);
			var dd = Notes.DistanceFromMid(note, ActiveClef);

			if (note.IsSharp)
				return new SharpNote(note) { X = x, Y = y };

			return new NoteViewModel(note) { X = x, Y = dd * NoteHeight };
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

			return x.Notes.Any(note => note.Note.IsSharp || note.Note.IsFlat) ||
				x as ChordSection != null && (x as ChordSection).HasInterval(ChordSection.Interval.Second, ActiveClef) ? noteDist * 2 : noteDist;
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

		private bool ChordIsOutsideStaff(Clef clef, ChordSection chord)
		{
			var high = chord.Highest(clef);
			var low = chord.Lowest(clef);

			var indexes = new List<int>() { NotesIndexInClef(high.Note), NotesIndexInClef(low.Note) };
			//TODO yes this does make notes say C1 and C3 would return true, but hey that will never happen, we can assume that a chord will never have notes on opposite sides of the stem
			var isInsideStaff = indexes.Any(x => !IsOutsideLedger(x));
			return !isInsideStaff;
		}

		private bool IsOutsideLedger(int index)
		{
			return Math.Abs(index) > 5;
		}

		private int NotesIndexInClef(Note note)
		{
			var dd = Notes.DistanceFromMid(note, ActiveClef);
			return dd;
		}
	}
}

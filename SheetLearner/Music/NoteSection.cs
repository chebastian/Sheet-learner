﻿using MVVMHelpers;
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
			AllNotes = new List<Note>(notes.Select(x => x.Note));
			Notes = notes ?? new List<NoteViewModel>();
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

		public enum Interval
		{
			Frst,
			Second,
			Third,
			Fourth,
			Fifth,
			Sixth,
			Seventh,
			Octave
		}

		public enum RelationToMid
		{
			Above, Below
		}

		public ChordSection()
		{
		}

		public ChordSection(List<NoteViewModel> notes) : base(notes)
		{
		}

		public static ChordSection Build(Note a, Note b)
		{
			var section = new ChordSection();
			section.Notes.Add(new NoteViewModel( a ));
			section.Notes.Add(new NoteViewModel( b ));

			return section;
		}

		public static ChordSection Build(Note a, Note b, Note c)
		{
			var section = new ChordSection();
			section.Notes.Add(new NoteViewModel( a ));
			section.Notes.Add(new NoteViewModel( b ));
			section.Notes.Add(new NoteViewModel( c ));

			return section;
		} 

		public ChordSection Transpose(int semitones)
		{
			var newNotes = new List<Note>();
			var notes = Music.Notes.NotesInClef(Clef.Bass);
			foreach(var note in Notes.Select(x => x.Note))
			{
				var idx = notes.IndexOf(note);
				newNotes.Add(notes[idx + semitones]);
			}
			Notes = newNotes.Select(x => new NoteViewModel(x)).ToList();
			return this;
		}

		public static ChordSection Build(Note a, Note b, Note c, Note d)
		{
			var section = new ChordSection();
			section.Notes.Add(new NoteViewModel( a ));
			section.Notes.Add(new NoteViewModel( b ));
			section.Notes.Add(new NoteViewModel( c ));
			section.Notes.Add(new NoteViewModel( d ));

			return section;
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
			var relations = Notes.Select(x => x.Note.RelationToMidpoint(clef)).ToList();
			var higher = relations.Where(x => x == Relation.Higher).ToList().Count;
			var lower = relations.Where(x => x == Relation.Lower).ToList().Count; 

			return higher > lower ? Relation.Higher : (lower == higher ? Relation.Equal : Relation.Lower);
		}

		public bool HasInterval(Interval interval, Clef clef)
		{
			return IntervalsInOrder(clef).Contains(interval);
		}

		public IList<Interval> IntervalsInOrder(Clef clef)
		{	
			var intervals = new List<Interval>();
			for (var i = 0; i < Notes.Count; i++)
			{
				var note = Notes[i];
				for (var j = 0; j < Notes.Count; j++)
				{
					intervals.Add((Interval)Music.Notes.GetInterval(note.Note, Notes[j].Note, clef));
				}
			}

			return intervals.Where(x => x > 0).ToList(); 
		}

		public NoteViewModel FirstNoteInSecond(Clef clef)
		{
			var intervals = new List<Interval>();
			for (var i = 0; i < Notes.OrderBy(x => x.Y).ToList().Count; i++)
			{
				var note = Notes[i];
				for (var j = 0; j < Notes.Count; j++)
				{
					var interval = (Interval)Music.Notes.GetInterval(note.Note, Notes[j].Note, clef);
					if(interval == Interval.Second)
					{
						return note;
					}
				}
			}

			return null;
		}

		public int MinInterval(Clef clef)
		{
			//TODO used to get min interval to see if chord contains 2nd interval
			var range = Music.Notes.NotesInClef(clef);
			var intervals = new List<int>();
			for (var i = 0; i < Notes.Count; i++)
			{
				var note = Notes[i];
				for (var j = 0; j < Notes.Count; j++)
				{
					intervals.Add(Music.Notes.GetInterval(note.Note, Notes[j].Note, clef));
				}
			}

			return intervals.Where(x => x > 0).Min();
		}

		public NoteViewModel Highest(Clef clef)
		{
			var group = Music.Notes.NotesInClef(clef);
			return Notes.OrderBy(x => group.IndexOf(x.Note)).Last();
		}

		public NoteViewModel Lowest(Clef clef)
		{
			var group = Music.Notes.NotesInClef(clef);
			return Notes.OrderBy(x => group.IndexOf(x.Note)).First();
		}
	}

}
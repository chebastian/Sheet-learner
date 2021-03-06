﻿using Music.Interfaces;
using Music.Models;
using Music.ViewModels;
using SheetLearner.Music;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Music.Services
{
	public class RandomNoteReader : INoteReader
	{
		private static Random _rand;
		private int _length;
		private Clef _clef;

		public bool FavorChords { get; private set; }

		public RandomNoteReader(Clef clef, int length)
		{
			_length = length;
			_clef = clef;
			_rand = _rand ?? new Random();
		}

		public List<NoteSection> GetNoteSections()
		{
			if (FavorChords)
				return CreateLinearChordSection();

			return CreateRandomSections(_length);

			return new List<NoteSection>()
			{
				//Top and bottom 2nd chords 
				ChordSection.Build(Notes.A1,Notes.B1),
				ChordSection.Build(Notes.G2,Notes.A3),
				ChordSection.Build(Notes.E2,Notes.F2),
				ChordSection.Build(Notes.E1,Notes.F1), 

				//Triads spanning the clef
				ChordSection.Build(Notes.A2,Notes.C2,Notes.E2),
				ChordSection.Build(Notes.A2,Notes.C2,Notes.E2).Transpose(2),
				ChordSection.Build(Notes.A2,Notes.C2,Notes.E2).Transpose(4),
				ChordSection.Build(Notes.A2,Notes.C2,Notes.E2).Transpose(6),
				ChordSection.Build(Notes.A2,Notes.C2,Notes.E2).Transpose(-2),
				ChordSection.Build(Notes.A2,Notes.C2,Notes.E2).Transpose(-4),
				ChordSection.Build(Notes.A2,Notes.C2,Notes.E2).Transpose(-6), 

				//Fifths
				ChordSection.Build(Notes.C2, Notes.G2),
				ChordSection.Build(Notes.C2, Notes.G2).Transpose(-1),
				ChordSection.Build(Notes.C2, Notes.G2).Transpose(-2),
				ChordSection.Build(Notes.C2, Notes.G2).Transpose(-4),


				//Triads with 2nd
				ChordSection.Build(Notes.A1,Notes.B1,Notes.G1),
				ChordSection.Build(Notes.A1,Notes.B1,Notes.G1).Transpose(4),

				ChordSection.Build(Notes.G2,Notes.A3, Notes.C2),

			};
		}

		private List<NoteSection> CreateLinearChordSection()
		{
			var result = new List<NoteSection>();
			var start = Notes.C1;
			var baseVm = new NoteViewModel(start);
			var bases = new List<Note> { Notes.C1, Notes.C2, Notes.C3 };

			for (var i = 0; i < bases.Count; i++)
			{
				start = bases[i];
				baseVm = new NoteViewModel(bases[i]);
				for (var interval = 1; interval < 8; interval++)
				{
					var iv = Notes.GetInterval(start, interval, _clef);
					var ivVm = new NoteViewModel(iv);
					result.Add(new ChordSection(new List<NoteViewModel>() { baseVm, ivVm }));
				}
			}

			return result;
		}

		public static List<NoteSection> CreateEmpty(int len)
		{
			var section = new NoteSection();
			var notes = new List<Note>();
			notes.AddRange(Enumerable.Repeat(new Note(), Notes.BassNotes.Count));

			return Enumerable.Repeat(section, len).ToList();
		}

		public static List<NoteSection> CreateChordGroups(Clef clef, int groupLen, int num, bool startEmpty)
		{
			var result = new List<NoteSection>();
			var reader = new RandomNoteReader(clef, groupLen)
			{
				FavorChords = false
			};
			for (var i = 0; i < num; i++)
				result.AddRange(reader.GetNoteSections());
			return result;
		}

		public static List<NoteSection> CreateGroups(Clef clef, int groupLength, int numGroups, bool startEmpty)
		{
			var ret = new List<NoteSection>();
			var reader = new RandomNoteReader(clef, groupLength);

			while (ret.Count < groupLength * numGroups)
			{
				if (startEmpty)
				{
					ret.AddRange(reader.GetNoteSections());
					ret.AddRange(CreateEmpty(groupLength));
				}
				else
				{
					ret.AddRange(CreateEmpty(groupLength));
					ret.AddRange(reader.GetNoteSections());
				}
			}

			return ret;
		}

		private List<NoteSection> CreateRandomSections(int count)
		{
			var notes = Notes.AllNotes;
			var list = new List<NoteSection>();

			var lastNote = Notes.C1;
			var stepSize = 8;
			for (var i = 0; i < count; i++)
			{
				if (_rand.NextDouble() < 0.3)
				{
					var first = _rand.Next(notes.Count - 1);
					var third = _rand.Next(notes.Count - 1);
					var noteA = new NoteViewModel(notes[first]);
					var randIndex = _rand.Next(stepSize);
					var inter = Notes.NotesInRange(Notes.NotesInClef(_clef), lastNote, Notes.GetInterval(lastNote, 7, _clef));
					inter = inter.OrderBy(x => _rand.Next()).ToList();
					var st = new Stack<Note>(inter);

					if (list.Count > 0)
						noteA = new NoteViewModel(st.Pop());

					var noteB = new NoteViewModel(st.Pop());
					var noteC = new NoteViewModel(st.Pop());
					list.Add(new NoteSection(new List<NoteViewModel> { noteA, noteB, noteC }));

				}
				//else if(rand.NextDouble() < 0.4)
				//    list.Add(new NoteSection(new List<NoteViewModel>(){ new NoteViewModel(Notes.GetInterval(lastNote,rand.Next(stepSize),_clef).Sharped())}));
				else
					list.Add(new NoteSection(new List<NoteViewModel>() { new NoteViewModel(Notes.GetInterval(lastNote, _rand.Next(stepSize), _clef)) }));
			}

			return list;
		}
	}
}

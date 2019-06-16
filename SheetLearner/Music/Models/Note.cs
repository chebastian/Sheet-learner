using MVVMHelpers;
using SheetLearner.Music.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SheetLearner.Music
{
	public enum Relation
	{
		Higher,
		Lower,
		Equal
	}

	public static class Notes
	{
		public static List<Note> C_Octave => Note.NotesFromString("cdefgAB");

		public static string AllIdentifiers => "c,c#,d,d#,e,f,f#,g,g#,a,a#,b";

		public static Note C0 = new Note("0");
		public static Note D0 = new Note("1");
		public static Note E0 = new Note("2");
		public static Note F0 = new Note("3");
		public static Note G0 = new Note("4");
		public static Note A0 = new Note("5");
		public static Note B0 = new Note("6");
		public static Note C1 = new Note("c");
		public static Note D1 = new Note("d");
		public static Note E1 = new Note("e");
		public static Note F1 = new Note("f");
		public static Note G1 = new Note("g");
		public static Note A1 = new Note("a");
		public static Note B1 = new Note("b");
		public static Note C2 = new Note("C");
		public static Note D2 = new Note("D");
		public static Note E2 = new Note("E");
		public static Note F2 = new Note("F");
		public static Note G2 = new Note("G");
		public static Note A2 = new Note("A");
		public static Note B2 = new Note("B");
		public static Note C3 = new Note("C1");
		public static Note D3 = new Note("D1");
		public static Note E3 = new Note("E1");
		public static Note F3 = new Note("F1");
		public static Note G3 = new Note("G1");
		public static Note A3 = new Note("A1");
		public static Note B3 = new Note("B1");

		public static Note C4 = new Note("C2");
		public static Note D4 = new Note("D2");
		public static Note E4 = new Note("E2");
		public static Note F4 = new Note("F2");
		public static Note G4 = new Note("G2");
		public static Note A4 = new Note("A2");
		public static Note B4 = new Note("B2");

		public static List<Note> AllNotes => new List<Note>
		{
			A0,B0,C0,D0,E0,F0,G0,
			A1,B1,C1,D1,E1,F1,G1,
			A2,B2,C2,D2,E2,F2,G2,
			A3,B3,C3,D3,E3,F3,G3,
		};

		public static List<Note> BassNotes => new List<Note>
		{
			A0,B0,C0,D0,E0,F0,G0,
			A1,B1,C1,D1,E1,F1,G1,
			A2,B2,C2,D2,E2,F2,G2,
			A3,B3,C3,D3,E3,F3,G3,
			A4,B4,C3,D4,E4,F4,G4,
		};

		public static List<Note> TrebleNotes => new List<Note>
		{
			F0, G0, A0, B0, C0, D0, E0,
			F1, G1, A1, B1, C1, D1, E1,
			F2, G2, A2, B2, C2, D2, E2,
			F3, G3, A3, B3, C3, D3, E3,
			F4, G4, A4, B4, C4, D4, E4,
		};


		public static List<Note> NotesInClef(Clef clef)
		{
			return clef == Clef.Bass ? BassNotes : TrebleNotes;
		}

		public static Note Midpoint(Clef clef)
		{
			return clef == Clef.Bass ? D2 : B2;
		}

		public static int GetInterval(Note a, Note b, Clef clef)
		{
			var notes = NotesInClef(clef);
			return Math.Abs(notes.IndexOf(a) - notes.IndexOf(b));
		}

		public static int DistanceFromMid(Note a, Clef clef)
		{
			var notes = NotesInClef(clef);
			var mid = Midpoint(clef);
			return notes.IndexOf(mid) - notes.IndexOf(a);
		}

		public static Note GetInterval(Note n, int interval, Clef clef)
		{
			var source = clef == Clef.Bass ? BassNotes : TrebleNotes;
			source = source.Select(x => x).ToList(); // TODO make a deep copy

			var idx = source.IndexOf(n);
			var maxIdx = Math.Min(idx + interval, source.Count - 1);
			return source.ElementAt(maxIdx);
		}




		public static List<Note> NotesInRange(List<Note> range, Note min, Note max)
		{
			var indexMin = range.IndexOf(min);
			var indexMax = range.IndexOf(max);

			if (indexMax < 0 || indexMin < 0)
				return new List<Note>();

			var noteMin = Math.Min(indexMin, indexMax);
			var noteMax = Math.Max(indexMin, indexMax);
			return range.GetRange(noteMin, noteMax + 1 - noteMin).ToList();
		}

		private static List<Note> WhereMatch(List<Note> notes, List<Note> match)
		{
			return notes.Where(x => match.Contains(x)).ToList();
		}

		public static bool IsOuterLedger(Note note, Clef clef)
		{
			var theNotes = clef == Clef.Bass ? new { low = Notes.E1, high = Notes.C3 } : new { low = Notes.C1, high = Notes.A3 };

			var isLedger = note.RelationTo(theNotes.high, clef) == Relation.Higher || note.RelationTo(theNotes.low, clef) == Relation.Lower;
			return isLedger;
		}

		public static List<Note> GetNotesInLedger(Note note, Clef clef)
		{
			var result = new List<Note>();

			if (clef == Clef.Bass)
			{
				var bottom = new List<Note>() { A1, C1, E1 };
				var top = new List<Note>() { C3, E3, G3 };

				var topNotes = NotesInRange(BassNotes, G3, C3);
				var topLedger = NotesInRange(topNotes, note, C3);

				var bottomNotes = NotesInRange(BassNotes, A1, E1);
				var bottomLedger = NotesInRange(bottomNotes, note, E1);

				result = bottomLedger.Any() ? WhereMatch(bottomLedger, bottom) : WhereMatch(topLedger, top);
			}
			else if (clef == Clef.Treble)
			{
				var bottom = new List<Note>() { F1, A1, C1 };
				var top = new List<Note>() { A3, C3, E3 };

				var topNotes = NotesInRange(TrebleNotes, A3, E3);
				var topLedger = NotesInRange(topNotes, note, A3);

				var bottomNotes = NotesInRange(TrebleNotes, F1, C1);
				var bottomLedger = NotesInRange(bottomNotes, note, C1);

				result = bottomLedger.Any() ? WhereMatch(bottomLedger, bottom) : WhereMatch(topLedger, top);
			}

			return result;
		}

		public static int NumberOfLedgerLines(Note note, Clef clef)
		{
			if (clef == Clef.Bass)
			{
				var ledgerDict = new Dictionary<Note, int>()
				{
                    //bottom
                    {A1,3},
					{B1,2},
					{C1,2},
					{D1,1},
					{E1,1},
					{F1,0},

                    //Top
                    {B3,0},
					{C3,1},
					{D3,1},
					{E3,2},
					{F3,2},
					{G3,3},
				};

				if (ledgerDict.TryGetValue(note, out var bassCount))
				{
					return bassCount;
				}
			}

			if (clef == Clef.Treble)
			{
				var ledgerDict = new Dictionary<Note, int>()
				{
                    //F1, G1, A1, B1, C1, D1, E1,
                    //bottom
                    {F1,3},
					{G1,2},
					{A1,2},
					{B1,1},
					{C1,1},
					{D1,0},

                    //Top F3, G3, A3, B3, C3, D3, E3,
                    {F3,0},
					{G3,0},
					{A3,1},
					{B3,1},
					{C3,2},
					{D3,2},
					{E3,3},
				};

				if (ledgerDict.TryGetValue(note, out var bassCount))
				{
					return bassCount;
				}
			}

			return 0;
		}
		public static List<Note> BassLowerLedger => new List<Note>()
		{
			E1,
			F1
		};

		public static List<Note> TrebbleLowerLedger => new List<Note>()
		{
			C1,D1
		};

		public static List<Note> TrebbleUpperLedger => new List<Note>()
		{
			G2,A2
		};

		public static List<Note> BassUpperLedger => new List<Note>()
		{
			B2,C2
		};



	}

	[DebuggerDisplay("{_note} : {Value}")]
	public class Note : ViewModelBase
	{
		public Note()
		{
			//Value = 0;
			_note = string.Empty;
		}

		public Note(string note)
		{
			_note = note;
			IsSharp = note.Contains("#");
		}

		public Note(Note note)
		{
			_note = note._note;
			_isSharp = note.IsSharp;
			//Value = note.Value;
		}

		public static List<Note> Triad(string a, string b, string c)
		{
			return new List<Note>() { new Note(a), new Note(b), new Note(c) };
		}

		public Note Sharped()
		{
			return new Note(_note) { IsSharp = true };
		}

		public Note OctaveUp(Clef clef)
		{
			var notes = Notes.NotesInClef(clef);
			return notes.Skip(notes.IndexOf(this) + 8).FirstOrDefault();
		}

		public Note OctaveDown(Clef clef)
		{
			var notes = Notes.NotesInClef(clef);
			return notes.Skip(notes.IndexOf(this) - 8).FirstOrDefault();
		}

		public Relation RelationToMidpoint(Clef clef)
		{
			return RelationTo(Notes.Midpoint(clef), clef);
		}

		public int DistanceToMidPointAbs(Clef clef)
		{
			return DistanceToAbs(Notes.Midpoint(clef), clef);
		}

		public int DistanceTo(Note note, Clef clef)
		{
			var notes = Notes.NotesInClef(clef);
			var thisIndex = notes.IndexOf(this);
			var other = notes.IndexOf(note);

			return thisIndex - other;
		}

		public int DistanceToAbs(Note note, Clef clef)
		{
			return Math.Abs(DistanceTo(note, clef));
		}

		public Relation RelationTo(Note note, Clef clef)
		{
			var notes = Notes.NotesInClef(clef);
			var thisIndex = notes.IndexOf(this);
			var other = notes.IndexOf(note);

			if (thisIndex - other > 0)
				return Relation.Higher;

			if (thisIndex == other)
				return Relation.Equal;

			return Relation.Lower;
		}

		public Note Flattened()
		{
			var index = Notes.AllNotes.IndexOf(this);
			index = (index - 1) % Notes.AllNotes.Count;
			return new Note(Notes.AllNotes[index]);
		}

		public bool IsEmpty()
		{
			return string.IsNullOrWhiteSpace(_note);
		}

		public Note Root
		{
			get
			{
				return IsSharp ? Flattened() : this;
			}
		}


		protected int _val;
		protected string _note;
		private bool _isSharp;
		private bool _isFlat;

		public bool Show { get => _note.Length > 0; }
		public string Id { get => _note; }

		//public int Value
		//{
		//    get { return _val; }
		//    set { _val = value; OnPropertyChanged(); }
		//}

		public bool IsSharp
		{
			get { return _isSharp; }
			set { _isSharp = value; OnPropertyChanged(); }
		}

		public bool IsFlat
		{
			get { return _isFlat; }
			set { _isFlat = value; OnPropertyChanged(); }
		}

		public static List<Note> NotesFromString(string notes)
		{
			return notes.ToCharArray().Select(x => new Note(x.ToString())).ToList();
		}

		public override bool Equals(object obj)
		{
			return obj is Note note && _note == note._note;
		}

		public bool Equals(Note note)
		{
			var idx = Notes.TrebleNotes.IndexOf(note) % 8;
			var myidx = Notes.TrebleNotes.IndexOf(this) % 8;
			return idx == myidx;
		}

		public override int GetHashCode()
		{
			var hashCode = 1925529221;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_note);
			return hashCode;
		}
	}
}
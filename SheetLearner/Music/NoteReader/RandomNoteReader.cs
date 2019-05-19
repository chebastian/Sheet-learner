using SheetLearner.Music;
using SheetLearner.Music.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTestMan.Views.Music.Interfaces;

namespace XTestMan.Views.Music.NoteReader
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
				for(var interval = 1; interval < 8; interval++)
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
            notes.AddRange(Enumerable.Repeat<Note>(new Note(), Notes.BassNotes.Count));

            return Enumerable.Repeat(section,len).ToList(); 
        }
 
		public static List<NoteSection> CreateChordGroups(Clef clef,int groupLen, int num,bool startEmpty)
		{
			var result = new List<NoteSection>();
			var reader = new RandomNoteReader(clef,groupLen);
			reader.FavorChords = false;
			for(var i =0; i < num; i++)
				result.AddRange( reader.GetNoteSections() );
			return result;
		}

        public static List<NoteSection> CreateGroups(Clef clef, int groupLength, int numGroups, bool startEmpty)
        {
            var ret = new List<NoteSection>();
            var reader = new RandomNoteReader(clef,groupLength);

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
            for(var i =0; i < count; i++)
            {
                if (_rand.NextDouble() < 0.3)
                {
                    var first = _rand.Next(notes.Count - 1);
                    var third = _rand.Next(notes.Count - 1);
                    var noteA = new NoteViewModel(notes[first]);
                    if (list.Count > 0)
                        noteA = new NoteViewModel( Notes.GetInterval(lastNote, _rand.Next(stepSize), _clef) );
                    var noteB = new NoteViewModel(Notes.GetInterval(noteA.Note, _rand.Next(stepSize-3),_clef));
                    var noteC = new NoteViewModel(Notes.GetInterval(noteB.Note, _rand.Next(stepSize-3),_clef));
                    list.Add(new NoteSection(new List<NoteViewModel> { noteA, noteB,noteC }));

                }
                //else if(rand.NextDouble() < 0.4)
                //    list.Add(new NoteSection(new List<NoteViewModel>(){ new NoteViewModel(Notes.GetInterval(lastNote,rand.Next(stepSize),_clef).Sharped())}));
                else
                    list.Add(new NoteSection(new List<NoteViewModel>(){ new NoteViewModel(Notes.GetInterval(lastNote,_rand.Next(stepSize),_clef))}));
            }

            return list;
        }
    }
}

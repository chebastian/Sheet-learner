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
        private static Random rand;
        private int _length;
        private Clef _clef;

        public RandomNoteReader(Clef clef, int length)
        {
            _length = length;
            _clef = clef;
        } 

        public List<NoteSection> GetNoteSections()
        {
            return CreateRandomSections(_length);
        }
 
        public static List<NoteSection> CreateEmpty(int len)
        {
            var section = new NoteSection();
            var notes = new List<Note>();
            notes.AddRange(Enumerable.Repeat<Note>(new Note(), Notes.BassNotes.Count));
            section.Section = notes;

            return Enumerable.Repeat(section,len).ToList(); 
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
            rand = rand ?? new Random();
            var list = new List<NoteSection>();

            var lastNote = Notes.C1;
            var stepSize = 8;
            for(var i =0; i < count; i++)
            {
                if (rand.NextDouble() < 0.3)
                {
                    var first = rand.Next(notes.Count - 1);
                    var third = rand.Next(notes.Count - 1);
                    var noteA = new NoteViewModel(notes[first]);
                    if (list.Count > 0)
                        noteA = new NoteViewModel( Notes.GetInterval(lastNote, rand.Next(stepSize), _clef) );
                    var noteB = new NoteViewModel(Notes.GetInterval(noteA.Note, rand.Next(stepSize),_clef));
                    list.Add(new NoteSection(new List<NoteViewModel> { noteA, noteB }));

                }
                //else if(rand.NextDouble() < 0.4)
                //    list.Add(new NoteSection(new List<NoteViewModel>(){ new NoteViewModel(Notes.GetInterval(lastNote,rand.Next(stepSize),_clef).Sharped())}));
                else
                    list.Add(new NoteSection(new List<NoteViewModel>(){ new NoteViewModel(Notes.GetInterval(lastNote,rand.Next(stepSize),_clef))}));
            }

            return list;
        }
    }
}

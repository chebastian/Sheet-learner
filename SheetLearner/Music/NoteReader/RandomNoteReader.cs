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
        private Clef _clef;

        public RandomNoteReader(Clef clef)
        {
            _clef = clef;
        } 

        public List<NoteSection> GetNoteSections()
        {
            return CreateRandomSections(_clef,40);
        }
 
        public static List<NoteSection> CreateEmpty(int len)
        {
            var section = new NoteSection();
            var notes = new List<Note>();
            notes.AddRange(Enumerable.Repeat<Note>(new Note(), NotesFactory.BassNotes.Count));
            section.Section = notes;

            return Enumerable.Repeat(section,len).ToList(); 
        }

        public static List<NoteSection> CreateRandomSectionFromClef(Clef clef, int len)
        {
            var reader = new RandomNoteReader(clef);
            return reader.CreateRandomSections(clef, len);
        }

        public static List<NoteSection> CreateGroups(Clef clef, int groupLength, int numGroups, bool startEmpty)
        {
            var ret = new List<NoteSection>();

            while (ret.Count < groupLength * numGroups)
            {
                if (startEmpty)
                {
                    ret.AddRange(CreateRandomSectionFromClef(clef, groupLength));
                    ret.AddRange(CreateEmpty(groupLength));
                }
                else
                {
                    ret.AddRange(CreateEmpty(groupLength));
                    ret.AddRange(CreateRandomSectionFromClef(clef, groupLength));
                }
            }

            return ret;
        }

        private List<NoteSection> CreateRandomSections(int count)
        {
            var notes = Sheet.GetNotesInActiveClef(clef);
            rand = rand ?? new Random();
            var list = new List<NoteSection>();

            var lastNote = NotesFactory.C1;
            var stepSize = 8;
            for(var i =0; i < count; i++)
            {
                if (rand.NextDouble() < 0.3)
                {
                    var first = rand.Next(notes.Count - 1);
                    var third = rand.Next(notes.Count - 1);
                    var noteA = new NoteViewModel(notes[first]);
                    if (list.Count > 0)
                        noteA = new NoteViewModel( NotesFactory.GetInterval(lastNote, rand.Next(stepSize), clef) );
                    var noteB = new NoteViewModel(NotesFactory.GetInterval(noteA.Note, rand.Next(stepSize),clef));
                    list.Add(new NoteSection(new List<NoteViewModel> { noteA, noteB }));

                }
                else
                    list.Add(new NoteSection(new List<NoteViewModel>(){ new NoteViewModel(NotesFactory.GetInterval(lastNote,rand.Next(stepSize),clef))}));
            }

            return list;
        }
    }
}

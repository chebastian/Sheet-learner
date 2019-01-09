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

        public static List<NoteSection> CreateRandomSectionsFromClef(Clef clef)
        {
            var reader = new RandomNoteReader(clef);
            return reader.GetNoteSections();
        }

        public static List<NoteSection> CreateEmpty(int len)
        {
            var empty = NoteSection.CreateSectionFromNotes(new List<Note>(), Clef.Bass, new Sheet(Clef.Bass));
            return Enumerable.Repeat(empty,len).ToList(); 
        }

        public static List<NoteSection> CreateRandomSectionFromClef(Clef clef, int len)
        {
            var reader = new RandomNoteReader(clef);
            return reader.CreateRandomSections(clef, len, false);
        }

        public static List<NoteSection> CreateGroups(Clef clef, int groupLength, int numGroups, bool startEmpty)
        {
            var ret = new List<NoteSection>();

            while(ret.Count < groupLength * numGroups)
            {
                if(startEmpty)
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

        private List<NoteSection> CreateRandomSections(Clef clef, int count, bool waits=true)
        {
            var b = new StringBuilder();
            var res = new List<String>();
            var notes = Sheet.GetNotesInActiveClef(clef);
            rand = rand ?? new Random();

            for(var i =0; i < count; i++)
            {
                var rnd = rand.Next(notes.Count - 1);
                Func<string> nextIndex = () => 
                {
                    var isSharp = false;
                    rnd = rand.Next(notes.Count - 1); 

                    return  isSharp ? notes[rnd].Sharped().Id : notes[rnd].Id;
                };

                if (rand.NextDouble() < 0.3)
                {
                    var first = nextIndex();
                    var second = "";
                    var third = nextIndex();
                    b.Append($"{first}{second}{third},");
                }
                else if (rand.NextDouble() < 0.2 && waits)
                    b.Append("x,x,x,x,");
                else
                    b.Append($"{nextIndex()},"); 
            }

            return (new NoteStringReader(b.ToString(), clef)).GetNoteSections();
        }
    }
}

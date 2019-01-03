﻿using System;
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
            return Enumerable.Repeat(NoteSection.EmptySection(), len).ToList(); 
        }

        public static List<NoteSection> CreateRandomSectionFromClef(Clef clef, int len)
        {
            var reader = new RandomNoteReader(clef);
            return reader.CreateRandomSections(clef, len, false);
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
                    rnd = rand.Next(notes.Count - 1);
                    return notes[rnd].Id;
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

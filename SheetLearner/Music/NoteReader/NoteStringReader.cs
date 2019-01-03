using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTestMan.Views.Music.Interfaces;

namespace XTestMan.Views.Music.NoteReader
{
    public class NoteStringReader : INoteReader
    {
        private readonly Clef _clef;
        private readonly string _notes;
        private readonly Sheet _sheet;

        public NoteStringReader(String notes, Clef clef)
        {
            _clef = clef;
            _notes = notes;
            _sheet = new Sheet(clef);
        }

        public List<NoteSection> GetNoteSections()
        {
            var bars = _notes.Split(',');
            var createdSections = new List<NoteSection>();
            foreach(var section in bars)
            {
                if (string.IsNullOrEmpty(section))
                    break;

                var notesInSection = NotesInChord(section);

                //foreach(var c in section.ToCharArray())
                //{
                //    if(c.ToString() == "x")
                //    {
                //        continue;
                //    }
 
                //    var note = new Note(c.ToString());
                //    //if(c.ToString() == "#")
                //    //{

                //    //    continue;
                //    //}

                //    notesInSection.Add(note);
                //}

                createdSections.Add(NoteSection.CreateSectionFromNotes(notesInSection,_clef,_sheet));
            }

            return new List<NoteSection>(createdSections.Select(x => new PlayingNoteViewModel(x)).ToList());
        }

        private List<Note> NotesInChord(String notes)
        {
            var sharps = notes.Select((x, i) => new { val = x, index = i }).Where(x => x.val == '#' || x.val == '3').Select(x => x.index);

            var ret =  new List<Note>();
            if (notes.Length == 1)
                return new List<Note>() { new Note(notes) };

            for(var i = 0; i < notes.Length; i++)
            {
                var name = notes[i].ToString();
                if (sharps.Contains(i+1))
                {
                    name += notes[i + 1];
                    i++; // Advance to next whole note
                }

                ret.Add(new Note(name)); 
            }

            return ret;
        }
    }
}

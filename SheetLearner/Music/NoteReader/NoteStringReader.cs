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

                var notesInSection = new List<Note>();

                foreach(var c in section.ToCharArray())
                {
                    if(c.ToString() == "x")
                    {
                        continue;
                    }

                    var val = _sheet.GetNoteValueInClef(_clef,new Note(c.ToString()));
                    var note = new Note(c.ToString()) { Value = val };
                    notesInSection.Add(note);
                }

                createdSections.Add(NoteSection.CreateSectionFromNotes(notesInSection,_clef,_sheet));
            }

            return new List<NoteSection>(createdSections.Select(x => new PlayingNoteViewModel(x)).ToList());
        }
    }
}

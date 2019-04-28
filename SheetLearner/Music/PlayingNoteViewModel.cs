using SheetLearner.Music;
using System.Collections.Generic;
using System.Linq;

namespace XTestMan.Views.Music
{
    public class PlayingNoteViewModel : NoteSection
    {
        private NoteSection x;

        public PlayingNoteViewModel()
        {
                
        }

        public PlayingNoteViewModel(NoteSection x)
            :base()
        {
            this.x = x;
            TopLedger = x.TopLedger;
            BottomLedger = x.BottomLedger;
            AllNotes = x.Notes.Select(theNote => theNote.Note).ToList();
        }


        public override List<Note> Section { get => x.Section; set => x.Section = value; }

    }
}
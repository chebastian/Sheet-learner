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
            IsPlayed = false;
        }

        private bool _hit;

        public bool IsPlayed
        {
            get { return _hit; }
            set { _hit = value; OnPropertyChanged(); }
        }

        public override List<Note> Section { get => x.Section; set => x.Section = value; }

    }
}
using SheetLearner.Music;
using System.Linq;

namespace SheetViewModels.ViewModels
{
	public class PlayingNoteViewModel : NoteSection
	{
		private NoteSection x;

		public PlayingNoteViewModel()
		{

		}

		public PlayingNoteViewModel(NoteSection x)
			: base()
		{
			this.x = x;
			TopLedger = x.TopLedger;
			BottomLedger = x.BottomLedger;
			AllNotes = x.Notes.Select(theNote => theNote.Note).ToList();
		}


	}
}
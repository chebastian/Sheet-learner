using System.Collections.Generic;

namespace NoteReaderInterface
{
	public interface INoteListener
	{
		void OnNoteReleased(int note);
		void OnNotePressed(int note);
		void OnNotesPressed(List<int> notes);
	}
}

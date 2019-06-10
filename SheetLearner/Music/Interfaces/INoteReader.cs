using SheetLearner.Music;
using System.Collections.Generic;

namespace XTestMan.Views.Music.Interfaces
{
	public interface INoteReader
	{
		List<NoteSection> GetNoteSections();
	}
}

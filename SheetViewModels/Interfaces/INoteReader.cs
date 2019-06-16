using SheetLearner.Music;
using System.Collections.Generic;

namespace Music.Interfaces
{
	public interface INoteReader
	{
		List<NoteSection> GetNoteSections();
	}
}

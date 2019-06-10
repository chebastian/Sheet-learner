using SheetLearner.Music;
using System.Collections.Generic;

namespace XTestMan.Views.Music
{
	internal class RootNoteComparer : IEqualityComparer<Note>
	{
		public RootNoteComparer()
		{
		}

		public bool Equals(Note x, Note y)
		{
			//return x.Root == y.Root;
			return x.Id == y.Id;
			//throw new System.NotImplementedException();
		}

		public int GetHashCode(Note obj)
		{
			return obj.GetHashCode();
		}
	}
}
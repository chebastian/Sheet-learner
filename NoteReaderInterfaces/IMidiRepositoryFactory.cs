using NoteReaderInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteReaderInterfaces
{
	public interface IMidiRepositoryFactory
	{
		IMidiRepository CreateRepository();
	}
}

using System;
using System.Collections.Generic;

namespace NoteReaderInterface
{
	public interface IMidiRepository
	{
		List<string> AvailableDevices { get; set; }
		INotePublisher GetPublisherWithName(string name);

	}
}

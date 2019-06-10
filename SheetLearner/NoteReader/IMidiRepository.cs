using System;
using System.Collections.Generic;

namespace NoteModel
{
	public interface IMidiRepository
    {
        List<String> AvailableDevices { get; set; }
        INotePublisher GetPublisherWithName(string name);

    }
}

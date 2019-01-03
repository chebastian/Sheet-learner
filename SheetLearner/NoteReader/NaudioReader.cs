using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteReader
{
    public class NaudioReader
    {
        public NaudioReader()
        {
            var devices = NAudio.Midi.MidiIn.NumberOfDevices;
        }
    }
}

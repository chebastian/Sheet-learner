using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NoteModel
{ 
    public interface IMidiDeviceListener
    {
        void OnDeviceSelected(IMidiPublisher name);
    }

    public interface IMidiRepository
    {
        List<String> AvailableDevices { get; set; }
        IMidiPublisher GetPublisherWithName(string name);

    }

    public interface INoteListener
    {
        void OnNoteReleased(int note);
        void OnNotePressed(int note);
        void OnNotesPressed(List<int> notes);
    }

    public interface IMidiPublisher
    {
        void Register(INoteListener listener);
        void Unregister(INoteListener listener);
    }

    public class MidiKeyEventArgs : EventArgs
    {
        public int KeyInOctave { get; set; }
    }

    public class MidiListenerEventArgs : EventArgs
    {
        public String SelectedDevice { get; set; }
    } 
}

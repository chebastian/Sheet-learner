using Commons.Music.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NoteModel
{
    public interface INoteReader
    {
    }

    public interface INoteListener
    {
        void OnNotePressed(int note);
        void OnNotesPressed(List<int> notes);
    }

    public interface IMidiListener
    {
        event EventHandler MidiDeviceChanged;
    }

    public class MidiListenerEventArgs : EventArgs
    {
        public String SelectedDevice { get; set; }
    }

    public class NoteReader
    {
        private INoteListener _listener;
        private IMidiAccess _api;
        private IMidiPortDetails _device;
        private IMidiInput _input;
        public EventHandler<MidiReceivedEventArgs> OnNoteReceived;
        private IMidiListener _midiListener;

        public NoteReader(INoteListener listener, IMidiListener midiList)
        {
            _listener = listener;
            OnNoteReceived = new EventHandler<MidiReceivedEventArgs>(_input_MessageReceived);
            _midiListener = midiList;
            _midiListener.MidiDeviceChanged += _midiListener_MidiDeviceChanged;
        }

        private void _midiListener_MidiDeviceChanged(object sender, EventArgs e)
        {
        }

        public void InitDevice()
        {
            _api = MidiAccessManager.Default;
            _device = _api.Inputs.First(); 
            _input = _api.OpenInputAsync(_device.Id).Result;

            _input.MessageReceived += OnNoteReceived;
        }

        public void Close()
        {
            _input.MessageReceived -= OnNoteReceived;
            _input.CloseAsync().Wait();

        }

        private void _input_MessageReceived(object sender, MidiReceivedEventArgs e)
        {
            var data = new byte[e.Length];
            Array.Copy(e.Data, e.Start, data, 0, e.Length);
            var note = MidiMessageToNote(data.ToList());
            Debug.WriteLine($"Note Pressed: {note}");
            //_listener.OnNotePressed(note);
        }

        private String MidiMessageToNote(List<byte> msg)
        {
            var found = String.Empty;

            try
            {
                Console.WriteLine(String.Join(",", msg));
                var notes = "C,C#,D,D#,E,F,F#,G,G#,A,A#,B";
                var note = msg[1];
                var noteindex = note / 12; 
                found = notes.Split(',').ElementAt(noteindex);
            }
            catch
            {

            }

            return found; 
        }

        public void readNotes()
        {
            var api = MidiAccessManager.Default;
            var device = api.Inputs.First(); 
            var input = api.OpenInputAsync(device.Id).Result;

            var wait = new ManualResetEvent(false);
            byte[] data = null;


            input.MessageReceived += (o, e) =>
            {
                data = new byte[e.Length];
                Array.Copy(e.Data, e.Start, data, 0, e.Length);
                var note = MidiMessageToNote(data.Select(x => x).ToList());
                Debug.WriteLine($"Note Pressed: {note}" );
                //_listener.OnNotePressed(note);
                wait.Set();
            };

            wait.WaitOne((int)TimeSpan.FromSeconds(60).TotalMilliseconds);
            input.CloseAsync().Wait();
        }

        private void Input_MessageReceived(object sender, MidiReceivedEventArgs e)
        {
            //var data = new byte[e.Length];
            //Array.Copy(e.Data, e.Start, data, 0, e.Length);
            //wait.Set();
        }
    }
}

using NoteReaderInterface;
using System.Collections.Generic;

namespace NoteReader
{
	public class MidiPublisherChordifyer : INotePublisher, INoteListener
	{
		private INoteListener _listener;

		public MidiPublisherChordifyer(INotePublisher publisher)
		{
			NotesPressed = new List<int>();
			Publisher = publisher;
		}

		public static MidiPublisherChordifyer CreateChordsFromMidiNotes(INotePublisher publisher)
		{
			var chordifyer = new MidiPublisherChordifyer(publisher);
			publisher.Register(chordifyer);
			return chordifyer;
		}

		public void PublishNotesToListener(INoteListener listener)
		{
			_listener = listener;
		}

		public List<int> NotesPressed { get; set; }

		public string UniqueIdentifier => Publisher.UniqueIdentifier;

		public INotePublisher Publisher { get; }

		public void Register(INoteListener listener)
		{
			_listener = listener;
		}

		public void OnNotePressed(int note)
		{
			NotesPressed.Add(note);

			if (NotesPressed.Count > 1)
				_listener.OnNotesPressed(NotesPressed);
			else
				_listener.OnNotePressed(note);
		}

		public void OnNotesPressed(List<int> notes)
		{
		}

		public void OnNoteReleased(int note)
		{
			if (NotesPressed.Contains(note))
				NotesPressed.Remove(note);
		}

		public void Unregister(INoteListener listener)
		{
		}
	}
}

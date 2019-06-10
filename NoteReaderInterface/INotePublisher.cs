namespace NoteReaderInterface
{
	public interface INotePublisher
	{
		void Register(INoteListener listener);
		void Unregister(INoteListener listener);
	}
}

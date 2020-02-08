namespace NoteReaderInterface
{
	public interface INotePublisher
	{
		string UniqueIdentifier { get; }
		void Register(INoteListener listener);
		void Unregister(INoteListener listener);
	}
}

namespace CleaningRobot.InterfaceAdapters.Interfaces
{
    public interface IFileAdapter
    {
		bool Exists(string path);
		bool TryRead(string path, out string content);
		bool TryWrite(string path, out string content);
	}
}

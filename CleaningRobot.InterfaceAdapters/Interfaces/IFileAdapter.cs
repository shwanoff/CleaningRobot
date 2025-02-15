namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	public interface IFileAdapter
	{
		bool ValidateInput(string path, out string? error, bool mustExist = false);
		bool TryRead(string path, out string content);
		bool TryWrite(string path, string content, bool replase = true);
	}
}

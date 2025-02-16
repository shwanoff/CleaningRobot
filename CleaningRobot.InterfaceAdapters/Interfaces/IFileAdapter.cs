namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	public interface IFileAdapter
	{
		bool ValidateInput(string path, out string? error, bool mustExist = false);
		Task<string> ReadAsync(string path);
		Task WriteAsync(string path, string content, bool replase = true);
	}
}

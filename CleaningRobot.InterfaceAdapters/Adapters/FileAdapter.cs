using CleaningRobot.InterfaceAdapters.Interfaces;

namespace CleaningRobot.InterfaceAdapters.Adapters
{
	public class FileAdapter : IFileAdapter
	{
		public bool TryRead(string path, out string content)
		{
			using var reader = new StreamReader(path);
			content = reader.ReadToEnd();

			return !string.IsNullOrEmpty(content);
		}

		public bool TryWrite(string path, string content, bool replase = true)
		{
			using var writer = new StreamWriter(path, !replase);
			writer.Write(content);

			return Exists(path);
		}

		public bool ValidateInput(string path, out string? error, bool mustExist = false)
		{
			error = null;

			if (string.IsNullOrWhiteSpace(path))
			{
				error = "File path is empty";
				return false;
			}

			if (!IsPath(path))
			{
				error = $"File path '{path}' is invalid";
				return false;
			}

			if (mustExist && !Exists(path))
			{
				error = $"File '{path}' does not exist";
				return false;
			}

			return true;
		}

		private static bool Exists(string path)
		{
			return File.Exists(path);
		}

		private static bool IsPath(string path)
		{
			return Path.IsPathRooted(path);
		}
	}
}

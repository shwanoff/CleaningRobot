using CleaningRobot.InterfaceAdapters.Interfaces;

namespace CleaningRobot.InterfaceAdapters.Adapters
{
	public class FileAdapter : IFileAdapter
	{
		public async Task<string> ReadAsync(string path)
		{
			using var reader = new StreamReader(path);
			var content = await reader.ReadToEndAsync();

			if (string.IsNullOrEmpty(content))
			{
				throw new ArgumentException("File is empty");
			}

			return content;
		}

		public Task WriteAsync(string path, string content, bool replase = true)
		{
			using var writer = new StreamWriter(path, !replase);
			writer.WriteAsync(content);

			if (!Exists(path))
			{
				throw new ArgumentException("File was not created");
			}

			return Task.CompletedTask;
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

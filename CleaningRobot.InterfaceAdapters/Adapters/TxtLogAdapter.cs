using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;

namespace CleaningRobot.InterfaceAdapters.Adapters
{
	public class TxtLogAdapter(IFileAdapter fileAdapter, TxtLogConfigurationDto configuration) : ILogAdapter
	{
		private readonly IFileAdapter _fileAdapter = fileAdapter;
		private readonly TxtLogConfigurationDto _configuration = configuration;

		public async Task ErrorAsync(string message, Guid executionId, Exception? exception = null)
		{
			if (_configuration.LogLevel.Error)
			{
				await Write(message, "Error", executionId);
			}
		}

		public async Task InfoAsync(string message, Guid executionId)
		{
			if (_configuration.LogLevel.Info)
			{
				await Write(message, "Info", executionId);
			}
		}

		public async Task WarningAsync(string message, Guid executionId)
		{
			if (_configuration.LogLevel.Warning)
			{
				await Write(message, "Warning", executionId);
			}
		}

		private async Task Write(string message, string type, Guid executionId, Exception? ex = null)
		{
			string dirctory;

			if (Path.IsPathRooted(_configuration.TextLog.Path))
			{
				dirctory = _configuration.TextLog.Path;
			}
			else
			{
				var currentDirectory = Directory.GetCurrentDirectory();
				dirctory = Path.Combine(currentDirectory, _configuration.TextLog.Path);
			}

			var fileName = string.Format(_configuration.TextLog.FileNameFormat, DateTime.Now);

			var fullPath = Path.Combine(dirctory, fileName);

			var text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}];[{executionId}];[{type}];{message};{ex?.Message};{ex?.Source};{ex?.StackTrace}";

			await _fileAdapter.WriteAsync(fullPath, text, replase: false);
		}
	}
}

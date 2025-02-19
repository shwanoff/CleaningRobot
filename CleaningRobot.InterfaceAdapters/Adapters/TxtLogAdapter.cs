using CleaningRobot.Entities.Interfaces;
using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;

namespace CleaningRobot.InterfaceAdapters.Adapters
{
	public class TxtLogAdapter(IFileAdapter fileAdapter, TxtLogConfigurationDto configuration) : ILogAdapter
	{
		private readonly IFileAdapter _fileAdapter = fileAdapter;
		private readonly TxtLogConfigurationDto _configuration = configuration;

		public async Task ErrorAsync(string message, string location, Guid executionId, Exception? exception = null)
		{
			if (_configuration.LogLevel.Error)
			{
				await Write($"{location}: {message}", "Error", executionId);
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

		public async Task TraceAsync(string message, Guid executionId)
		{
			if (_configuration.LogLevel.Trace)
			{
				await Write(message, "Trace", executionId);
			}
		}

		public async Task DebugAsync(string message, Guid executionId)
		{
			if (_configuration.LogLevel.Debug)
			{
				await Write(message, "Debug", executionId);
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

			message = new string(message.Where(c => !char.IsControl(c)).ToArray());
			var exceptionMessage = ex?.Message != null ? new string(ex.Message.Where(c => !char.IsControl(c)).ToArray()) : null;
			var exceptionSource = ex?.Source != null ? new string(ex.Source.Where(c => !char.IsControl(c)).ToArray()) : null;
			var exceptionStackTrace = ex?.StackTrace != null ? new string(ex.StackTrace.Where(c => !char.IsControl(c)).ToArray()) : null;

			var text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}];[{type}];[{executionId}];{message};{exceptionMessage};{exceptionSource};{exceptionStackTrace}{Environment.NewLine}";

			await _fileAdapter.WriteAsync(fullPath, text, replace: false);
		}
	}
}

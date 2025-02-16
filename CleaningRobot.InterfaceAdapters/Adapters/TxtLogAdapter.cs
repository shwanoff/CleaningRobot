using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;

namespace CleaningRobot.InterfaceAdapters.Adapters
{
	public class TxtLogAdapter(IJsonAdapter jsonAdapter, IFileAdapter fileAdapter) : ILogAdapter
	{
		private readonly IJsonAdapter _jsonAdapter = jsonAdapter;
		private readonly IFileAdapter _fileAdapter = fileAdapter;

		private const string FILE_NAME_FORMAT = "cleaning_robot_log_{0:yyyyMMdd}.log";

		private TxtLogConnectionStringDto _configuration;

		public async Task SetupAsync(string connectionString)
		{
			_configuration = await _jsonAdapter.DeserializeAsync<TxtLogConnectionStringDto>(connectionString);
		}

		public async Task ErrorAsync(string message, Guid executionId, Exception? exception = null)
		{
			await Write(message, "ErrorAsync", executionId);
		}

		public async Task InfoAsync(string message, Guid executionId)
		{
			if (_configuration.WriteTrace)
			{
				await Write(message, "InfoAsync", executionId);
			}
		}

		public async Task WarningAsync(string message, Guid executionId)
		{
			await Write(message, "WarningAsync", executionId);
		}

		private async Task Write(string message, string type, Guid executionId)
		{
			var fileName = string.Format(FILE_NAME_FORMAT, DateTime.Now);
			var fullPath = Path.Combine(_configuration.FolderPath, fileName);

			var text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{executionId}] [{type}] {message}";

			await _fileAdapter.WriteAsync(fullPath, text, replase: false);
		}
	}
}

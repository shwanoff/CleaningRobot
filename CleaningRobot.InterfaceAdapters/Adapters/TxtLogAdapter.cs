using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;

namespace CleaningRobot.InterfaceAdapters.Adapters
{
	public class TxtLogAdapter(IJsonAdapter jsonAdapter, IFileAdapter fileAdapter) : ILogAdapter
	{
		private readonly IJsonAdapter _jsonAdapter = jsonAdapter;
		private readonly IFileAdapter _fileAdapter = fileAdapter;

		private const string FILE_NAME_FORMAT = "cleanin_robot_log_{0:yyyyMMdd}.log";

		private TxtLogConnectionStringDto _configuration;

		public void Setup(string connectionString)
		{
			if (_jsonAdapter.TryDeserialize(connectionString, out TxtLogConnectionStringDto configuration))
			{
				if (configuration != null)
				{
					_configuration = configuration;
				}
				else
				{
					throw new ArgumentException("Invalid configuration");
				}
			}
			else
			{
				throw new ArgumentException("Invalid configuration");
			}
		}

		public void Error(string message, Exception? exception = null)
		{
			Write(message, "Error");
		}

		public void Info(string message)
		{
			if (_configuration.WriteTrace)
			{
				Write(message, "Info");
			}
		}

		public void Warning(string message)
		{
			Write(message, "Warning");
		}

		private void Write(string message, string type)
		{
			var fileName = string.Format(FILE_NAME_FORMAT, DateTime.Now);
			var fullPath = Path.Combine(_configuration.FolderPath, fileName);

			var text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{type}] {message}";

			if (!_fileAdapter.TryWrite(fullPath, text, replase: false))
			{
				throw new Exception($"Error writing log to file '{fullPath}'");
			}
		}
	}
}

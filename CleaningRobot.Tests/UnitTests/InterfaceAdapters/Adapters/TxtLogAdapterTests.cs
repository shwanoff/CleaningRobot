using CleaningRobot.InterfaceAdapters.Adapters;
using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;
using Moq;

namespace CleaningRobot.Tests.UnitTests.InterfaceAdapters.Adapters
{
	[TestFixture]
	public class TxtLogAdapterTests
	{
		private Mock<IFileAdapter> _fileAdapterMock;
		private TxtLogAdapter _txtLogAdapter;
		private TxtLogConfigurationDto _configuration;
		private string _testDirectory;

		[SetUp]
		public void SetUp()
		{
			_fileAdapterMock = new Mock<IFileAdapter>();
			_testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(_testDirectory);

			_configuration = new TxtLogConfigurationDto
			{
				LogLevel = new LogLevelConfiguration
				{
					Trace = true,
					Debug = true,
					Info = true,
					Warning = true,
					Error = true
				},
				TextLog = new TextLogConfiguration
				{
					Path = _testDirectory,
					FileNameFormat = "log_{0:yyyyMMdd}.txt"
				}
			};

			_txtLogAdapter = new TxtLogAdapter(_fileAdapterMock.Object, _configuration);
		}

		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(_testDirectory))
			{
				Directory.Delete(_testDirectory, true);
			}
		}

		[Test]
		public async Task ErrorAsync_LogsErrorMessage()
		{
			var message = "Error message";
			var executionId = Guid.NewGuid();

			await _txtLogAdapter.ErrorAsync(message, nameof(ErrorAsync_LogsErrorMessage), executionId);

			_fileAdapterMock.Verify(f => f.WriteAsync(It.IsAny<string>(), It.Is<string>(s => s.Contains(message)), false), Times.Once);
		}

		[Test]
		public async Task InfoAsync_LogsInfoMessage()
		{
			var message = "Info message";
			var executionId = Guid.NewGuid();

			await _txtLogAdapter.InfoAsync(message, executionId);

			_fileAdapterMock.Verify(f => f.WriteAsync(It.IsAny<string>(), It.Is<string>(s => s.Contains(message)), false), Times.Once);
		}

		[Test]
		public async Task WarningAsync_LogsWarningMessage()
		{
			var message = "Warning message";
			var executionId = Guid.NewGuid();

			await _txtLogAdapter.WarningAsync(message, executionId);

			_fileAdapterMock.Verify(f => f.WriteAsync(It.IsAny<string>(), It.Is<string>(s => s.Contains(message)), false), Times.Once);
		}

		[Test]
		public async Task TraceAsync_LogsTraceMessage()
		{
			var message = "Trace message";
			var executionId = Guid.NewGuid();

			await _txtLogAdapter.TraceAsync(message, executionId);

			_fileAdapterMock.Verify(f => f.WriteAsync(It.IsAny<string>(), It.Is<string>(s => s.Contains(message)), false), Times.Once);
		}
	}
}

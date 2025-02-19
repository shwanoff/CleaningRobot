using CleaningRobot.InterfaceAdapters.Adapters;

namespace CleaningRobot.Tests.UnitTests.InterfaceAdapters.Adapters
{
	[TestFixture]
	public class FileAdapterTests
	{
		private FileAdapter _fileAdapter;
		private string _testDirectory;

		[SetUp]
		public void SetUp()
		{
			_fileAdapter = new FileAdapter();
			_testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(_testDirectory);
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
		public async Task ReadAsync_FileExists_ReturnsContent()
		{
			var filePath = Path.Combine(_testDirectory, "test.txt");
			var content = "Test content";
			await File.WriteAllTextAsync(filePath, content);

			var result = await _fileAdapter.ReadAsync(filePath);

			Assert.That(result, Is.EqualTo(content));
		}

		[Test]
		public void ReadAsync_FileDoesNotExist_ThrowsException()
		{
			var filePath = Path.Combine(_testDirectory, "nonexistent.txt");

			Assert.ThrowsAsync<FileNotFoundException>(async () => await _fileAdapter.ReadAsync(filePath));
		}

		[Test]
		public async Task WriteAsync_ValidPath_WritesContent()
		{
			var filePath = Path.Combine(_testDirectory, "test.txt");
			var content = "Test content";

			await _fileAdapter.WriteAsync(filePath, content);

			var result = await File.ReadAllTextAsync(filePath);
			Assert.That(result, Is.EqualTo(content));
		}

		[Test]
		public void ValidateInput_ValidPath_ReturnsTrue()
		{
			var filePath = Path.Combine(_testDirectory, "test.txt");

			var result = _fileAdapter.ValidateInput(filePath, out var error);

			Assert.That(result, Is.True);
			Assert.That(error, Is.Null);
		}

		[Test]
		public void ValidateInput_InvalidPath_ReturnsFalse()
		{
			var filePath = "invalid_path";

			var result = _fileAdapter.ValidateInput(filePath, out var error);

			Assert.That(result, Is.False);
			Assert.That(error, Is.Not.Null);
		}

		[Test]
		public void ValidateInput_FileMustExist_FileDoesNotExist_ReturnsFalse()
		{
			var filePath = Path.Combine(_testDirectory, "nonexistent.txt");

			var result = _fileAdapter.ValidateInput(filePath, out var error, mustExist: true);

			Assert.That(result, Is.False);
			Assert.That(error, Is.Not.Null);
		}
	}
}

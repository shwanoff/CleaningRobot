using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Maps
{
	[TestFixture]
	public class CreateMapCommandTests
	{
		private Mock<IRepository<Map>> _mapRepositoryMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private CreateMapCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_mapRepositoryMock = new Mock<IRepository<Map>>();
			_logAdapterMock = new Mock<ILogAdapter>();
			_handler = new CreateMapCommandHandler(_mapRepositoryMock.Object, _logAdapterMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var mapData = new string[][]
			{
				new string[] { "S", "S", "S" },
				new string[] { "S", "S", "S" },
				new string[] { "S", "S", "S" }
			};
			var cells = new Cell[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					cells[i, j] = new Cell(i, j, CellType.CleanableSpace);
				}
			}
			var map = new Map(cells);

			_mapRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Map>(), executionId))
				.ReturnsAsync(map);

			var request = new CreateMapCommand
			{
				ExecutionId = executionId,
				MapData = mapData
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.Width, Is.EqualTo(3));
			Assert.That(result.Height, Is.EqualTo(3));
			Assert.That(result.Cells.Count, Is.EqualTo(9));
			Assert.That(result.Cells.All(c => c.Type == CellType.CleanableSpace), Is.True);
		}

		[Test]
		public void Handle_NullRequest_ThrowsArgumentNullException()
		{
			// Arrange
			CreateMapCommand request = null;

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public void Handle_NullMapData_ThrowsArgumentNullException()
		{
			// Arrange
			var request = new CreateMapCommand
			{
				ExecutionId = Guid.NewGuid(),
				MapData = null
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}
	}
}
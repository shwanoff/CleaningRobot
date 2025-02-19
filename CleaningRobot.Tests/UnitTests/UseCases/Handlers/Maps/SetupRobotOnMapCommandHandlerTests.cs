using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;
using CleaningRobot.UseCases.Enums;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Maps
{
	[TestFixture]
	public class SetupRobotOnMapCommandHandlerTests
	{
		private Mock<IRepository<Robot>> _robotRepositoryMock;
		private Mock<IRepository<Map>> _mapRepositoryMock;
		private SetupRobotOnMapCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_robotRepositoryMock = new Mock<IRepository<Robot>>();
			_mapRepositoryMock = new Mock<IRepository<Map>>();
			_handler = new SetupRobotOnMapCommandHandler(_robotRepositoryMock.Object, _mapRepositoryMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var robot = new Robot(1, 1, Facing.North, 100);
			var cells = new Cell[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					cells[i, j] = new Cell(i, j, CellType.CleanableSpace)
					{
						State = CellState.NotVisited
					};
				}
			}
			var map = new Map(cells);

			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);
			_mapRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(map);
			_mapRepositoryMock.Setup(repo => repo.UpdateAsync(map, executionId)).ReturnsAsync(map);

			var request = new SetupRobotOnMapCommand
			{
				ExecutionId = executionId
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}

		[Test]
		public void Handle_CellNotAvailable_ThrowsInvalidOperationException()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var robot = new Robot(1, 1, Facing.North, 100);
			var cells = new Cell[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					cells[i, j] = new Cell(i, j, CellType.CleanableSpace)
					{
						State = CellState.NotVisited
					};
				}
			}
			cells[1, 1] = new Cell(1, 1, CellType.Wall); 
			var map = new Map(cells);

			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);
			_mapRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(map);

			var request = new SetupRobotOnMapCommand
			{
				ExecutionId = executionId
			};

			// Act & Assert
			var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(request, CancellationToken.None));
			Assert.That(ex.Message, Is.EqualTo("The robot cannot be on a wall"));
		}
	}
}
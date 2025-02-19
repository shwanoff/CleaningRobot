using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Maps
{
	[TestFixture]
	public class ExecuteMapCommandHandlerTests
	{
		private Mock<IRepository<Map>> _mapRepositoryMock;
		private Mock<IRepository<Robot>> _robotRepositoryMock;
		private Mock<IQueueRepository<Command>> _commandRepositoryMock;
		private ExecuteMapCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_mapRepositoryMock = new Mock<IRepository<Map>>();
			_robotRepositoryMock = new Mock<IRepository<Robot>>();
			_commandRepositoryMock = new Mock<IQueueRepository<Command>>();
			_handler = new ExecuteMapCommandHandler(_mapRepositoryMock.Object, _robotRepositoryMock.Object, _commandRepositoryMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Clean, 10)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true
			};

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
			var robot = new Robot(1, 1, Facing.North, 100);

			_mapRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(map);
			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);
			_commandRepositoryMock.Setup(repo => repo.UpdateFirstAsync(command, executionId)).ReturnsAsync(command);
			_mapRepositoryMock.Setup(repo => repo.UpdateAsync(map, executionId)).ReturnsAsync(map);

			var request = new ExecuteMapCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.IsCompleted, Is.True);
			Assert.That(result.Result.Width, Is.EqualTo(3));
			Assert.That(result.Result.Height, Is.EqualTo(3));
			Assert.That(result.Result.Cells.Count, Is.EqualTo(9));
			Assert.That(result.Result.Cells.All(c => c.Type == CellType.CleanableSpace), Is.True);
		}

		[Test]
		public void Handle_NullCommand_ThrowsArgumentNullException()
		{
			// Arrange
			var request = new ExecuteMapCommand
			{
				ExecutionId = Guid.NewGuid(),
				Command = null
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public void Handle_NegativeEnergyConsumption_ThrowsArgumentException()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Clean, -10)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true
			};

			var request = new ExecuteMapCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(request, CancellationToken.None));
		}
	}
}
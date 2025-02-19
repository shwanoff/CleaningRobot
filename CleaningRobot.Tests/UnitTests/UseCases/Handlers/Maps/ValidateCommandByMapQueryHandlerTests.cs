using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Maps
{
	[TestFixture]
	public class ValidateCommandByMapQueryHandlerTests
	{
		private Mock<IRepository<Map>> _mapRepositoryMock;
		private Mock<IRepository<Robot>> _robotRepositoryMock;
		private Mock<IQueueRepository<Command>> _commandRepositoryMock;
		private ValidateCommandByMapQueryHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_mapRepositoryMock = new Mock<IRepository<Map>>();
			_robotRepositoryMock = new Mock<IRepository<Robot>>();
			_commandRepositoryMock = new Mock<IQueueRepository<Command>>();
			_handler = new ValidateCommandByMapQueryHandler(_mapRepositoryMock.Object, _robotRepositoryMock.Object, _commandRepositoryMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Clean, 10)
			{
				IsValidatedByCommand = true
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

			var request = new ValidateCommandByMapQuery
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.IsValid, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}

		[Test]
		public async Task Handle_NullRequest_ReturnsValidationError()
		{
			// Arrange
			ValidateCommandByMapQuery request = null;

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo("Request cannot be null"));
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
		}

		[Test]
		public async Task Handle_NullCommand_ReturnsValidationError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var request = new ValidateCommandByMapQuery
			{
				ExecutionId = executionId,
				Command = null
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo("Command cannot be null"));
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
		}

		[Test]
		public async Task Handle_NegativeEnergyConsumption_ReturnsValidationError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Clean, -10);
			var request = new ValidateCommandByMapQuery
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo("The energy consumption of a command cannot be negative"));
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
		}

		[Test]
		public async Task Handle_NonExistentMap_ReturnsValidationError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Clean, 10);
			var request = new ValidateCommandByMapQuery
			{
				ExecutionId = executionId,
				Command = command
			};

			_mapRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync((Map)null);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo($"Map for execution ID {executionId} not found."));
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
		}

		[Test]
		public async Task Handle_NonExistentRobot_ReturnsValidationError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Clean, 10);
			var request = new ValidateCommandByMapQuery
			{
				ExecutionId = executionId,
				Command = command
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
			_mapRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(map);
			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync((Robot)null);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo($"Robot for execution ID {executionId} not found."));
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
		}

		[Test]
		public async Task Handle_InvalidCommandForMap_ReturnsBackOff()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Advance, 10);
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

			cells[0, 0] = new Cell(0, 0, CellType.Wall);
			var map = new Map(cells);
			var robot = new Robot(0, 0, Facing.North, 100);

			_mapRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(map);
			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);
			_commandRepositoryMock.Setup(repo => repo.UpdateFirstAsync(command, executionId)).ReturnsAsync(command);

			var request = new ValidateCommandByMapQuery
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.BackOff));
		}
	}
}
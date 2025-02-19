using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Robots
{
	[TestFixture]
	public class CreateRobotCommandHandlerTests
	{
		private Mock<IRepository<Robot>> _robotRepositoryMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private CreateRobotCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_robotRepositoryMock = new Mock<IRepository<Robot>>();
			_logAdapterMock = new Mock<ILogAdapter>();
			_handler = new CreateRobotCommandHandler(_robotRepositoryMock.Object, _logAdapterMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var position = new RobotPosition(1, 1, Facing.North);
			var battery = 100;
			var robot = new Robot(position, battery);

			_robotRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Robot>(), executionId)).ReturnsAsync(robot);

			var request = new CreateRobotCommand
			{
				ExecutionId = executionId,
				Position = position,
				Battery = battery
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.X, Is.EqualTo(position.X));
			Assert.That(result.Y, Is.EqualTo(position.Y));
			Assert.That(result.Facing, Is.EqualTo(position.Facing));
			Assert.That(result.Battery, Is.EqualTo(battery));
			Assert.That(result.IsCorrect, Is.True);
		}

		[Test]
		public void Handle_NullRequest_ThrowsArgumentNullException()
		{
			// Arrange
			CreateRobotCommand request = null;

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public void Handle_NullPosition_ThrowsArgumentNullException()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var request = new CreateRobotCommand
			{
				ExecutionId = executionId,
				Position = null,
				Battery = 100
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public void Handle_NegativeBattery_ThrowsArgumentException()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var position = new RobotPosition(1, 1, Facing.North);
			var request = new CreateRobotCommand
			{
				ExecutionId = executionId,
				Position = position,
				Battery = -100
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(request, CancellationToken.None));
		}
	}
}
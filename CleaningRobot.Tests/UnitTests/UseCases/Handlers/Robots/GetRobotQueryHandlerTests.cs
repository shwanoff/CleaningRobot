using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Robots
{
	[TestFixture]
	public class GetRobotQueryHandlerTests
	{
		private Mock<IRepository<Robot>> _robotRepositoryMock;
		private GetRobotQueryHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_robotRepositoryMock = new Mock<IRepository<Robot>>();
			_handler = new GetRobotQueryHandler(_robotRepositoryMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var robot = new Robot(1, 1, Facing.North, 100);

			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);

			var request = new GetRobotQuery
			{
				ExecutionId = executionId
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.X, Is.EqualTo(robot.Position.X));
			Assert.That(result.Y, Is.EqualTo(robot.Position.Y));
			Assert.That(result.Facing, Is.EqualTo(robot.Position.Facing));
			Assert.That(result.Battery, Is.EqualTo(robot.Battery));
			Assert.That(result.IsCorrect, Is.True);
		}
	}
}
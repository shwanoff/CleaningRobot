using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Robots
{
	[TestFixture]
	public class ConsumeEnergyCommandHandlerTests
	{
		private Mock<IRepository<Robot>> _robotRepositoryMock;
		private ConsumeEnergyCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_robotRepositoryMock = new Mock<IRepository<Robot>>();
			_handler = new ConsumeEnergyCommandHandler(_robotRepositoryMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var consumedEnergy = 10;
			var robot = new Robot(1, 1, Facing.North, 100);

			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);
			_robotRepositoryMock.Setup(repo => repo.UpdateAsync(robot, executionId)).ReturnsAsync(robot);

			var request = new ConsumeEnergyCommand
			{
				ExecutionId = executionId,
				ConsumedEnergy = consumedEnergy
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.IsCompleted, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
			Assert.That(result.Result.Battery, Is.EqualTo(90));
		}
	}
}
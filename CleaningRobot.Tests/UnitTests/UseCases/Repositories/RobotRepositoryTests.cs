using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Repositories;

namespace CleaningRobot.Tests.UnitTests.UseCases.Repositories
{
	[TestFixture]
	public class RobotRepositoryTests
	{
		private RobotRepository _repository;

		[SetUp]
		public void SetUp()
		{
			_repository = new RobotRepository();
		}

		[Test]
		public async Task AddAsync_AddsNewRobot()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var position = new RobotPosition(0, 0, Facing.North);
			var robot = new Robot(position, 100);

			// Act
			var result = await _repository.AddAsync(robot, executionId);

			// Assert
			Assert.That(result, Is.EqualTo(robot));
		}

		[Test]
		public async Task DeleteAsync_RemovesRobot()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var position = new RobotPosition(0, 0, Facing.North);
			var robot = new Robot(position, 100);
			await _repository.AddAsync(robot, executionId);

			// Act
			await _repository.DeleteAsync(executionId);

			// Assert
			Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.GetByIdAsync(executionId));
		}

		[Test]
		public async Task GetAllAsync_ReturnsAllRobots()
		{
			// Arrange
			var executionId1 = Guid.NewGuid();
			var executionId2 = Guid.NewGuid();
			var position1 = new RobotPosition(0, 0, Facing.North);
			var position2 = new RobotPosition(1, 1, Facing.East);
			var robot1 = new Robot(position1, 100);
			var robot2 = new Robot(position2, 100);
			await _repository.AddAsync(robot1, executionId1);
			await _repository.AddAsync(robot2, executionId2);

			// Act
			var result = await _repository.GetAllAsync();

			// Assert
			Assert.That(result, Has.Exactly(2).Items);
		}

		[Test]
		public async Task GetByIdAsync_ReturnsRobotById()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var position = new RobotPosition(0, 0, Facing.North);
			var robot = new Robot(position, 100);
			await _repository.AddAsync(robot, executionId);

			// Act
			var result = await _repository.GetByIdAsync(executionId);

			// Assert
			Assert.That(result, Is.EqualTo(robot));
		}

		[Test]
		public async Task UpdateAsync_UpdatesRobot()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var position = new RobotPosition(0, 0, Facing.North);
			var robot = new Robot(position, 100);
			await _repository.AddAsync(robot, executionId);
			var newPosition = new RobotPosition(1, 1, Facing.East);
			var newRobot = new Robot(newPosition, 80);

			// Act
			var result = await _repository.UpdateAsync(newRobot, executionId);

			// Assert
			Assert.That(result.Position, Is.EqualTo(newRobot.Position));
			Assert.That(result.Battery, Is.EqualTo(newRobot.Battery));
		}
	}
}
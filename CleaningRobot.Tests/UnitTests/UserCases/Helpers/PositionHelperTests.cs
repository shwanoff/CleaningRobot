using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Helpers;

namespace CleaningRobot.Tests.UnitTests.UserCases.Helpers
{
	[TestFixture]
	public class PositionHelperTests
	{
		[TestCase(Facing.North, CommandType.Advance, 0, -1)]
		[TestCase(Facing.East, CommandType.Advance, 1, 0)]
		[TestCase(Facing.South, CommandType.Advance, 0, 1)]
		[TestCase(Facing.West, CommandType.Advance, -1, 0)]
		[TestCase(Facing.North, CommandType.Back, 0, 1)]
		[TestCase(Facing.East, CommandType.Back, -1, 0)]
		[TestCase(Facing.South, CommandType.Back, 0, -1)]
		[TestCase(Facing.West, CommandType.Back, 1, 0)]
		public void GetNextPosition_ValidInputs_ReturnsExpectedPosition(Facing facing, CommandType commandType, int expectedX, int expectedY)
		{
			// Arrange
			var initialPosition = new RobotPosition(0, 0, facing);

			// Act
			var nextPosition = PositionHelper.GetNextPosition(initialPosition, commandType);

			// Assert
			Assert.That(nextPosition.X, Is.EqualTo(expectedX));
			Assert.That(nextPosition.Y, Is.EqualTo(expectedY));
		}
	}
}

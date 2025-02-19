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

		[Test]
		public void IsCellAvailable_PositionOutsideMap_ReturnsFalse()
		{
			// Arrange
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
			var position = new Position(-1, 0);

			// Act
			var result = PositionHelper.IsCellAvailable(map, position, out string? error);

			// Assert
			Assert.That(result, Is.False);
			Assert.That(error, Is.EqualTo("The robot cannot be outside the map"));
		}

		[Test]
		public void IsCellAvailable_PositionOnWall_ReturnsFalse()
		{
			// Arrange
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
			var position = new Position(1, 1);

			// Act
			var result = PositionHelper.IsCellAvailable(map, position, out string? error);

			// Assert
			Assert.That(result, Is.False);
			Assert.That(error, Is.EqualTo("The robot cannot be on a wall"));
		}

		[Test]
		public void IsCellAvailable_PositionOnColumn_ReturnsFalse()
		{
			// Arrange
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
			cells[1, 1] = new Cell(1, 1, CellType.Column);
			var map = new Map(cells);
			var position = new Position(1, 1);

			// Act
			var result = PositionHelper.IsCellAvailable(map, position, out string? error);

			// Assert
			Assert.That(result, Is.False);
			Assert.That(error, Is.EqualTo("The robot cannot be on a column"));
		}

		[Test]
		public void IsCellAvailable_PositionOnCleanableSpace_ReturnsTrue()
		{
			// Arrange
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
			var position = new Position(1, 1);

			// Act
			var result = PositionHelper.IsCellAvailable(map, position, out string? error);

			// Assert
			Assert.That(result, Is.True);
			Assert.That(error, Is.Null);
		}
	}
}

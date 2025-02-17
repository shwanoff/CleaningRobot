using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Extensions;

namespace CleaningRobot.Tests.UnitTests.Entities.Extensions
{
	[TestFixture]
	public class CellExtensionsTests
	{
		[TestCase("S", CellType.CleanableSpace)]
		[TestCase("C", CellType.Column)]
		[TestCase("W", CellType.Wall)]
		[TestCase("CleanableSpace", CellType.CleanableSpace)]
		[TestCase("Column", CellType.Column)]
		[TestCase("Wall", CellType.Wall)]
		[TestCase("", CellType.Wall)]
		[TestCase("null", CellType.Wall)]
		[TestCase(" ", CellType.Wall)]
		[TestCase("s", CellType.CleanableSpace)]
		[TestCase("c", CellType.Column)]
		[TestCase("w", CellType.Wall)]
		[TestCase("cleanable space", CellType.CleanableSpace)]
		[TestCase("column", CellType.Column)]
		[TestCase("wall", CellType.Wall)]
		public void ToCell_String_ValidInput_ReturnsExpectedCell(string input, CellType expected)
		{
			// Act
			var result = input.ToCellType();

			// Assert
			Assert.That(result, Is.EqualTo(expected));
		}

		[TestCase("X")]
		[TestCase("Invalid")]
		public void ToCell_String_InvalidInput_ThrowsArgumentException(string input)
		{
			// Act & Assert
			Assert.Throws<ArgumentException>(() => input.ToCellType());
		}

		[TestCase("N", CellState.NotVisited)]
		[TestCase("V", CellState.Visited)]
		[TestCase("C", CellState.Cleaned)]
		[TestCase("NotVisited", CellState.NotVisited)]
		[TestCase("Visited", CellState.Visited)]
		[TestCase("Cleaned", CellState.Cleaned)]
		[TestCase("n", CellState.NotVisited)]
		[TestCase("v", CellState.Visited)]
		[TestCase("c", CellState.Cleaned)]
		[TestCase("notvisited", CellState.NotVisited)]
		[TestCase("visited", CellState.Visited)]
		[TestCase("cleaned", CellState.Cleaned)]
		public void ToCellState_String_ValidInput_ReturnsExpectedCellState(string input, CellState expected)
		{
			// Act
			var result = input.ToCellState();

			// Assert
			Assert.That(result, Is.EqualTo(expected));
		}

		[TestCase("X")]
		[TestCase("Invalid")]
		public void ToCellState_String_InvalidInput_ThrowsArgumentException(string input)
		{
			// Act & Assert
			Assert.Throws<ArgumentException>(() => input.ToCellState());
		}
	}
}

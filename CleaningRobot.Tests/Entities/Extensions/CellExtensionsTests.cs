using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Extensions;

namespace CleaningRobot.Tests.Entities.Extensions
{
	public class CellExtensionsTests
	{
		[TestCase("S", Cell.CleanableSpace)]
		[TestCase("C", Cell.Column)]
		[TestCase("W", Cell.Wall)]
		[TestCase("CleanableSpace", Cell.CleanableSpace)]
		[TestCase("Column", Cell.Column)]
		[TestCase("Wall", Cell.Wall)]
		[TestCase("", Cell.Wall)]
		[TestCase("null", Cell.Wall)]
		[TestCase(" ", Cell.Wall)]
		[TestCase("s", Cell.CleanableSpace)]
		[TestCase("c", Cell.Column)]
		[TestCase("w", Cell.Wall)]
		[TestCase("cleanable space", Cell.CleanableSpace)]
		[TestCase("column", Cell.Column)]
		[TestCase("wall", Cell.Wall)]
		public void ToCell_String_ValidInput_ReturnsExpectedCell(string input, Cell expected)
		{
			// Act
			var result = input.ToCell();

			// Assert
			Assert.That(result, Is.EqualTo(expected));
		}

		[TestCase("X")]
		[TestCase("Invalid")]
		public void ToCell_String_InvalidInput_ThrowsArgumentException(string input)
		{
			// Act & Assert
			Assert.Throws<ArgumentException>(() => input.ToCell());
		}
	}
}

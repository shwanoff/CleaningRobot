using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Helpers;

namespace CleaningRobot.Tests.UnitTests.UserCases.Helpers
{
	[TestFixture]
	public class MapHelperTests
    {
		[Test]
		public void ConvertToRectangularArray_ValidInput_ReturnsExpectedArray()
		{
			// Arrange
			string[][] jaggedArray =
			{
				["S", "C", "S"],
				["S", "S", "C"],
				["C", "S", "S"]
			};

			// Act
			Cell[,] result = MapHelper.ConvertToRectangularArray(jaggedArray);

			// Assert
			Assert.That(result.GetLength(0), Is.EqualTo(3));
			Assert.That(result.GetLength(1), Is.EqualTo(3));
			Assert.That(result[0, 0].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[0, 1].Type, Is.EqualTo(CellType.Column));
			Assert.That(result[0, 2].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[1, 0].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[1, 1].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[1, 2].Type, Is.EqualTo(CellType.Column));
			Assert.That(result[2, 0].Type, Is.EqualTo(CellType.Column));
			Assert.That(result[2, 1].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[2, 2].Type, Is.EqualTo(CellType.CleanableSpace));

			CheckCoordinates(result);
		}

		[Test]
		public void ConvertToRectangularArray_CaseInsensitive_ReturnsExpectedArray()
		{
			// Arrange
			string[][] jaggedArray = 
			{
				["s", null, "S"],
				["S", "s", "NULL"],
				["Null", "S", "s"]
			};

			// Act
			Cell[,] result = MapHelper.ConvertToRectangularArray(jaggedArray);

			// Assert
			Assert.That(result.GetLength(0), Is.EqualTo(3));
			Assert.That(result.GetLength(1), Is.EqualTo(3));
			Assert.That(result[0, 0].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[0, 1].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[0, 2].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[1, 0].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[1, 1].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[1, 2].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[2, 0].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[2, 1].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[2, 2].Type, Is.EqualTo(CellType.CleanableSpace));

			CheckCoordinates(result);
		}

		[Test]
		public void ConvertToRectangularArray_EmptyRows_ReturnsExpectedArray()
		{
			// Arrange
			string[][] jaggedArray =
			{
				["S", "C"],
				[],
				["W"]
			};

			// Act
			Cell[,] result = MapHelper.ConvertToRectangularArray(jaggedArray);

			// Assert
			Assert.That(result.GetLength(0), Is.EqualTo(3));
			Assert.That(result.GetLength(1), Is.EqualTo(2));
			Assert.That(result[0, 0].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[0, 1].Type, Is.EqualTo(CellType.Column));
			Assert.That(result[1, 0].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[1, 1].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[2, 0].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[2, 1].Type, Is.EqualTo(CellType.Wall));

			CheckCoordinates(result);
		}

		[Test]
		public void ConvertToRectangularArray_InvalidCellType_ThrowsArgumentException()
		{
			// Arrange
			string[][] jaggedArray = 
			{
				["S", "X"]
			};

			// Act & Assert
			Assert.Throws<ArgumentException>(() => MapHelper.ConvertToRectangularArray(jaggedArray));
		}

		[Test]
		public void ConvertToRectangularArray_NullAndStringNullAndWhitespaceValuesInArray_TreatsAsWall()
		{
			// Arrange
			string[][] jaggedArray = 
			{
				["S", "null", "W", null, "S", "C"],
				["C", "S", "W", " ", "W", "S"],
				["W", "", "S", "C", "S", "W"],
				["null", "C", "W", "S", "C", "null"]
			};

			// Act
			Cell[,] result = MapHelper.ConvertToRectangularArray(jaggedArray);

			// Assert
			Assert.That(result.GetLength(0), Is.EqualTo(4));
			Assert.That(result.GetLength(1), Is.EqualTo(6));
			Assert.That(result[0, 0].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[0, 1].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[0, 2].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[0, 3].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[0, 4].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[0, 5].Type, Is.EqualTo(CellType.Column));
			Assert.That(result[1, 0].Type, Is.EqualTo(CellType.Column));
			Assert.That(result[1, 1].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[1, 2].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[1, 3].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[1, 4].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[1, 5].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[2, 0].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[2, 1].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[2, 2].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[2, 3].Type, Is.EqualTo(CellType.Column));
			Assert.That(result[2, 4].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[2, 5].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[3, 0].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[3, 1].Type, Is.EqualTo(CellType.Column));
			Assert.That(result[3, 2].Type, Is.EqualTo(CellType.Wall));
			Assert.That(result[3, 3].Type, Is.EqualTo(CellType.CleanableSpace));
			Assert.That(result[3, 4].Type, Is.EqualTo(CellType.Column));
			Assert.That(result[3, 5].Type, Is.EqualTo(CellType.Wall));

			CheckCoordinates(result);
		}

		private static void CheckCoordinates(Cell[,] result)
		{
			for (int y = 0; y < result.GetLength(0); y++)
			{
				for (int x = 0; x < result.GetLength(1); x++)
				{
					Assert.That(result[y, x].Position.X, Is.EqualTo(x));
					Assert.That(result[y, x].Position.Y, Is.EqualTo(y));
				}
			}
		}
	}
}

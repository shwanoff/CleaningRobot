using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Extensions;

namespace CleaningRobot.Tests.Entities.Extensions
{
	public class FacingExtensionsTests
	{
		[TestCase("N", Facing.North)]
		[TestCase("E", Facing.East)]
		[TestCase("S", Facing.South)]
		[TestCase("W", Facing.West)]
		[TestCase("North", Facing.North)]
		[TestCase("East", Facing.East)]
		[TestCase("South", Facing.South)]
		[TestCase("West", Facing.West)]
		[TestCase("n", Facing.North)]
		[TestCase("e", Facing.East)]
		[TestCase("s", Facing.South)]
		[TestCase("w", Facing.West)]
		[TestCase("north", Facing.North)]
		[TestCase("east", Facing.East)]
		[TestCase("south", Facing.South)]
		[TestCase("west", Facing.West)]
		public void ToFacing_String_ValidInput_ReturnsExpectedFacing(string input, Facing expected)
		{
			// Act
			var result = input.ToFacing();

			// Assert
			Assert.That(result, Is.EqualTo(expected));
		}

		[TestCase('N', Facing.North)]
		[TestCase('E', Facing.East)]
		[TestCase('S', Facing.South)]
		[TestCase('W', Facing.West)]
		public void ToFacing_Char_ValidInput_ReturnsExpectedFacing(char input, Facing expected)
		{
			// Act
			var result = input.ToFacing();

			// Assert
			Assert.That(result, Is.EqualTo(expected));
		}

		[TestCase("X")]
		[TestCase("Invalid")]
		public void ToFacing_String_InvalidInput_ThrowsArgumentException(string input)
		{
			// Act & Assert
			Assert.Throws<ArgumentException>(() => input.ToFacing());
		}

		[TestCase('X')]
		public void ToFacing_Char_InvalidInput_ThrowsArgumentException(char input)
		{
			// Act & Assert
			Assert.Throws<ArgumentException>(() => input.ToFacing());
		}
	}
}

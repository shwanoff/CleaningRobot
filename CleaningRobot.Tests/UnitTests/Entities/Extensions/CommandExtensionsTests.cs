using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Extensions;

namespace CleaningRobot.Tests.UnitTests.Entities.Extensions
{
	public class CommandExtensionsTests
	{
		[TestCase("TL", CommandType.TurnLeft)]
		[TestCase("TR", CommandType.TurnRight)]
		[TestCase("A", CommandType.Advance)]
		[TestCase("B", CommandType.Back)]
		[TestCase("C", CommandType.Clean)]
		[TestCase("TurnLeft", CommandType.TurnLeft)]
		[TestCase("Turn Right", CommandType.TurnRight)]
		[TestCase("Advance", CommandType.Advance)]
		[TestCase("Back", CommandType.Back)]
		[TestCase("Clean", CommandType.Clean)]
		[TestCase("tl", CommandType.TurnLeft)]
		[TestCase("tr", CommandType.TurnRight)]
		[TestCase("a", CommandType.Advance)]
		[TestCase("b", CommandType.Back)]
		[TestCase("c", CommandType.Clean)]
		[TestCase("turnleft", CommandType.TurnLeft)]
		[TestCase("turn right", CommandType.TurnRight)]
		[TestCase("advance", CommandType.Advance)]
		[TestCase("back", CommandType.Back)]
		[TestCase("clean", CommandType.Clean)]
		public void ToCommand_String_ValidInput_ReturnsExpectedCommand(string input, CommandType expected)
		{
			// Act
			var result = input.ToCommand();

			// Assert
			Assert.That(result, Is.EqualTo(expected));
		}

		[TestCase("X")]
		[TestCase("Invalid")]
		public void ToCommand_String_InvalidInput_ThrowsArgumentException(string input)
		{
			// Act & Assert
			Assert.Throws<ArgumentException>(() => input.ToCommand());
		}
	}
}

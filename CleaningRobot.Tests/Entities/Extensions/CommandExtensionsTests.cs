using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Extensions;

namespace CleaningRobot.Tests.Entities.Extensions
{
	public class CommandExtensionsTests
	{
		[TestCase("TL", Command.TurnLeft)]
		[TestCase("TR", Command.TurnRight)]
		[TestCase("A", Command.Advance)]
		[TestCase("B", Command.Back)]
		[TestCase("C", Command.Clean)]
		[TestCase("TurnLeft", Command.TurnLeft)]
		[TestCase("Turn Right", Command.TurnRight)]
		[TestCase("Advance", Command.Advance)]
		[TestCase("Back", Command.Back)]
		[TestCase("Clean", Command.Clean)]
		[TestCase("tl", Command.TurnLeft)]
		[TestCase("tr", Command.TurnRight)]
		[TestCase("a", Command.Advance)]
		[TestCase("b", Command.Back)]
		[TestCase("c", Command.Clean)]
		[TestCase("turnleft", Command.TurnLeft)]
		[TestCase("turn right", Command.TurnRight)]
		[TestCase("advance", Command.Advance)]
		[TestCase("back", Command.Back)]
		[TestCase("clean", Command.Clean)]
		public void ToCommand_String_ValidInput_ReturnsExpectedCommand(string input, Command expected)
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

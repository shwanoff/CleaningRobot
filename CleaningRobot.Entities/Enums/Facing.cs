using System.ComponentModel;

namespace CleaningRobot.Entities.Enums
{
	/// <summary>
	/// Represents the facing of the robot.
	/// </summary>
	public enum Facing
	{
		[Description("N")]
		North,
		[Description("E")]
		East,
		[Description("S")]
		South,
		[Description("W")]
		West
	}
}
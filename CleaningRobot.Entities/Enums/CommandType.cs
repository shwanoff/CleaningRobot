using System.ComponentModel;

namespace CleaningRobot.Entities.Enums
{
	/// <summary>
	/// Represents commands that the robot can execute
	/// </summary>
	public enum CommandType
    {
		[Description("TL")]
		TurnLeft,
		[Description("TR")]
		TurnRight,
		[Description("A")]
		Advance,
		[Description("B")]
		Back,
		[Description("C")]
		Clean
	}
}

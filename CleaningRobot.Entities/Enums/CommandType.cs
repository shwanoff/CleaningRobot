using System.ComponentModel;

namespace CleaningRobot.Entities.Enums
{
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

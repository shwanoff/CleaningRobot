using System.ComponentModel;

namespace CleaningRobot.Entities.Enums
{
	/// <summary>
	/// Represents the type of a cell in the map.
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

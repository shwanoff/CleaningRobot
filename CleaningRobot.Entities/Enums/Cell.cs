using System.ComponentModel;

namespace CleaningRobot.Entities.Enums
{
    public enum Cell
    {
		[Description("S")]
		CleanableSpace,
		[Description("C")]
		Column,
		[Description("W")]
		Wall
	}
}

using System.ComponentModel;

namespace CleaningRobot.Entities.Enums
{
	public enum CellType
	{
		[Description("S")]
		CleanableSpace,
		[Description("C")]
		Column,
		[Description("W")]
		Wall
	}
}

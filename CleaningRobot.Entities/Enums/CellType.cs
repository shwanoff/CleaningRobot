using System.ComponentModel;

namespace CleaningRobot.Entities.Enums
{
	/// <summary>
	/// Represents the type of a cell in the map.
	/// </summary>
	public enum CellType
	{
		[Description("S")]
		CleanableSpace,
		[Description("C")]
		Column,
		[Description("W")] // same as null
		Wall
	}
}

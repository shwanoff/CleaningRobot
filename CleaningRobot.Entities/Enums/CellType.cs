using System.ComponentModel;

namespace CleaningRobot.Entities.Enums
{
	/// <summary>
	/// Represents the type of the cell
	/// </summary>
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

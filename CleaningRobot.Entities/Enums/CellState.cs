using System.ComponentModel;

namespace CleaningRobot.Entities.Enums
{
	/// <summary>
	/// Represents the state of the cell
	/// </summary>
	public enum CellState
    {
		[Description("N")]
		NotVisited,
		[Description("V")]
		Visited,
		[Description("C")]
		Cleaned
	}
}

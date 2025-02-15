using System.ComponentModel;

namespace CleaningRobot.Entities.Enums
{
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

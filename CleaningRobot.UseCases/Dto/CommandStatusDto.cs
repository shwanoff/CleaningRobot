using CleaningRobot.Entities.Enums;

namespace CleaningRobot.UseCases.Dto
{
    public class CommandStatusDto
	{
		public CommandType CommandType { get; set; }

		public int ConsumedEnergy { get; set; }
	}
}

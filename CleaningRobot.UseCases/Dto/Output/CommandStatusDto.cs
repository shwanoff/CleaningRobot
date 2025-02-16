using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
	public class CommandStatusDto : StatusDtoBase
	{
		public CommandType Type { get; set; }

		public int EnergyConsumption { get; set; }

		public bool IsCompleted { get; set; }
	}
}

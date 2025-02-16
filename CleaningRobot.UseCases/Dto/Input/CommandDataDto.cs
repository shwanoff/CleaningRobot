using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Input
{
    public class CommandDataDto : DataDtoBase
    {
		public required IEnumerable<string> Commands { get; set; } 
		public required IDictionary<string, int> EnergyConsumptions { get; set; }

	}
}

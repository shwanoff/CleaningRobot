using CleaningRobot.Entities.Enums;

namespace CleaningRobot.UseCases.Dto
{
    public class CommandStatusDto(CommandType commandType, uint consumedEnergy)
	{
		public CommandType CommandType { get; set; } = commandType;

		public uint ConsumedEnergy { get; set; } = consumedEnergy;

		override public string ToString()
		{
			return $"CommandType: {CommandType}, ConsumedEnergy: {ConsumedEnergy}";
		}
	}
}

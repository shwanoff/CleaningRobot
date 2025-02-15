using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	public class Command(CommandType commandType, int consumedEnergy)
	{
		public CommandType CommandType { get; set; } = commandType;
		public int ConsumedEnergy { get; set; } = consumedEnergy;

		public override string ToString()
		{
			return $"{CommandType} Consumed energy: {ConsumedEnergy}";
		}
	}
}

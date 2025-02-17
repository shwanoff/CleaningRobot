using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	public class Command(CommandType commandType, int consumedEnergy) : EntityBase
	{
		public CommandType Type { get; set; } = commandType;
		public int EnergyConsumption { get; set; } = consumedEnergy;

		
		public bool IsValidatedByCommand { get; set; } = false;
		public bool IsValidatedByMap { get; set; } = false;
		public bool IsValidatedByRobot { get; set; } = false;
		public bool IsValid => IsValidatedByCommand && IsValidatedByMap && IsValidatedByRobot;

		public bool IsCompletedByCommand { get; set; } = false;
		public bool IsCompletedByMap { get; set; } = false;
		public bool IsCompletedByRobot { get; set; } = false;
		public bool IsCompleted => IsCompletedByCommand && IsCompletedByMap && IsCompletedByRobot;



		public override string ToString()
		{
			return $"{Type} {EnergyConsumption}";
		}
	}
}

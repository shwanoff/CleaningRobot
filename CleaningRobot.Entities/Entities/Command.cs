using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	/// <summary>
	/// Represents a command that the robot can perform
	/// </summary>
	/// <param name="commandType"> Operation that the robot can perform </param>
	/// <param name="consumedEnergy"> How much energy is consumed by the command </param>
	public class Command(CommandType commandType, uint consumedEnergy)
	{
		/// <summary>
		/// Operation that the robot can perform
		/// </summary>
		public CommandType CommandType { get; set; } = commandType;

		/// <summary>
		/// How much energy is consumed by the command
		/// </summary>
		public uint ConsumedEnergy { get; set; } = consumedEnergy;

		public override string ToString()
		{
			return $"{CommandType} Consumed energy: {ConsumedEnergy}";
		}
	}
}

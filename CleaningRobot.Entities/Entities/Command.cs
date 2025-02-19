using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	/// <summary>
	/// Represents a command that the cleaning robot can execute.
	/// </summary>
	public class Command : EntityBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> class with the specified command type and energy consumption.
		/// </summary>
		/// <param name="commandType">The type of the command.</param>
		/// <param name="consumedEnergy">The energy consumption of the command.</param>
		public Command(CommandType commandType, int consumedEnergy)
		{
			Type = commandType;
			EnergyConsumption = consumedEnergy;
		}

		/// <summary>
		/// Gets or sets the type of the command.
		/// </summary>
		public CommandType Type { get; set; }

		/// <summary>
		/// Gets or sets the energy consumption of the command.
		/// </summary>
		public int EnergyConsumption { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the command is validated by the command itself.
		/// </summary>
		public bool IsValidatedByCommand { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the command is validated by the map.
		/// </summary>
		public bool IsValidatedByMap { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the command is validated by the robot.
		/// </summary>
		public bool IsValidatedByRobot { get; set; } = false;

		/// <summary>
		/// Gets a value indicating whether the command is valid.
		/// </summary>
		public bool IsValid => IsValidatedByCommand && IsValidatedByMap && IsValidatedByRobot;

		/// <summary>
		/// Gets or sets a value indicating whether the command is completed by the command itself.
		/// </summary>
		public bool IsCompletedByCommand { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the command is completed by the map.
		/// </summary>
		public bool IsCompletedByMap { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the command is completed by the robot.
		/// </summary>
		public bool IsCompletedByRobot { get; set; } = false;

		/// <summary>
		/// Gets a value indicating whether the command is completed.
		/// </summary>
		public bool IsCompleted => IsCompletedByCommand && IsCompletedByMap && IsCompletedByRobot;

		/// <summary>
		/// Returns a string that represents the current command.
		/// </summary>
		/// <returns>A string that represents the current command.</returns>
		public override string ToString()
		{
			return $"{Type} {EnergyConsumption}";
		}
	}
}
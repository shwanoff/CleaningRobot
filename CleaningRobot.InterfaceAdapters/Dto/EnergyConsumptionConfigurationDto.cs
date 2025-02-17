namespace CleaningRobot.InterfaceAdapters.Dto
{
    public class EnergyConsumptionConfigurationDto
    {
		public CommandConsumption? TurnLeft { get; set; }
		public CommandConsumption? TurnRight { get; set; }
		public CommandConsumption? Advance { get; set; }
		public CommandConsumption? Back { get; set; }
		public CommandConsumption? Clean { get; set; }
	}

	public class CommandConsumption
	{
		public required string Command { get; set; }
		public required int EnergyConsumption { get; set; }
	}
}

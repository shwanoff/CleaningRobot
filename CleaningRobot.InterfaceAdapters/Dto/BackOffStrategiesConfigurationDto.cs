namespace CleaningRobot.InterfaceAdapters.Dto
{
    public class BackOffStrategiesConfigurationDto
    {
		public required List<List<string>> Sequences { get; set; }
		public required bool ConsumeEnergyWhenBackOff { get; set; }
		public required bool StopWhenBackOff { get; set; }
	}
}

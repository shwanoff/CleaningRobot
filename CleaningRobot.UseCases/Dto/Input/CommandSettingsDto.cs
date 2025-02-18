namespace CleaningRobot.UseCases.Dto.Input
{
    public class CommandSettingsDto 
    {
		public required bool StopWhenBackOff { get; set; }
		public required bool ConsumeEnergyWhenBackOff { get; set; }
	}
}

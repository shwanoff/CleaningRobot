using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Input
{
	public class CommandDataDto : DataDtoBase
	{
		public required IEnumerable<string> Commands { get; set; }
		public required IDictionary<string, int> EnergyConsumptions { get; set; }
		public required IEnumerable<IEnumerable<string>> BackoffStrategy { get; set; }
		public required bool StopWhenBackOff { get; set; }
		public required bool ConsumeEnergyWhenBackOff { get; set; }

		#if DEBUG
		public override string ToString()
		{
			return $"Commands: {string.Join(", ", Commands)}";
		}
		#endif
	}
}

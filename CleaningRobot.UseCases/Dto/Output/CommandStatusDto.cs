using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
	public class CommandStatusDto : StatusDtoBase
	{
		public required CommandType Type { get; set; }
		public required int EnergyConsumption { get; set; }

		public required bool IsValid { get; set; }
		public required bool IsCompleted { get; set; }

#if DEBUG
		public override string ToString()
		{
			var success = IsCorrect ? "Success" : "Failed";
			return $"[{success}] {Type} {EnergyConsumption}";
		}
		#endif
	}
}

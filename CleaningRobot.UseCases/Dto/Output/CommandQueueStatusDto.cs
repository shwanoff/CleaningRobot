using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
    public class CommandQueueStatusDto : StatusDtoBase
	{
        public required IReadOnlyList<CommandStatusDto> Commands { get; set; }

		#if DEBUG
		public override string ToString()
		{
			var success = IsCorrect ? "Success" : "Failed";
			return $"[{success}] {string.Join(", ", Commands)}";
		}
		#endif
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
    public class ExecutionResultStatusDto<T> : ResultStatusDto where T : StatusDtoBase
	{
		public required bool IsCompleted { get; set; }
		public required T Result { get; set; }

		#if DEBUG
		public override string ToString()
		{
			var success = IsCorrect ? "Success" : "Failed";
			return $"[{success}] {Result}";
		}
		#endif
	}
}

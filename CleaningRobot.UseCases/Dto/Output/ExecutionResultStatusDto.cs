using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
    public class ExecutionResultStatusDto<T> : StatusDtoBase where T : EntityBase
	{
		public required T Result { get; set; }
		public bool IsCompleted { get; set; }
	}
}

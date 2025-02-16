namespace CleaningRobot.UseCases.Dto.Base
{
    public class StatusDto : StatusDtoBase
	{
		public required bool IsCompleted { get; set; }
		public string? Error { get; set; }
	}
}

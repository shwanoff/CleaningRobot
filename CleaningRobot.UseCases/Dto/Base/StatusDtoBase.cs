namespace CleaningRobot.UseCases.Dto.Base
{
    public abstract class StatusDtoBase : DtoBase
	{
		public required bool IsCorrect { get; set; }
		public string? Error { get; set; }
	}
}

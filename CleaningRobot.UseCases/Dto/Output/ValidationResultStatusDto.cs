namespace CleaningRobot.UseCases.Dto.Output
{
    public class ValidationResultStatusDto : ResultStatusDto
	{
        public required bool IsValid { get; set; }
	}
}

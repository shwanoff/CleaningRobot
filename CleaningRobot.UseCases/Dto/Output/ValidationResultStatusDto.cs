using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
    public class ValidationResultStatusDto : StatusDtoBase
	{
        public bool IsValid { get; set; }
		public string? Error { get; set; }
	}
}

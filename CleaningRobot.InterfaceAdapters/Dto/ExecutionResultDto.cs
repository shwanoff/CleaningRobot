namespace CleaningRobot.InterfaceAdapters.Dto
{
	public class ExecutionResultDto
	{
		public Guid ExecutionId { get; set; }
		public bool Success { get; set; }
		public string? Result { get; set; }
		public Exception? Error { get; set; }

		public ExecutionResultDto(string result, Guid executionId)
		{
			ExecutionId = executionId;
			Success = true;
			Result = result;
			Error = null;
		}

		public ExecutionResultDto(Exception error, Guid executionId)
		{
			ExecutionId = executionId;
			Success = false;
			Result = null;
			Error = error;
		}
	}
}

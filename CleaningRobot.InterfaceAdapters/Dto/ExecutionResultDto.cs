namespace CleaningRobot.InterfaceAdapters.Dto
{
	public class ExecutionResultDto
    {
		public bool Success { get; set; }
		public string? Result { get; set; }
		public Exception? Error { get; set; }

		public ExecutionResultDto(string result) 
		{
			Success = true;
			Result = result;
			Error = null;
		}

		public ExecutionResultDto(Exception error)
		{
			Success = false;
			Result = null;
			Error = error;
		}

		public override string ToString()
		{
			return Success ? Result ?? "Success" : Error?.Message ?? "Unknown error";
		}
	}
}

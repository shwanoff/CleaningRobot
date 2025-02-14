namespace CleaningRobot.Entities.Entities
{
    public class ExecutionResult
    {
		public bool Success { get; set; }
		public string? Result { get; set; }
		public Exception? Error { get; set; }
		

		public ExecutionResult(string result) 
		{
			Success = true;
			Result = result;
			Error = null;
		}

		public ExecutionResult(Exception error)
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

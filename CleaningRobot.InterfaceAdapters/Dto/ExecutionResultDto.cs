namespace CleaningRobot.Entities.Entities
{
	/// <summary>
	/// Represents the result of the execution of commands by the robot
	/// </summary>
	public class ExecutionResultDto
    {
		/// <summary>
		/// Indicates whether the execution was successful
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// Result of the execution
		/// </summary>
		public string? Result { get; set; }

		/// <summary>
		/// Error that occurred during execution
		/// </summary>
		public Exception? Error { get; set; }

		/// <summary>
		/// Initializes a new instance of the ExecutionResultDto class with successful result
		/// </summary>
		/// <param name="result"> Serealized output results </param>
		public ExecutionResultDto(string result) 
		{
			Success = true;
			Result = result;
			Error = null;
		}

		/// <summary>
		/// Initializes a new instance of the ExecutionResultDto class with error
		/// </summary>
		/// <param name="error"> Error </param>
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

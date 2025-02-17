namespace CleaningRobot.UseCases.Enums
{
    public enum ResultState
    {
		Ok,
		QueueIsEmpty,
		BackOff,
		OutOfEnergy,
		ValidationError,
		ExecutionError,
		Error
	}
}

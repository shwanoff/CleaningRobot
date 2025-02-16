namespace CleaningRobot.UseCases.Interfaces
{
    public interface IQueueRepository<T> : IRepository<Queue<T>> where T : class
	{
		Task<T> PeekFirstAsync(Guid executionId);
		Task<IEnumerable<T>> PeekAllAsync(Guid executionId);
		Task PushAsync(Guid executionId, T entity);
		Task UpdateFirstAsync(Guid executionId, T entity);
		Task<T> PullAsync(Guid executionId);
	}
}

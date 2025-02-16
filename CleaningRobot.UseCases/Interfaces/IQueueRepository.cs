namespace CleaningRobot.UseCases.Interfaces
{
    public interface IQueueRepository<T> : IRepository<Queue<T>> where T : class
	{
		Task<T> PeekAsync(Guid executionId);
		Task<IEnumerable<T>> ReadAllAsync(Guid executionId);
		Task PushAsync(Guid executionId, T entity);
		Task UpdateFirstAsync(Guid executionId, T entity);
		Task<T?> PullAsync(Guid executionId);
	}
}

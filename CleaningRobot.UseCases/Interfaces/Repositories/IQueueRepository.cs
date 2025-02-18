namespace CleaningRobot.UseCases.Interfaces.Repositories
{
    public interface IQueueRepository<T> : IRepository<Queue<T>> where T : class
	{
		Task<T?> PeekAsync(Guid executionId);
		Task<IEnumerable<T>> ReadAllAsync(Guid executionId);
		Task PushAsync(T entity, Guid executionId);
		Task<T> UpdateFirstAsync(T entity, Guid executionId);
		Task<T?> PullAsync(Guid executionId);
	}
}

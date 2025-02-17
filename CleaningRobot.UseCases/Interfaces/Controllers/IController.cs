using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
    public interface IController<in TInput, TOutput>
		where TInput : DataDtoBase
		where TOutput : StatusDtoBase
	{
		Task<TOutput> CreateAsync(TInput data, Guid executionId);
		Task<TOutput> GetAsync(Guid executionId);
	}
}

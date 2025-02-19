using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
	public class GetCommandQueueQuery : IRequest<CommandCollectionStatusDto>
	{
		public required Guid ExecutionId { get; set; }
	}

	public class GetCommandQueueQueryHandler(IRepository<Queue<Command>> commandRepository, ILogAdapter logAdapter) : IRequestHandler<GetCommandQueueQuery, CommandCollectionStatusDto>
	{
		private readonly IRepository<Queue<Command>> _commandRepository = commandRepository;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<CommandCollectionStatusDto> Handle(GetCommandQueueQuery request, CancellationToken cancellationToken = default)
		{
			try
			{
				request.NotNull();

				var commandQueue = await _commandRepository
					.GetByIdAsync(request.ExecutionId)
					.NotNull();

				await _logAdapter.TraceAsync($"Command queue state {commandQueue}. Commands: {string.Join(',', commandQueue)}", request.ExecutionId);

				return new CommandCollectionStatusDto
				{
					IsCorrect = true,
					ExecutionId = request.ExecutionId,
					Commands = [.. commandQueue.Select(command => new CommandStatusDto
				{
					Type = command.Type,
					EnergyConsumption = command.EnergyConsumption,
					IsValid = command.IsValid,
					IsCompleted = command.IsCompleted,
					IsCorrect = true,
					ExecutionId = request.ExecutionId
				})]
				};
			}
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(GetCommandQueueQueryHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
		}
	}
}

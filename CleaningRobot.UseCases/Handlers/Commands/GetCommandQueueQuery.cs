using CleaningRobot.Entities.Entities;
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

	public class GetCommandQueueQueryHandler(IRepository<Queue<Command>> commandRepository) : IRequestHandler<GetCommandQueueQuery, CommandCollectionStatusDto>
	{
		private readonly IRepository<Queue<Command>> _commandRepository = commandRepository;

		public async Task<CommandCollectionStatusDto> Handle(GetCommandQueueQuery request, CancellationToken cancellationToken = default)
		{
			request.NotNull();

			var commandQueue = await _commandRepository
				.GetByIdAsync(request.ExecutionId)
				.NotNull();

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
	}
}

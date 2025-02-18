using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
    public class SetupBackoffStrategyCommand : IRequest<ResultStatusDto>
	{
		public required Guid ExecutionId { get; set; }

		public required IEnumerable<IEnumerable<CommandType>> BackoffCommands { get; set; }
		public required IDictionary<CommandType, int> EnergyConsumptions { get; set; }
	}

	public class SetupBackoffStrategyCommandHandler(IBackoffRepository backoffRepository) : IRequestHandler<SetupBackoffStrategyCommand, ResultStatusDto>
	{
		private readonly IBackoffRepository _backoffRepository = backoffRepository;

		public async Task<ResultStatusDto> Handle(SetupBackoffStrategyCommand request, CancellationToken cancellationToken)
		{
			request.NotNull();
			request.BackoffCommands.NotNull();
			request.BackoffCommands.HasItems();
			request.EnergyConsumptions.NotNull();
			request.EnergyConsumptions.HasItems();

			var resultQueueOfQueues = Setup(request);

			var result = await _backoffRepository.Initialize(resultQueueOfQueues, request.ExecutionId);

			request.NotNull();
			result.HasItems();

			return new ResultStatusDto
			{
				ExecutionId = request.ExecutionId,
				IsCorrect = true,
				State = ResultState.Ok
			};
		}

		private static Queue<Queue<Command>> Setup(SetupBackoffStrategyCommand request)
		{
			var resultQueueOfQueues = new Queue<Queue<Command>>();

			foreach (var queues in request.BackoffCommands)
			{
				queues.NotNull();
				queues.HasItems();

				var queue = new Queue<Command>();

				foreach (var commandType in queues)
				{
					var command = new Command(commandType, request.EnergyConsumptions[commandType]);
					queue.Enqueue(command);
				}

				resultQueueOfQueues.Enqueue(queue);
			}

			return resultQueueOfQueues;
		}
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
    public class SetupBackoffStrategyCommand : IRequest<CommandCollectionStatusDto>
	{
		public required Guid ExecutionId { get; set; }
		public required IEnumerable<IEnumerable<CommandType>> BackoffCommands { get; set; }
		public required IDictionary<CommandType, int> EnergyConsumptions { get; set; }
		public required CommandSettingsDto CommandSettings { get; set; }
	}

	public class SetupBackoffStrategyCommandHandler(IBackoffRepository backoffRepository) : IRequestHandler<SetupBackoffStrategyCommand, CommandCollectionStatusDto>
	{
		private readonly IBackoffRepository _backoffRepository = backoffRepository;

		public async Task<CommandCollectionStatusDto> Handle(SetupBackoffStrategyCommand request, CancellationToken cancellationToken)
		{
			request.NotNull();
			request.BackoffCommands.NotNull();
			request.BackoffCommands.HasItems();
			request.EnergyConsumptions.NotNull();
			request.EnergyConsumptions.HasItems();

			var resultQueueOfQueues = Setup(request);

			var result = await _backoffRepository.Initialize(resultQueueOfQueues, request.ExecutionId);

			result.NotNull();
			result.HasItems();

			_backoffRepository.Settings = request.CommandSettings;

			var allCommands = result.SelectMany(queue => queue).ToList();

			return new CommandCollectionStatusDto
			{
				ExecutionId = request.ExecutionId,
				IsCorrect = true,
				Commands = [.. allCommands.Select(c => new CommandStatusDto
				{
					Type = c.Type,
					EnergyConsumption = c.EnergyConsumption,
					IsCorrect = true,
					IsValid = c.IsValid,
					IsCompleted = c.IsCompleted,
					ExecutionId = request.ExecutionId
				})]
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

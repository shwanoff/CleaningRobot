using CleaningRobot.Entities.Extensions;
using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Handlers.Commands;
using CleaningRobot.UseCases.Interfaces.Controllers;
using MediatR;

namespace CleaningRobot.UseCases.Controllers
{
	public class CommandContoller(IMediator mediator) : ICommandController
	{
		private readonly IMediator _mediator = mediator;

		public async Task<CommandCollectionStatusDto> CreateAsync(CommandDataDto data, Guid executionId)
		{
			var commandSetupResult = await SetupCommandQueue(data, executionId);

			if (!commandSetupResult.IsCorrect)
			{
				return new CommandCollectionStatusDto
				{
					IsCorrect = false,
					Error = commandSetupResult.Error,
					Commands = commandSetupResult.Commands,
					ExecutionId = executionId
				};
			}

			var backoffSetupResult = await SetupBackoffStrategy(data, executionId);

			if (!backoffSetupResult.IsCorrect)
			{
				return new CommandCollectionStatusDto
				{
					IsCorrect = false,
					Error = backoffSetupResult.Error,
					Commands = backoffSetupResult.Commands,
					ExecutionId = executionId
				};
			}

			return commandSetupResult;
		}

		public async Task<string> ExcecuteAllAsync(Guid executionId)
		{
			var command = new StartCommand
			{
				ExecutionId = executionId
			};

			var result = await _mediator.Send(command);

			if (!result.IsCorrect)
			{
				return $"Error: {result.Error}";
			}
			else
			{
				return $"Execution ID {executionId} completed successfully";
			}
		}

		public async Task<CommandCollectionStatusDto> GetAsync(Guid executionId)
		{
			var query = new GetCommandQueueQuery
			{
				ExecutionId = executionId
			};

			return await _mediator.Send(query);
		}

		private async Task<CommandCollectionStatusDto> SetupCommandQueue(CommandDataDto data, Guid executionId)
		{
			var command = new CreateCommandQueueCommand
			{
				ExecutionId = executionId,
				Commands = data.Commands.Select(x => x.ToCommand()),
				EnergyConsumptions = data.EnergyConsumptions.ToDictionary(x => x.Key.ToCommand(), x => x.Value)
			};

			return await _mediator.Send(command);
		}

		private async Task<CommandCollectionStatusDto> SetupBackoffStrategy(CommandDataDto data, Guid executionId)
		{
			var command = new SetupBackoffStrategyCommand
			{
				ExecutionId = executionId,
				BackoffCommands = data.BackoffStrategy.Select(x => x.Select(y => y.ToCommand()).ToList()).ToList(),
				EnergyConsumptions = data.EnergyConsumptions.ToDictionary(x => x.Key.ToCommand(), x => x.Value),
				CommandSettings = new CommandSettingsDto
				{
					ConsumeEnergyWhenBackOff = data.ConsumeEnergyWhenBackOff,
					StopWhenBackOff = data.StopWhenBackOff
				}
			};

			return await _mediator.Send(command);
		}
	}
}

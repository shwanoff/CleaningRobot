using CleaningRobot.Entities.Entities;
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

		public async Task<CommandQueueStatusDto> CreateAsync(CommandDataDto data, Guid executionId)
		{
			var command = new CreateCommandQueueCommand
			{
				ExecutionId = executionId,
				Commands = data.Commands,
				EnergyConsumptions = data.EnergyConsumptions.ToDictionary(x => x.Key.ToCommand(), x => x.Value)
			};

			return await _mediator.Send(command);
		}

		public async Task ExcecuteAllAsync(Guid executionId)
		{
			ExecutionResultStatusDto<Command> result;

			do
			{
				var command = new ExecuteNextCommandFromQueueCommand
				{
					ExecutionId = executionId
				};

				result = await _mediator.Send(command);
			}
			while (result != null);
		}

		public async Task<CommandQueueStatusDto> GetAsync(Guid executionId)
		{
			var query = new GetCommandQueueQuery
			{
				ExecutionId = executionId
			};

			return await _mediator.Send(query);
		}
	}
}

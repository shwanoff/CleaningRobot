using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Base;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
	public class ExecuteNextCommandFromQueueCommand : IRequest<ResultStatusDto>
	{
		public Guid ExecutionId { get; set; }
	}

	public class ExecuteNextCommandFromQueueCommandHandler(IQueueRepository<Command> commandRepository, IMediator mediator) : IRequestHandler<ExecuteNextCommandFromQueueCommand, ResultStatusDto>
	{
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;
		private readonly IMediator _mediator = mediator;

		public async Task<ResultStatusDto> Handle(ExecuteNextCommandFromQueueCommand request, CancellationToken cancellationToken)
		{
			request.NotNull();

			//TODO: Make transactional

			var command = await _commandRepository
				.PeekAsync(request.ExecutionId)
				.NotNull();

			var executionResult = await ExecuteNext(command, request.ExecutionId);

			if (!executionResult.IsCorrect)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = executionResult.Error,
					State = executionResult.State
				};
			}


			var result = await _commandRepository.PullAsync(request.ExecutionId);

			if (result == null)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = "Command cannot be pulled from the queue",
					State = ResultState.QueueIsEmpty
				};
			}

			if (!result.IsValid)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = "Validation of the command failed at the end of operation",
					State = ResultState.Error
				};
			}

			if (!result.IsCompleted)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = "Execution of the command failed at the end of operation",
					State = ResultState.Error
				};
			}

			return new ExecutionResultStatusDto<CommandStatusDto>
			{
				Result = new CommandStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = true,
					IsCompleted = result.IsCompleted,
					IsValid = result.IsValid,
					Type = result.Type,
					EnergyConsumption = result.EnergyConsumption
				},
				ExecutionId = request.ExecutionId,
				IsCorrect = true,
				IsCompleted = true,
				State = ResultState.Ok
			};
		}

		private async Task<ResultStatusDto> ExecuteNext(Command command, Guid executionId)
		{
			var executeCommand = new ExecuteNextCommand
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = false
			};

			return await _mediator.Send(executeCommand);
		}
	}
}

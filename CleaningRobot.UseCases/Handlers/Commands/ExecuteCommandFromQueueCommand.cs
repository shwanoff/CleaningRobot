using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
	public class ExecuteCommandFromQueueCommand : IRequest<ResultStatusDto>
	{
		public Guid ExecutionId { get; set; }
	}

	public class ExecuteCommandFromQueueCommandHandler(IQueueRepository<Command> commandRepository, IMediator mediator, ILogAdapter logAdapter) : IRequestHandler<ExecuteCommandFromQueueCommand, ResultStatusDto>
	{
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;
		private readonly IMediator _mediator = mediator;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<ResultStatusDto> Handle(ExecuteCommandFromQueueCommand request, CancellationToken cancellationToken)
		{
			try
			{
				request.NotNull();

				//TODO: Make transactional

				var command = await _commandRepository.PeekAsync(request.ExecutionId);

				if (command == null)
				{
					return new ResultStatusDto
					{
						ExecutionId = request.ExecutionId,
						IsCorrect = false,
						Error = "Command cannot be peeked from the queue",
						State = ResultState.QueueIsEmpty
					};
				}

				var executionResult = await ExecuteNext(command, request.ExecutionId);

				var result = await _commandRepository.PullAsync(request.ExecutionId);

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

				await _logAdapter.TraceAsync($"Command executed. Command state {result}", request.ExecutionId);

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
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(ExecuteCommandFromQueueCommandHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
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

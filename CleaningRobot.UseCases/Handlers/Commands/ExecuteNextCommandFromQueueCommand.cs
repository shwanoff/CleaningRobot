using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Base;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.UseCases.Interfaces;
using MediatR;
using System.Threading;

namespace CleaningRobot.UseCases.Handlers.Commands
{
	public class ExecuteNextCommandFromQueueCommand : IRequest<ExecutionResultStatusDto<Command>>
	{
		public Guid ExecutionId { get; set; }
	}

	public class ExecuteNextCommandFromQueueCommandHandler(IQueueRepository<Command> commandRepository, IMediator mediator) : IRequestHandler<ExecuteNextCommandFromQueueCommand, ExecutionResultStatusDto<Command>>
	{
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;
		private readonly IMediator _mediator = mediator;

		public async Task<ExecutionResultStatusDto<Command>?> Handle(ExecuteNextCommandFromQueueCommand request, CancellationToken cancellationToken)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request), "Request cannot be null");
			}

			//TODO: Make transactional

			var command = await _commandRepository.PeekAsync(request.ExecutionId);

			if (command != null)
			{
				var validationResult = await ValidateCommand(command, request.ExecutionId);

				if (!validationResult.IsCompleted)
				{
					return new ExecutionResultStatusDto<Command>
					{
						Result = command,
						ExecutionId = request.ExecutionId,
						IsCompleted = false,
						Error = validationResult.Error
					};
				}

				var executionResult = await ExecuteCommand<StatusDto>(command, request.ExecutionId);

				if (!executionResult.IsCompleted)
				{
					return new ExecutionResultStatusDto<Command>
					{
						Result = command,
						ExecutionId = request.ExecutionId,
						IsCompleted = false,
						Error = executionResult.Error
					};
				}

				var result = await _commandRepository.PullAsync(request.ExecutionId);

				if (result == null)
				{
					throw new InvalidOperationException("Command was not removed from the queue");
				}

				return new ExecutionResultStatusDto<Command>
				{
					Result = command,
					ExecutionId = request.ExecutionId,
					IsCompleted = true
				};
			}
			else
			{
				return null;
			}
		}

		private async Task<StatusDto> ExecuteCommand<T>(Command command, Guid executionId)
		{
			var commandExecution = ExecuteByCommand(command, executionId);
			var mapExecution = ExecuteByMap(command, executionId);
			var robotExecution = ExecuteByRobot(command, executionId);

			await Task.WhenAll(commandExecution, mapExecution, robotExecution);

			if (!commandExecution.Result.IsCompleted)
			{
				return commandExecution.Result;
			}
			else if (!mapExecution.Result.IsCompleted)
			{
				return mapExecution.Result;
			}
			else if (!robotExecution.Result.IsCompleted)
			{
				return robotExecution.Result;
			}
			else
			{
				return new StatusDto
				{
					ExecutionId = executionId,
					IsCompleted = true
				};
			}
		}

		private async Task<ExecutionResultStatusDto<Command>> ExecuteByCommand(Command command, Guid executionId)
		{
			var executeByCommand = new ExecuteCommandCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			return await _mediator.Send(executeByCommand);
		}

		private async Task<ExecutionResultStatusDto<Map>> ExecuteByMap(Command command, Guid executionId)
		{
			var executeByMap = new ExecuteMapCommand
			{
				ExecutionId = executionId,
				Command = command
			};
			return await _mediator.Send(executeByMap);
		}

		private async Task<ExecutionResultStatusDto<Robot>> ExecuteByRobot(Command command, Guid executionId)
		{
			var executeByRobot = new ExecuteRobotCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			return await _mediator.Send(executeByRobot);
		}

		private async Task<ValidationResultStatusDto> ValidateCommand(Command command, Guid executionId)
		{
			var commandValidation = ValidateByCommand(command, executionId);
			var mapValidation = ValidateByMap(command, executionId);
			var robotValidation = ValidateByRobot(command, executionId);

			await Task.WhenAll(commandValidation, mapValidation, robotValidation);

			if (!commandValidation.Result.IsCompleted)
			{
				return commandValidation.Result;
			}
			else if (!mapValidation.Result.IsCompleted)
			{
				return mapValidation.Result;
			}
			else if (!robotValidation.Result.IsCompleted)
			{
				return robotValidation.Result;
			}
			else
			{
				return new ValidationResultStatusDto
				{
					IsCompleted = true,
					ExecutionId = executionId
				};
			}
		}

		private async Task<ValidationResultStatusDto> ValidateByCommand(Command command, Guid executionId)
		{
			var validateByCommand = new ValidateCommandByCommandQuery
			{
				ExecutionId = executionId,
				Command = command
			};

			return await _mediator.Send(validateByCommand);
		}

		private async Task<ValidationResultStatusDto> ValidateByMap(Command command, Guid executionId)
		{
			var validateByMap = new ValidateCommandByMapQuery
			{
				ExecutionId = executionId,
				Command = command
			};

			return await _mediator.Send(validateByMap);
		}

		private async Task<ValidationResultStatusDto> ValidateByRobot(Command command, Guid executionId)
		{
			var validateByRobot = new ValidateCommandByRobotQuery
			{
				ExecutionId = executionId,
				Command = command
			};

			return await _mediator.Send(validateByRobot);
		}
	}
}

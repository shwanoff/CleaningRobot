using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Base;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.UseCases.Interfaces;
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
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request), "Request cannot be null");
			}

			//TODO: Make transactional

			var command = await _commandRepository.PeekAsync(request.ExecutionId);

			if (command == null)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = "Queue is empty"
				};
			}

			var validationResult = await ValidateCommand(command, request.ExecutionId);

			if (!validationResult.IsCorrect || !validationResult.IsValid)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = validationResult.Error
				};
			}

			var executionResult = await ExecuteCommand<ResultStatusDto>(command, request.ExecutionId);

			if (!executionResult.IsCorrect)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = executionResult.Error
				};
			}

			var result = await _commandRepository.PullAsync(request.ExecutionId);

			if (result == null)
			{
				throw new InvalidOperationException("Command was not removed from the queue");
			}

			if (!result.IsValid)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = "Validation of the command failed at the end of operation"
				};
			}

			if (!result.IsCompleted)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = "Execution of the command failed at the end of operation"
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
					
			};
			
		}

		private async Task<ValidationResultStatusDto> ValidateCommand(Command command, Guid executionId)
		{
			var commandValidation = ValidateByCommand(command, executionId);
			var mapValidation = ValidateByMap(command, executionId);
			var robotValidation = ValidateByRobot(command, executionId);

			await Task.WhenAll(commandValidation, mapValidation, robotValidation);

			if (!commandValidation.Result.IsValid)
			{
				return commandValidation.Result;
			}
			else if (!mapValidation.Result.IsValid)
			{
				return mapValidation.Result;
			}
			else if (!robotValidation.Result.IsValid)
			{
				return robotValidation.Result;
			}
			else
			{
				return new ValidationResultStatusDto
				{
					IsCorrect = true,
					IsValid = true,
					ExecutionId = executionId
				};
			}
		}

		private async Task<ResultStatusDto> ExecuteCommand<T>(Command command, Guid executionId)
		{
			var robotExecution = await ExecuteByRobot(command, executionId);

			if (!robotExecution.IsCorrect || !robotExecution.IsCompleted)
			{
				return robotExecution;
			}

			var mapExecution = await ExecuteByMap(command, executionId);

			if (!mapExecution.IsCorrect || !mapExecution.IsCompleted)
			{
				return mapExecution;
			}

			var commandExecution = await ExecuteByCommand(command, executionId);

			if (!commandExecution.IsCorrect || !commandExecution.IsCompleted)
			{
				return commandExecution;
			}

			else
			{
				return new ResultStatusDto
				{
					ExecutionId = executionId,
					IsCorrect = true,
				};
			}
		}

		private async Task<ExecutionResultStatusDto<CommandStatusDto>> ExecuteByCommand(Command command, Guid executionId)
		{
			var executeByCommand = new ExecuteCommandCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			return await _mediator.Send(executeByCommand);
		}

		private async Task<ExecutionResultStatusDto<MapStatusDto>> ExecuteByMap(Command command, Guid executionId)
		{
			var executeByMap = new ExecuteMapCommand
			{
				ExecutionId = executionId,
				Command = command
			};
			return await _mediator.Send(executeByMap);
		}

		private async Task<ExecutionResultStatusDto<RobotStatusDto>> ExecuteByRobot(Command command, Guid executionId)
		{
			var executeByRobot = new ExecuteRobotCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			return await _mediator.Send(executeByRobot);
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

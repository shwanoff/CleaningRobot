using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.UseCases.Helpers;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
    public class ExecuteNextCommand : IRequest<ResultStatusDto>
	{
        public required Guid ExecutionId { get; set; }
        public required Command Command { get; set; }
		public required bool Backoff { get; set; }
	}

	public class ExecuteNextCommandHandler(IMediator mediator, ILogAdapter logAdapter) : IRequestHandler<ExecuteNextCommand, ResultStatusDto>
	{
		private readonly IMediator _mediator = mediator;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<ResultStatusDto> Handle(ExecuteNextCommand request, CancellationToken cancellationToken)
		{
			try
			{
				request.NotNull();
				request.Command.NotNull();

				var validationResult = await ValidateCommand(request.Command, request.Backoff, request.ExecutionId);

				if (!validationResult.IsCorrect || !validationResult.IsValid)
				{
					await _logAdapter.WarningAsync($"Command validation failed. Command {request.Command}. Error {validationResult.Error}", request.ExecutionId);

					return new ResultStatusDto
					{
						ExecutionId = request.ExecutionId,
						IsCorrect = false,
						Error = validationResult.Error,
						State = validationResult.State
					};
				}

				var executionResult = await ExecuteCommand<ResultStatusDto>(request.Command, request.Backoff, request.ExecutionId);

				if (!executionResult.IsCorrect)
				{
					await _logAdapter.WarningAsync($"Command execution failed. Command {request.Command}. Error {executionResult.Error}", request.ExecutionId);

					return new ResultStatusDto
					{
						ExecutionId = request.ExecutionId,
						IsCorrect = false,
						Error = executionResult.Error,
						State = ResultState.ExecutionError
					};
				}

				await _logAdapter.TraceAsync($"Command executed. Command {request.Command}", request.ExecutionId);

				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = true,
					State = ResultState.Ok
				};
			}
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(ExecuteNextCommandHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
		}

		#region Private methods
		private async Task<ValidationResultStatusDto> ValidateCommand(Command command, bool backoff, Guid executionId)
		{
			var commandValidation = ValidateByCommand(command,backoff, executionId);
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

			return new ValidationResultStatusDto
			{
				IsCorrect = true,
				IsValid = true,
				ExecutionId = executionId,
				State = ResultState.Ok
			};
		}

		private async Task<ValidationResultStatusDto> ValidateByCommand(Command command, bool backoff, Guid executionId)
		{
			var validateByCommand = new ValidateCommandByCommandQuery
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = backoff
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

		private async Task<ResultStatusDto> ExecuteCommand<T>(Command command, bool backoff, Guid executionId)
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

			var commandExecution = await ExecuteByCommand(command, backoff, executionId);

			if (!commandExecution.IsCorrect || !commandExecution.IsCompleted)
			{
				return commandExecution;
			}

			
			return new ResultStatusDto
			{
				ExecutionId = executionId,
				IsCorrect = true,
				State = ResultState.Ok
			};
		}

		private async Task<ExecutionResultStatusDto<CommandStatusDto>> ExecuteByCommand(Command command, bool backoff, Guid executionId)
		{
			var executeByCommand = new ExecuteCommandCommand
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = backoff
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
		#endregion
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Helpers;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
    public class StartCommand: IRequest<ResultStatusDto>
	{
		public required Guid ExecutionId { get; set; }
	}

	public class StartCommandHandler(IMediator mediator) : IRequestHandler<StartCommand, ResultStatusDto>
	{
		private readonly IMediator _mediator = mediator;

		public async Task<ResultStatusDto> Handle(StartCommand request, CancellationToken cancellationToken)
		{
			request.NotNull();

			var setupBackoffResult = await SetupBackoffStrategy(request.ExecutionId);

			{
				if (!setupBackoffResult.IsCorrect)
				{
					return new ResultStatusDto
					{
						ExecutionId = request.ExecutionId,
						IsCorrect = false,
						Error = setupBackoffResult.Error,
						State = setupBackoffResult.State
					};
				}
			}

			var setupRobotResult = await SetupRobot(request.ExecutionId);

			if (!setupRobotResult.IsCorrect)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = setupRobotResult.Error,
					State = setupRobotResult.State
				};
			}

			bool finished = false;

			do
			{
				var executionResult = await ExecuteNextCommand(request.ExecutionId);

				if (!executionResult.IsCorrect)
				{
					switch (executionResult.State)
					{
						case ResultState.OutOfEnergy:
						case ResultState.QueueIsEmpty:
							finished = true;
							break;
						case ResultState.BackOff:
							// Consume enerty
							// Get back off stratagies
							// Try execute first back off strategy
							// If success, continue main queue
							// If not, try next back off strategy
							// If all back off strategies failed, return error
							ResultStatusDto backoffResult;

							do
							{
								backoffResult = await ExecuteNextBackoff(request.ExecutionId);
							} while (
								backoffResult.State != ResultState.Ok ||
								backoffResult.State != ResultState.QueueIsEmpty ||
								backoffResult.State != ResultState.QueueIsEmpty);

							continue;
						case ResultState.Error:
						case ResultState.ValidationError:
						case ResultState.ExecutionError:
							return new ResultStatusDto
							{
								ExecutionId = request.ExecutionId,
								IsCorrect = false,
								Error = executionResult.Error,
								State = executionResult.State
							};
						default:
							throw new NotImplementedException();
					}
				}
			} while (!finished);

			return new ResultStatusDto
			{
				ExecutionId = request.ExecutionId,
				IsCorrect = true,
				State = ResultState.Ok
			};
		}

		private async Task<ResultStatusDto> ExecuteNextBackoff(Guid executionId)
		{
			var command = new ExecuteNextBackoffStrategyCommand
			{
				ExecutionId = executionId
			};

			return await _mediator.Send(command);
		}

		private async Task<ResultStatusDto> SetupRobot(Guid executionId)
		{
			var setupRobotCommand = new SetupRobotOnMapCommand
			{
				ExecutionId = executionId
			};

			return await _mediator.Send(setupRobotCommand);
		}

		private async Task<ResultStatusDto> ExecuteNextCommand(Guid executionId)
		{
			var command = new ExecuteNextCommandFromQueueCommand
			{
				ExecutionId = executionId
			};

			return await _mediator.Send(command);
		}

		private async Task<ResultStatusDto> SetupBackoffStrategy(Guid executionId)
		{
			//TODO fix this

			var command = new SetupBackoffStrategyCommand
			{
				ExecutionId = executionId,
				BackoffCommands = new List<List<CommandType>>
				{
					new List<CommandType>
					{
						CommandType.TurnRight,
						CommandType.Advance,
						CommandType.TurnLeft,
					},
					new List<CommandType>
					{
						CommandType.TurnRight,
						CommandType.Advance,
						CommandType.TurnRight
					}
				},
				EnergyConsumptions = new Dictionary<CommandType, int>
				{
					{ CommandType.TurnRight, 1 },
					{ CommandType.TurnLeft, 1 },
					{ CommandType.Advance, 2 }
				}
			};
			return await _mediator.Send(command);
		}
	}
}

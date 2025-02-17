using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Handlers.Maps;
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
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request), "Request cannot be null");
			}

			var setupRobotResult = await SetupRobot(request.ExecutionId);

			if (!setupRobotResult.IsCorrect)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = setupRobotResult.Error
				};
			}

			bool finished = false;

			do
			{
				var executionResult = await ExecuteNextCommand(request.ExecutionId);

				if (!executionResult.IsCorrect)
				{
					if (executionResult.Error == "Queue is empty")
					{
						finished = true;
					}
					else
					{
						return new ResultStatusDto
						{
							ExecutionId = request.ExecutionId,
							IsCorrect = false,
							Error = executionResult.Error
						};
					}
				}
			} while (!finished);

			return new ResultStatusDto
			{
				ExecutionId = request.ExecutionId,
				IsCorrect = true,
			};
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
	}
}

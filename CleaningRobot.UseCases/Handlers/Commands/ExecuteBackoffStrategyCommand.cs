using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
	public class ExecuteBackoffStrategyCommand : IRequest<ResultStatusDto>
	{
		public required Guid ExecutionId { get; set; }
	}

	public class ExecuteBackoffStrategyCommandHandler(IBackoffRepository backoffRepository, IMediator mediator) : IRequestHandler<ExecuteBackoffStrategyCommand, ResultStatusDto>
	{
		private readonly IBackoffRepository _backoffRepository = backoffRepository;
		private readonly IMediator _mediator = mediator;

		public async Task<ResultStatusDto> Handle(ExecuteBackoffStrategyCommand request, CancellationToken cancellationToken)
		{
			request.NotNull();

			var backoffStrategies = await _backoffRepository.PeekAsync(request.ExecutionId);

			if (backoffStrategies == null || backoffStrategies.Count == 0)
			{
				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = false,
					Error = "Backoff strategies cannot be pulled from the queue",
					State = ResultState.QueueIsEmpty
				};
			}

			foreach (var backoffCommand in backoffStrategies)
			{
				backoffCommand.NotNull();

				var executionResult = await ExecuteNext(request, backoffCommand);

				if (!executionResult.IsCorrect)
				{
					await _backoffRepository.PullAsync(request.ExecutionId);

					return new ResultStatusDto
					{
						ExecutionId = request.ExecutionId,
						IsCorrect = false,
						Error = executionResult.Error,
						State = executionResult.State
					};
				}
			}

			await _backoffRepository
				.Refresh(request.ExecutionId)
				.NotNull();

			return new ResultStatusDto
			{
				ExecutionId = request.ExecutionId,
				IsCorrect = true,
				State = ResultState.Ok
			};
		}

		private async Task<ResultStatusDto> ExecuteNext(ExecuteBackoffStrategyCommand request, Entities.Entities.Command backoffCommand)
		{
			var executeCommand = new ExecuteNextCommand()
			{
				Command = backoffCommand,
				ExecutionId = request.ExecutionId,
				Backoff = true
			};

			return await _mediator.Send(executeCommand);
		}
	}
}

using CleaningRobot.Entities.Interfaces;
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

	public class ExecuteBackoffStrategyCommandHandler(IBackoffRepository backoffRepository, IMediator mediator, ILogAdapter logAdapter) : IRequestHandler<ExecuteBackoffStrategyCommand, ResultStatusDto>
	{
		private readonly IBackoffRepository _backoffRepository = backoffRepository;
		private readonly IMediator _mediator = mediator;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<ResultStatusDto> Handle(ExecuteBackoffStrategyCommand request, CancellationToken cancellationToken)
		{
			try
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

				await _logAdapter.TraceAsync($"Executing backoff strategies for execution {request.ExecutionId}. Commands: {string.Join(',', backoffStrategies)}", request.ExecutionId);

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

				var result = await _backoffRepository
					.Refresh(request.ExecutionId)
					.NotNull();

				await _logAdapter.TraceAsync($"Backoff strategies was executed. Backoff stategy was refreshed", request.ExecutionId);

				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = true,
					State = ResultState.Ok
				};
			}
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(ExecuteBackoffStrategyCommandHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
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

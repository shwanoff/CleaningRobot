﻿using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
	public class ExecuteNextBackoffStrategyCommand : IRequest<ResultStatusDto>
	{
		public required Guid ExecutionId { get; set; }
	}

	public class ExecuteNextBackoffStrategyCommandHandler(IBackoffRepository backoffRepository, IMediator mediator) : IRequestHandler<ExecuteNextBackoffStrategyCommand, ResultStatusDto>
	{
		private readonly IBackoffRepository _backoffRepository = backoffRepository;
		private readonly IMediator _mediator = mediator;

		public async Task<ResultStatusDto> Handle(ExecuteNextBackoffStrategyCommand request, CancellationToken cancellationToken)
		{
			request.NotNull();

			var backoffStrategies = await _backoffRepository.PullAsync(request.ExecutionId);

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
				var executionResult = await Execute(request, backoffCommand);

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
			}

			var refreshResult = await _backoffRepository
				.Refresh(request.ExecutionId)
				.NotNull();

			return new ResultStatusDto
			{
				ExecutionId = request.ExecutionId,
				IsCorrect = true,
				State = ResultState.Ok
			};
		}

		private async Task<ResultStatusDto> Execute(ExecuteNextBackoffStrategyCommand request, Entities.Entities.Command backoffCommand)
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

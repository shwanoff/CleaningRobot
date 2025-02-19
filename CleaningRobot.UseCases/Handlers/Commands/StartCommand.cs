using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
    public class StartCommand: IRequest<ResultStatusDto>
	{
		public required Guid ExecutionId { get; set; }
	}

	public class StartCommandHandler(IMediator mediator, IBackoffRepository backoffRepository, ILogAdapter logAdapter) : IRequestHandler<StartCommand, ResultStatusDto>
	{
		private readonly IBackoffRepository _backoffRepository = backoffRepository;
		private readonly IMediator _mediator = mediator;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<ResultStatusDto> Handle(StartCommand request, CancellationToken cancellationToken)
		{
			try
			{
				request.NotNull();

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

				await _logAdapter.TraceAsync("Execution started", request.ExecutionId);

				bool executionFinished = false;

				do
				{
					var queueEexecutionResult = await ExecuteNextFromQueue(request.ExecutionId);

					if (!queueEexecutionResult.IsCorrect)
					{
						switch (queueEexecutionResult.State)
						{
							case ResultState.OutOfEnergy:
							case ResultState.QueueIsEmpty:
								executionFinished = true;
								break;
							case ResultState.BackOff:
								if (_backoffRepository.Settings.ConsumeEnergyWhenBackOff)
								{
									await ConsumeEnergy(queueEexecutionResult.Error, request.ExecutionId);
								}

								bool backoffFinished = false;
								do
								{
									var backoffExecutionResult = await ExecuteNextBackoffStrategy(request.ExecutionId);

									switch (backoffExecutionResult.State)
									{
										case ResultState.BackOff:
											if (_backoffRepository.Settings.ConsumeEnergyWhenBackOff)
											{
												await ConsumeEnergy(backoffExecutionResult.Error, request.ExecutionId);
											}
											break;
										case ResultState.Ok:
											if (_backoffRepository.Settings.StopWhenBackOff)
											{
												executionFinished = true;
											}

											backoffFinished = true;
											break;
										case ResultState.QueueIsEmpty:
										case ResultState.OutOfEnergy:
											executionFinished = true;
											break;
										case ResultState.Error:
										case ResultState.ValidationError:
										case ResultState.ExecutionError:
											return new ResultStatusDto
											{
												ExecutionId = request.ExecutionId,
												IsCorrect = false,
												Error = backoffExecutionResult.Error,
												State = backoffExecutionResult.State
											};
										default:
											throw new NotImplementedException();
									}

								} while (!backoffFinished);

								continue;
							case ResultState.Error:
							case ResultState.ValidationError:
							case ResultState.ExecutionError:
								return new ResultStatusDto
								{
									ExecutionId = request.ExecutionId,
									IsCorrect = false,
									Error = queueEexecutionResult.Error,
									State = queueEexecutionResult.State
								};
							default:
								throw new NotImplementedException();
						}
					}
				} while (!executionFinished);

				await _logAdapter.TraceAsync("Execution finished", request.ExecutionId);

				return new ResultStatusDto
				{
					ExecutionId = request.ExecutionId,
					IsCorrect = true,
					State = ResultState.Ok
				};
			}
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(StartCommandHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
		}

		private async Task<ResultStatusDto> ExecuteNextBackoffStrategy(Guid executionId)
		{
			var command = new ExecuteBackoffStrategyCommand
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

		private async Task<ResultStatusDto> ExecuteNextFromQueue(Guid executionId)
		{
			var command = new ExecuteCommandFromQueueCommand
			{
				ExecutionId = executionId
			};

			return await _mediator.Send(command);
		}

		private async Task<ResultStatusDto> ConsumeEnergy(string? energy, Guid executionId)
		{
			var consumeEnergyCommand = new ConsumeEnergyCommand
			{
				ExecutionId = executionId,
				ConsumedEnergy = int.Parse(energy ?? "0")
			};

			return await _mediator.Send(consumeEnergyCommand);
		}
	}
}

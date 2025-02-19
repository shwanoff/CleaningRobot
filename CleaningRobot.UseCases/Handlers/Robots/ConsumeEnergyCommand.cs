using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Robots
{
    public class ConsumeEnergyCommand : IRequest<ExecutionResultStatusDto<RobotStatusDto>>
	{
		public required Guid ExecutionId { get; set; }
		public required int ConsumedEnergy { get; set; }
	}

	public class ConsumeEnergyCommandHandler(IRepository<Robot> robotRepository, ILogAdapter logAdapter) : IRequestHandler<ConsumeEnergyCommand, ExecutionResultStatusDto<RobotStatusDto>>
	{
		private readonly IRepository<Robot> _robotRepository = robotRepository;
		private readonly ILogAdapter _logAdapter = logAdapter;
		public async Task<ExecutionResultStatusDto<RobotStatusDto>> Handle(ConsumeEnergyCommand request, CancellationToken cancellationToken = default)
		{
			try
			{
				request.NotNull();
				request.ConsumedEnergy.IsPositive();

				var robot = await _robotRepository
					.GetByIdAsync(request.ExecutionId)
					.NotNull();

				robot.Battery -= request.ConsumedEnergy;

				var result = await _robotRepository
					.UpdateAsync(robot, request.ExecutionId)
					.NotNull();

				await _logAdapter.TraceAsync($"Robot consumed {request.ConsumedEnergy} energy. Robot state {robot}", request.ExecutionId);

				return new ExecutionResultStatusDto<RobotStatusDto>
				{
					Result = new RobotStatusDto
					{
						X = robot.Position.X,
						Y = robot.Position.Y,
						Facing = robot.Position.Facing,
						Battery = robot.Battery,
						IsCorrect = true,
						ExecutionId = request.ExecutionId
					},
					IsCorrect = true,
					ExecutionId = request.ExecutionId,
					IsCompleted = true,
					State = ResultState.Ok
				};
			}
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(ConsumeEnergyCommandHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
		}
	}
}

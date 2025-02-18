using CleaningRobot.Entities.Entities;
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

	public class ConsumeEnergyCommandHandler(IRepository<Robot> robotRepository) : IRequestHandler<ConsumeEnergyCommand, ExecutionResultStatusDto<RobotStatusDto>>
	{
		private readonly IRepository<Robot> _robotRepository = robotRepository;
		public async Task<ExecutionResultStatusDto<RobotStatusDto>> Handle(ConsumeEnergyCommand request, CancellationToken cancellationToken = default)
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
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Robots
{
	public class CreateRobotCommand : IRequest<RobotStatusDto>
	{
		public required Guid ExecutionId { get; set; }
		public required RobotPosition Position { get; set; }
		public required int Battery { get; set; }
	}

	public class CreateRobotCommandHandler(IRepository<Robot> robotRepository) : IRequestHandler<CreateRobotCommand, RobotStatusDto>
	{
		private readonly IRepository<Robot> _robotRepository = robotRepository;

		public async Task<RobotStatusDto> Handle(CreateRobotCommand request, CancellationToken cancellationToken = default)
		{
			var robot = new Robot(request.Position, request.Battery);

			var result = await _robotRepository.AddAsync(robot, request.ExecutionId);

			if (result == null)
			{
				throw new Exception("Could not create the robot");
			}

			return new RobotStatusDto
			{
				ExecutionId = request.ExecutionId,
				X = result.Position.X,
				Y = result.Position.Y,
				Facing = result.Position.Facing,
				Battery = result.Battery,
				IsCorrect = true
			};
		}
	}
}

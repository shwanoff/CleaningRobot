using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
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
			request.NotNull();
			request.Position.NotNull();
			request.Battery.IsPositive();

			var robot = new Robot(request.Position, request.Battery);

			var result = await _robotRepository
				.AddAsync(robot, request.ExecutionId)
				.NotNull();

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

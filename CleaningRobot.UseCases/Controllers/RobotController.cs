using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Extensions;
using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.UseCases.Interfaces.Controllers;
using MediatR;

namespace CleaningRobot.UseCases.Controllers
{
	public class RobotController(IMediator mediator) : IRobotController
	{
		private readonly IMediator _mediator = mediator;

		public async Task<RobotStatusDto> CreateAsync(RobotDataDto data, Guid executionId)
		{
			var command = new CreateRobotCommand
			{
				ExecutionId = executionId,
				Position = new RobotPosition(data.X, data.Y, data.Facing.ToFacing()),
				Battery = data.Battery
			};

			return await _mediator.Send(command);
		}

		public async Task<RobotStatusDto> GetAsync(Guid executionId)
		{
			var query = new GetRobotQuery
			{
				ExecutionId = executionId
			};

			return await _mediator.Send(query);
		}
	}
}

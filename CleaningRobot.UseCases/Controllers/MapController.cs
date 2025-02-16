using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Interfaces.Controllers;
using MediatR;

namespace CleaningRobot.UseCases.Controllers
{
	public class MapController(IMediator mediator) : IMapController
	{
		private readonly IMediator _mediator = mediator;

		public async Task<MapStatusDto> CreateAsync(MapDataDto data, Guid executionId)
		{
			var command = new CreateMapCommand
			{
				ExecutionId = executionId,
				MapData = data.Map
			};

			return await _mediator.Send(command);
		}

		public async Task<MapStatusDto> GetAsync(Guid executionId)
		{
			var query = new GetMapQuery
			{
				ExecutionId = executionId
			};

			return await _mediator.Send(query);
		}
	}
}

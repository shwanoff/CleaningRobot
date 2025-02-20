﻿using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Maps
{
    public class SetupRobotOnMapCommand : IRequest<ResultStatusDto>
	{
		public Guid ExecutionId { get; set; }
	}

	public class SetupRobotOnMapCommandHandler(IRepository<Robot> robotRepository, IRepository<Map> mapRepository, ILogAdapter logAdapter) : IRequestHandler<SetupRobotOnMapCommand, ResultStatusDto>
	{
		private readonly IRepository<Robot> _robotRepository = robotRepository;
		private readonly IRepository<Map> _mapRepository = mapRepository;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<ResultStatusDto> Handle(SetupRobotOnMapCommand request, CancellationToken cancellationToken)
		{
			try
			{
				request.NotNull();

				var robot = await _robotRepository
					.GetByIdAsync(request.ExecutionId)
					.NotNull();

				var map = await _mapRepository
					.GetByIdAsync(request.ExecutionId)
					.NotNull();

				var position = robot.Position;

				if (!PositionHelper.IsCellAvailable(map, position, out string? error))
				{
					throw new InvalidOperationException(error);
				}

				Update(map, position);

				var result = await _mapRepository
					.UpdateAsync(map, request.ExecutionId)
					.NotNull();

				await _logAdapter.TraceAsync($"Robot setup on map. Map state {map}", request.ExecutionId);
				await _logAdapter.DebugAsync(map.Draw(), request.ExecutionId);

				return new ExecutionResultStatusDto<MapStatusDto>
				{
					Result = new MapStatusDto
					{
						ExecutionId = request.ExecutionId,
						Width = result.Width,
						Height = result.Height,
						IsCorrect = true,
						Cells = [.. result.Cells.Cast<Cell>()
					.Select(c => new CellStatusDto
					{
						X = c.Position.X,
						Y = c.Position.Y,
						Type = c.Type,
						State = c.State,
						ExecutionId = request.ExecutionId,
						IsCorrect = true
					})]
					},
					IsCorrect = true,
					IsCompleted = true,
					ExecutionId = request.ExecutionId,
					State = ResultState.Ok
				};
			}
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(SetupRobotOnMapCommandHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
		}

		#region Private methods
		private static void Update(Map map, Position position)
		{
			var cell = map.Cells[position.Y, position.X];
			cell.State = CellState.Visited;
		}
		#endregion
	}
}

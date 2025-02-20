﻿using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Maps
{
	public class GetMapQuery : IRequest<MapStatusDto>
	{
		public required Guid ExecutionId { get; set; }
	}

	public class GetMapQueryHandler(IRepository<Map> mapRepository, ILogAdapter logAdapter) : IRequestHandler<GetMapQuery, MapStatusDto>
	{
		private readonly IRepository<Map> _mapRepository = mapRepository;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<MapStatusDto> Handle(GetMapQuery request, CancellationToken cancellationToken = default)
		{
			try
			{
				request.NotNull();

				var map = await _mapRepository
					.GetByIdAsync(request.ExecutionId)
					.NotNull();

				await _logAdapter.TraceAsync($"Map state {map}", request.ExecutionId);
				await _logAdapter.DebugAsync(map.Draw(), request.ExecutionId);

				return new MapStatusDto
				{
					ExecutionId = request.ExecutionId,
					Width = map.Width,
					Height = map.Height,
					IsCorrect = true,
					Cells = [.. map.Cells.Cast<Cell>()
					.Select(c => new CellStatusDto
					{
						X = c.Position.X,
						Y = c.Position.Y,
						Type = c.Type,
						State = c.State,
						ExecutionId = request.ExecutionId,
						IsCorrect = true
					})]
				};
			}
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(GetMapQueryHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
		}
	}
}

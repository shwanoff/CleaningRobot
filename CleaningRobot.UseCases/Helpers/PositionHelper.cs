using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;

namespace CleaningRobot.UseCases.Helpers
{
	public static class PositionHelper
	{
		public static Position GetNextPosition(RobotPosition position, CommandType commandType)
		{
			return commandType switch
			{
				CommandType.Advance => position.Facing switch
				{
					Facing.North => new Position(position.X,	 position.Y - 1),
					Facing.East  => new Position(position.X + 1, position.Y),
					Facing.South => new Position(position.X,	 position.Y + 1),
					Facing.West  => new Position(position.X - 1, position.Y),
					_ => throw new ArgumentException($"Facing '{position.Facing}' is invalid")
				},
				CommandType.Back => position.Facing switch
				{
					Facing.North => new Position(position.X,	 position.Y + 1),
					Facing.East  => new Position(position.X - 1, position.Y),
					Facing.South => new Position(position.X,	 position.Y - 1),
					Facing.West  => new Position(position.X + 1, position.Y),
					_ => throw new ArgumentException($"Facing '{position.Facing}' is invalid")
				},
				_ => throw new ArgumentException($"Type '{commandType}' is invalid")
			};
		}

		public static bool IsCellAvailable(Map map, Position position, out string? error)
		{
			error = null;

			if (position.X < 0 || position.X >= map.Width || position.Y < 0 || position.Y >= map.Height)
			{
				error = "The robot cannot be outside the map";
				return false;
			}

			var cell = map.Cells[position.Y, position.X];

			if (cell.Type == CellType.Wall)
			{
				error = "The robot cannot be on a wall";
				return false;
			}

			if (cell.Type == CellType.Column)
			{
				error = "The robot cannot be on a column";
				return false;
			}

			return true;
		}
	}
}

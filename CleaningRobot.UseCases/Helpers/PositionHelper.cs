using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;

namespace CleaningRobot.UseCases.Helpers
{
	internal static class PositionHelper
	{
		internal static Position GetNextPosition(RobotPosition position, CommandType commandType)
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
	}
}

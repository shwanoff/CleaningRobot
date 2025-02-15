using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Extensions;
using CleaningRobot.UseCases.Dto;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Controllers;
using CleaningRobot.UseCases.Interfaces.Operators;

namespace CleaningRobot.UseCases.Controllers
{
	internal class RobotController : IRobotController, IRobotOperator
	{
		private bool IsRobotCreated => _robot != null;

		private readonly IMapOperator _mapOperation;

		private Robot _robot;

		public void Create(int x, int y, string facing, int battery)
		{
			if (x < 0 || x >= _mapOperation.GetMapWidth())
			{
				throw new ArgumentException($"The x-coordinate '{x}' is out of bounds. The map width is {_mapOperation.GetMapWidth()}");
			}

			if (y < 0 || y >= _mapOperation.GetMapHeight())
			{
				throw new ArgumentException($"The y-coordinate '{y}' is out of bounds. The map height is {_mapOperation.GetMapHeight()}");
			}

			if (battery < 0)
			{
				throw new ArgumentException($"The battery level '{battery}' is invalid. The battery level must be a positive integer");
			}

			var facingItem = facing.ToFacing();

			var cellStatus = _mapOperation.GetCellStatus(x, y);
			if (cellStatus == null || cellStatus.Type == CellType.Column || cellStatus.Type == CellType.Wall)
			{
				throw new ArgumentException($"The robot cannot be placed on a {cellStatus.Type.ToString().ToLower()}");
			}

			_robot = new Robot(x, y, facingItem, battery);
		}

		public RobotPosition GetRobotPosition()
		{
			if (!IsRobotCreated)
			{
				throw new InvalidOperationException("The robot has not been created yet");
			}

			return _robot.Position;
		}

		public RobotStatusDto GetCurrentStatus()
		{
			if (!IsRobotCreated)
			{
				throw new InvalidOperationException("The robot has not been created yet");
			}

			return new RobotStatusDto
			{
				X = _robot.Position.X,
				Y = _robot.Position.Y,
				Facing = _robot.Position.Facing,
				Battery = _robot.Battery
			};

		}

		public bool ValidateCommand(Command command, out string? error)
		{
			error = null;

			if (!IsRobotCreated)
			{
				throw new InvalidOperationException("The robot has not been created yet");
			}

			if (command == null)
			{
				throw new ArgumentNullException(nameof(command), "Command cannot be null");
			}

			return IsEnoughBattery(command, out error);
		}

		public void ExecuteCommand(Command command)
		{
			if (command == null)
			{
				throw new ArgumentNullException(nameof(command), "Command cannot be null");
			}

			if (!IsRobotCreated)
			{
				throw new InvalidOperationException("The robot has not been created yet");
			}

			if (ValidateCommand(command, out var error))
			{
				switch (command.CommandType)
				{
					case CommandType.TurnLeft:
						TurnLeft(command.ConsumedEnergy);
						break;
					case CommandType.TurnRight:
						TurnRight(command.ConsumedEnergy);
						break;
					case CommandType.Advance:
						Advance(command.ConsumedEnergy);
						break;
					case CommandType.Back:
						Back(command.ConsumedEnergy);
						break;
					case CommandType.Clean:
						Clean(command.ConsumedEnergy);
						break;
					default:
						throw new ArgumentException($"CommandType '{command}' is invalid. Valid values are: TL, TR, A, B, C, TurnLeft, Turn Right, Advance, Back, Clean");
				}
			}
			else
			{
				throw new ArgumentException(error);
			}
		}

		

		#region Private methods
		private void TurnLeft(int energy)
		{
			_robot.Position.Facing = _robot.Position.Facing switch
			{
				Facing.North => Facing.West,
				Facing.West  => Facing.South,
				Facing.South => Facing.East,
				Facing.East  => Facing.North,
				_ => throw new ArgumentException($"Facing '{_robot.Position.Facing}' is invalid")
			};

			ConsumeBattery(energy);
		}

		private void TurnRight(int energy)
		{
			_robot.Position.Facing = _robot.Position.Facing switch
			{
				Facing.North => Facing.East,
				Facing.East  => Facing.South,
				Facing.South => Facing.West,
				Facing.West  => Facing.North,
				_ => throw new ArgumentException($"Facing '{_robot.Position.Facing}' is invalid")
			};

			ConsumeBattery(energy);
		}

		private void Advance(int energy)
		{
			var nextPositon = PositionHelper.GetNextPosition(_robot.Position, CommandType.Advance);
			Move(nextPositon);

			ConsumeBattery(energy);
		}

		private void Back(int energy)
		{
			var nextPositon = PositionHelper.GetNextPosition(_robot.Position, CommandType.Back);
			Move(nextPositon);

			ConsumeBattery(energy);
		}

		private void Clean(int energy)
		{
			ConsumeBattery(energy);
		}

		private bool IsEnoughBattery(Command command, out string? error)
		{
			error = null;

			if (_robot.Battery < command.ConsumedEnergy)
			{
				error = $"The battery level is too low. The battery level is {_robot.Battery} and the command requires {command.ConsumedEnergy}";
				return false;
			}

			return true;
		}

		private void ConsumeBattery(int energy)
		{
			_robot.Battery -= energy;
		}

		private void Move(Position newPosition)
		{
			_robot.Position.X = newPosition.X;
			_robot.Position.Y = newPosition.Y;
		}
		#endregion
	}
}

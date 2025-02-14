using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto;
using CleaningRobot.UseCases.Interfaces.Controllers;
using CleaningRobot.UseCases.Interfaces.Operations;

namespace CleaningRobot.UseCases.Controllers
{
    public class RobotController : IRobotController, IRobotOperation
	{
		public void ExecuteCommand(Command command)
		{
			switch (command.CommandType)
			{
				case CommandType.TurnLeft:
					TurnLeft();
					break;
				case CommandType.TurnRight:
					TurnRight();
					break;
				case CommandType.Advance:
					Advance();
					break;
				case CommandType.Back:
					Back();
					break;
				case CommandType.Clean:
					Clean();
					break;
				default:
					throw new ArgumentException($"CommandType '{command}' is invalid. Valid values are: TL, TR, A, B, C, TurnLeft, Turn Right, Advance, Back, Clean");
			}
		}

		public bool ValidateCommand(Command command, out string error)
		{
			throw new NotImplementedException();
		}

		public void Create(int x, int y, string facing, int battery)
		{
			throw new NotImplementedException();
		}

		public RobotStatusDto GetCurrentStatus()
		{
			throw new NotImplementedException();
		}

		#region Private methods
		private void TurnLeft()
		{
			throw new NotImplementedException();
		}

		private void TurnRight()
		{
			throw new NotImplementedException();
		}

		private void Advance()
		{
			throw new NotImplementedException();
		}

		private void Back()
		{
			throw new NotImplementedException();
		}

		private void Clean()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}

using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Interfaces;

namespace CleaningRobot.UseCases.Controllers
{
    public class RobotController : IRobotController
	{
		public void ExecuteCommand(Command command)
		{
			switch (command)
			{
				case Command.TurnLeft:
					TurnLeft();
					break;
				case Command.TurnRight:
					TurnRight();
					break;
				case Command.Advance:
					Advance();
					break;
				case Command.Back:
					Back();
					break;
				case Command.Clean:
					Clean();
					break;
				default:
					throw new ArgumentException($"Command '{command}' is invalid. Valid values are: TL, TR, A, B, C, TurnLeft, Turn Right, Advance, Back, Clean");
			}
		}

		public void ValidateCommand(Command command)
		{
			throw new NotImplementedException();
		}

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
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Extensions;
using CleaningRobot.UseCases.Dto;
using CleaningRobot.UseCases.Interfaces.Controllers;
using CleaningRobot.UseCases.Interfaces.Operators;

namespace CleaningRobot.UseCases.Controllers
{
	internal class CommandContoller(IRobotOperator robotOperation, IMapOperator mapOperation) : ICommandController, ICommandOperator
	{
		private readonly IRobotOperator _robotOperation = robotOperation;
		private readonly IMapOperator _mapOperation = mapOperation;

		private Queue<Command> commandQueue = [];

		public void Create(IEnumerable<string> commands)
		{
			foreach (var command in commands)
			{
				var newCommand = new Command(command.ToCommand(), 1); // TODO: Get battery from configuration

				if (!_robotOperation.ValidateCommand(newCommand, out var error))
				{
					throw new ArgumentException($"Command '{command}' is invalid. {error}");
				}

				commandQueue.Enqueue(newCommand);
			}
		}

		public void ExcecuteAll()
		{
			while (commandQueue.Count != 0)
			{
				ExecuteNext();
			}
		}

		public CommandStatusDto ExecuteNext()
		{
			var command = commandQueue.Dequeue();

			if (command == null)
			{
				throw new InvalidOperationException("No commands to execute");
			}

			if (!_robotOperation.ValidateCommand(command, out var robotError))
			{
				throw new ArgumentException($"Command '{command}' is invalid. {robotError}");
			}

			if (!_mapOperation.ValidateCommand(command, out var mapError))
			{
				throw new ArgumentException($"Command '{command}' is invalid. {mapError}");
			}

			_robotOperation.ExecuteCommand(command);
			_mapOperation.Update(command);

			return new CommandStatusDto()
			{
				CommandType = command.CommandType,
				ConsumedEnergy = command.ConsumedEnergy
			};
		}

		public bool ValidateCommand(Command command, out string? error)
		{
			error = null;

			if (command == null)
			{
				error = "Command is null";
				return false;
			}

			if (command.CommandType == (CommandType)0)
			{
				error = $"CommandType '{command}' is invalid. Valid values are: TL, TR, A, B, C, TurnLeft, Turn Right, Advance, Back, Clean";
				return false;
			}

			if (command.ConsumedEnergy < 0)
			{
				error = "ConsumedEnergy is invalid. It must be greater than or equal to 0";
				return false;
			}

			return true;
		}
	}
}

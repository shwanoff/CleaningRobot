using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Extensions;
using CleaningRobot.UseCases.Dto;
using CleaningRobot.UseCases.Interfaces.Controllers;
using CleaningRobot.UseCases.Interfaces.Operations;

namespace CleaningRobot.UseCases.Controllers
{
	/// <summary>
	/// Command controller. Responsible for external and internal command operations
	/// </summary>
	/// <param name="robotOperation">The robot operation interface</param>
	/// <param name="mapOperation">The map operation interface</param>
	internal class CommandContoller(IRobotOperation robotOperation, IMapOperation mapOperation) : ICommandController, ICommandOperation
	{
		private readonly IRobotOperation _robotOperation = robotOperation;
		private readonly IMapOperation _mapOperation = mapOperation;

		private Queue<Command> commandQueue = [];

		/// <summary>
		/// Creates commands from a collection of command strings
		/// </summary>
		/// <param name="commands">A collection of command strings to be created</param>
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

		/// <summary>
		/// Executes all commands in the queue
		/// </summary>
		public void ExcecuteAll()
		{
			while (commandQueue.Count != 0)
			{
				ExecuteNext();
			}
		}

		/// <summary>
		/// Executes the next command in the queue
		/// </summary>
		/// <returns>Returns the status of the executed command</returns>
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

			return new CommandStatusDto(command.CommandType, command.ConsumedEnergy);
		}

		/// <summary>
		/// Validates the given command
		/// </summary>
		/// <param name="command">The command to validate</param>
		/// <param name="error">The error message if the command is invalid</param>
		/// <returns>True if the command is valid, otherwise false</returns>
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

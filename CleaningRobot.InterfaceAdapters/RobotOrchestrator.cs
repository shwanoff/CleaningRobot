using CleaningRobot.InterfaceAdapters.Interfaces;
using CleaningRobot.UseCases.Interfaces;

namespace CleaningRobot.InterfaceAdapters
{
	public class RobotOrchestrator : IRobotOrchestrator
	{
		private readonly IRobotController _robotController;
		private readonly IMapController _mapController;
		private readonly IFileAdapter _fileAdapter;
		private readonly IJsonAdapter _jsonAdapter;
		private readonly ILogAdapter _logAdapter;

		public bool Execute(string inputFilePath, string outputFilePath)
		{
			try
			{
				return true;
			}
			catch (Exception ex)
			{
				_logAdapter.Error(ex.Message);
				return false;
			}
		}

		private void CreateCommandsList()
		{
			throw new NotImplementedException();
		}

		private void CreateMap()
		{
			throw new NotImplementedException();
		}

		private void CreateRobot()
		{
			throw new NotImplementedException();
		}

		private bool ValidateInput(string filePath)
		{
			throw new NotImplementedException();
		}
	}
}

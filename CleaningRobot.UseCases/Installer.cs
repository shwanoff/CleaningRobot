using CleaningRobot.UseCases.Controllers;
using CleaningRobot.UseCases.Interfaces.Controllers;
using CleaningRobot.UseCases.Interfaces.Operators;
using Microsoft.Extensions.DependencyInjection;

namespace CleaningRobot.UseCases
{
	public static class Installer
	{
		public static void AddUseCases(this IServiceCollection services)
		{
			services.AddTransient<IRobotController, RobotController>();
			services.AddTransient<IMapController, MapController>();
			services.AddTransient<ICommandController, CommandContoller>();
			services.AddTransient<IRobotOperator, RobotController>();
			services.AddTransient<IMapOperator, MapController>();
			services.AddTransient<ICommandOperator, CommandContoller>();
		}
	}
}

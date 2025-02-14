using CleaningRobot.UseCases.Controllers;
using CleaningRobot.UseCases.Interfaces.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace CleaningRobot.UseCases
{
	public static class Installer
	{
		public static void AddUseCases(this IServiceCollection services)
		{
			services.AddTransient<IRobotController, RobotController>();
			services.AddTransient<IMapController, MapController>();
		}
	}
}

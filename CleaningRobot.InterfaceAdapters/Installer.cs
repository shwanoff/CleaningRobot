using CleaningRobot.InterfaceAdapters.Adapters;
using CleaningRobot.InterfaceAdapters.Interfaces;
using CleaningRobot.UseCases;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CleaningRobot.InterfaceAdapters
{
	public static class Installer
	{
		public static IServiceCollection AddInterfaceAdapters(this IServiceCollection services)
		{
			services.AddUseCases();

			services.AddTransient<IFileAdapter, FileAdapter>();
			services.AddTransient<IJsonAdapter, JsonAdapter>();
			services.AddTransient<ILogAdapter, TxtLogAdapter>();

			services.AddTransient<IRobotOrchestrator, RobotOrchestrator>();

			services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())); 

			return services;
		}
	}
}

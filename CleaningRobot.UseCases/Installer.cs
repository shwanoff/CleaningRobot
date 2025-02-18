using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Controllers;
using CleaningRobot.UseCases.Interfaces.Controllers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using CleaningRobot.UseCases.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CleaningRobot.UseCases
{
	public static class Installer
	{
		public static IServiceCollection AddUseCases(this IServiceCollection services)
		{
			services.AddSingleton<IRepository<Robot>, RobotRepository>();
			services.AddSingleton<IRepository<Map>, MapRepository>();
			services.AddSingleton<IBackoffRepository, BackoffRepository>();
			services.AddSingleton<CommandRepository>();

			services.AddSingleton<IRepository<Queue<Command>>>(provider => provider.GetService<CommandRepository>());
			services.AddSingleton<IQueueRepository<Command>>(provider => provider.GetService<CommandRepository>());


			services.AddScoped<IRobotController, RobotController>();
			services.AddScoped<IMapController, MapController>();
			services.AddScoped<ICommandController, CommandContoller>();

			return services;
		}
	}
}

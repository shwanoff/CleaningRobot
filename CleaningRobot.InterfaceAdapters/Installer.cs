using CleaningRobot.InterfaceAdapters.Adapters;
using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;
using CleaningRobot.UseCases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleaningRobot.InterfaceAdapters
{
	public static class Installer
	{
		public static IServiceCollection AddInterfaceAdapters(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IFileAdapter, FileAdapter>();
			services.AddScoped<IJsonAdapter, JsonAdapter>();
			services.AddScoped<ILogAdapter, TxtLogAdapter>();

			services.AddScoped<IRobotOrchestrator, RobotOrchestrator>();

			services.AddSingleton(configuration.GetSection("Logging").Get<TxtLogConfigurationDto>());

			return services;
		}
	}
}

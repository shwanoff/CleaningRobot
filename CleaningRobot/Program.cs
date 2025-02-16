using CleaningRobot;
using CleaningRobot.InterfaceAdapters.Interfaces;
using Microsoft.Extensions.DependencyInjection;

if (args.Length != 2)
{
	Console.WriteLine("Usage: cleaning_robot <source.json> <result.json>");
	return;
}
string inputFilePath = args[0];
string outputFilePath = args[1];

var serviceProvider = Build.ConfigureServices();
var orchestrator = serviceProvider.GetService<IRobotOrchestrator>();

if (orchestrator == null)
{
	Console.WriteLine("Failed to initialize the orchestrator.");
	return;
}

var result = await orchestrator.ExecuteAsync(inputFilePath, outputFilePath);

if (result.Success)
{
	Console.WriteLine("Execution completed successfully.");
}
else
{
	Console.WriteLine($"Execution failed: {result.Error?.Message ?? "Unknown error"}");
}

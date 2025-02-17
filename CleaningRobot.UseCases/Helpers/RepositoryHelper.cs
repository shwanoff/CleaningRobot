using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Helpers
{
    public static class RepositoryHelper
    {
		public static void UpdateItem<T>(T currentState, Dictionary<string, object> valuesToUpdate)
		{
			if (currentState == null)
			{
				throw new ArgumentNullException(nameof(currentState), "Current state cannot be null.");
			}

			foreach (var (key, value) in valuesToUpdate)
			{
				var property = currentState.GetType().GetProperty(key);
				if (property == null)
				{
					throw new ArgumentException($"Property {key} does not exist.");
				}

				property.SetValue(currentState, value);
			}
		}

		private static void ValidateUpdate(Command command, Dictionary<string, object> valuesToUpdate)
		{
			foreach (var (key, value) in valuesToUpdate)
			{
				var property = command.GetType().GetProperty(key);

				if (property == null)
				{
					throw new ArgumentException($"Property {key} does not exist.");
				}

				if (!property.PropertyType.IsAssignableFrom(value.GetType()))
				{
					throw new ArgumentException($"Property {key} is of type {property.PropertyType.Name} and cannot be set to {value.GetType().Name}.");
				}

				if (property.GetValue(command)?.Equals(value) == true)
				{
					throw new InvalidOperationException($"Property {key} is not updated.");
				}
			}
		}
	}
}

namespace CleaningRobot.UseCases.Helpers
{
    public static class ValidationHelper
    {
        public static T NotNull<T>(this T item)
			where T : class
		{
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), $"{item} cannot be null");
            }

            return item;
        }

        public static int IsPositive(this int item)
        {
            if (item < 0)
            {
                throw new ArgumentException($"{item} must be positive or zero", nameof(item));
            }

            return item;
        }

        public static bool IsTrue(this bool item)
        {
            if (!item)
            {
                throw new ArgumentException($"{item} must be true", nameof(item));
            }

            return item;
        }

		public static bool IsFalse(this bool item)
		{
			if (item)
			{
				throw new ArgumentException($"{item} must be false", nameof(item));
			}
			return item;
		}

		public static IDictionary<TKey, TValue> KeyExists<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			if (!dictionary.ContainsKey(key))
			{
				throw new KeyNotFoundException($"Key '{key}' does not exist in the dictionary.");
			}

			return dictionary;
		}

		public static IDictionary<TKey, TValue> KeyNotExists<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			if (dictionary.ContainsKey(key))
			{
				throw new InvalidOperationException($"Key '{key}' already exists in the dictionary.");
			}
			return dictionary;
		}


		public static IEnumerable<T> HasItems<T>(this IEnumerable<T> items)
		{
			if (items == null || !items.Any())
			{
				throw new ArgumentException("Collection must have at least one item", nameof(items));
			}

			return items;
		}
	}
}

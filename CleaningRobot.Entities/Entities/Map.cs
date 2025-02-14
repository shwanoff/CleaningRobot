﻿using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	/// <summary>
	/// Represents the map of the environment
	/// </summary>
	/// <param name="cells"> Matrix of the map </param>
	public class Map(Cell[,] cells)
	{
		/// <summary>
		/// Width of the map
		/// </summary>
		public uint Width
		{
			get
			{
				return (uint)Cells.GetLength(0);
			}
		}

		/// <summary>
		/// Height of the map
		/// </summary>
		public uint Height
		{
			get
			{
				return (uint)Cells.GetLength(1);
			}
		}

		/// <summary>
		/// Matrix of the map
		/// </summary>
		public Cell[,] Cells { get; private set; } = cells;

		public override string ToString()
		{
			return $"Map: {Width}x{Height}";
		}
	}
}

﻿namespace CleaningRobot.InterfaceAdapters.Dto
{
	public class PositionDto
    {
		public int X { get; set; }
		public int Y { get; set; }

		override public string ToString()
		{
			return $"({X}, {Y})";
		}
	}
}

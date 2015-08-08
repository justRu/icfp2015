using System;

namespace Solver
{
	public struct Position
	{
		public int X;
		public int Y;

		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}

		public double DistanceTo(Position other)
		{
			int dx = (other.X - X);
			int dy = (other.Y - Y);
			return Math.Sqrt(dx*dx + dy*dy);
		}
	}
}

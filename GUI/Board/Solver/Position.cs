using System;

namespace Solver
{
	public struct Position : IEquatable<Position>
	{
		public int X;
		public int Y;

		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}

		public bool Equals(Position other)
		{
			return X == other.X && Y == other.Y;
		}
	}
}

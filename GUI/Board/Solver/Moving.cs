using System.Linq;

namespace Solver
{
	public static class Moving
	{
        public static Unit Translate(this Unit unit, MoveDirection direction)
		{
			switch (direction)
			{
				case MoveDirection.SouthEast:
				case MoveDirection.East:
				case MoveDirection.SouthWest:
				case MoveDirection.West:
					return TranslateUnit(unit, direction);
				case MoveDirection.RotateClockwise:
					return Rotate(unit, true);
				case MoveDirection.RotateCounterClockwise:
					return Rotate(unit, false);
				default:
					return unit;
			}
		}

		public static Unit Rotate(Unit unit, bool direction)
		{
			return new Unit
			{
				Pivot = unit.Pivot,
				Members = unit.Members.Select(member => Rotate(member, unit.Pivot, direction)).ToArray()
			};
		}

        public static Unit Spawn(Input input, Unit unit)
		{
			var maxX = unit.Members.Max(t => t.X);
			int dx = (input.Width - maxX - 1)/2;
			return dx == 0 ? unit : Translate(unit, dx, 0);
		}

		public static Unit Spawn(this Field field, Unit unit)
		{
			var maxX = unit.Members.Max(t => t.X);
			int dx = (field.Width - maxX - 1) / 2;
			return dx == 0 ? unit : Translate(unit, dx, 0);
		}

		public static Position Translate(this Position p, MoveDirection direction)
		{
			switch (direction)
			{
				case MoveDirection.East:
					return new Position(p.X + 1, p.Y);
				case MoveDirection.West:
					return new Position(p.X - 1, p.Y);
				case MoveDirection.SouthEast:
					return new Position(p.X + (p.Y % 2 == 0 ? 0 : 1), p.Y + 1);
				case MoveDirection.SouthWest:
					return new Position(p.X + (p.Y % 2 == 0 ? -1 : 0), p.Y + 1);
				default:
					return p;

			}
		}

        private static Unit TranslateUnit(this Unit unit, MoveDirection dir)
		{
			return new Unit
			{
				Pivot = Translate(unit.Pivot, dir),
				Members = unit.Members.Select(member => Translate(member, dir)).ToArray()
			};
		}

		private static Unit Translate(this Unit unit, int dx, int dy)
		{
			var pivot = new Position
			{
				X = unit.Pivot.X + dx,
				Y = unit.Pivot.Y + dy
			};
			return new Unit
			{
				Pivot = pivot,
				Members = unit.Members.Select(member =>
				new Position
				{
					X = member.X + dx,
					Y = member.Y + dy
				}
				).ToArray()
			};
		}

		public static Position Rotate(Position point, Position pivot, bool direction)
		{
			var relCol = point.X - pivot.X;
			var relRow = point.Y - pivot.Y;
			var odd = (pivot.Y % 2) != 0;

			var cube = ConvertToCube(odd, relCol, relRow);

			CubePosition rotated;

			if (direction)
			{
				rotated.X = -cube.Z;
				rotated.Y = -cube.X;
				rotated.Z = -cube.Y;
			}
			else
			{
				rotated.X = -cube.Y;
				rotated.Y = -cube.Z;
				rotated.Z = -cube.X;
			}

			var offsets = ConvertToOffset(odd, rotated);

			var r = new Position { X = pivot.X + offsets.X, Y = pivot.Y + offsets.Y };
			return r;
		}

		/// <summary>
		/// Special thanks for http://www.redblobgames.com/grids/hexagons/#conversions
		/// </summary>
		private static CubePosition ConvertToCube(bool odd, int col, int row)
		{
			var x = col - (row - (row & 1) * (odd ? -1 : 1)) / 2;
			var z = row;
			var y = -x - z;
			return new CubePosition(x, y, z);
		}

		private static Position ConvertToOffset(bool odd, CubePosition pos)
		{
			var col = pos.X + (pos.Z - (pos.Z & 1) * (odd ? -1 : 1)) / 2;
			var row = pos.Z;

			return new Position { X = col, Y = row };
		}

		private struct CubePosition
		{
			internal int X;
			internal int Y;
			internal int Z;

			internal CubePosition(int x, int y, int z)
			{
				X = x;
				Y = y;
				Z = z;
			}
		}
	}
}

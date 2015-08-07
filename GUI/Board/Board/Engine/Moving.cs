using System.Linq;
using Board.Entities;

namespace Board.Engine
{
	public static class Moving
	{
        public static Unit Translate(this Unit unit, MoveDirection direction)
		{
			switch (direction)
			{
				case MoveDirection.SouthEast:
					return Translate(unit, (unit.Pivot.Y % 2) == 0 ? 0 : 1, 1);
				case MoveDirection.SouthWest:
					return Translate(unit, (unit.Pivot.Y % 2) == 0 ? -1 : 0, 1);
				case MoveDirection.East:
					return Translate(unit, 1, 0);
				case MoveDirection.West:
					return Translate(unit, -1, 0);
				case MoveDirection.RotateClockwise:
					return Rotate(unit, 60);
				case MoveDirection.RotateCounterClockwise:
					return Rotate(unit, -60);
				default:
					return unit;
			}
		}

        public static Unit Rotate(this Unit unit, double degrees)
		{
			return new Unit
			{
				Pivot = unit.Pivot,
				Members = unit.Members.Select(member => Rotate(member, unit.Pivot, degrees)).ToArray()
			};
		}

        public static Unit Spawn(Input input, Unit unit)
		{
			var maxX = unit.Members.Max(t => t.X);
			int dx = (input.Width - maxX - 1)/2;
			return dx == 0 ? unit : Translate(unit, dx, 0);
		}

        public static Unit Translate(this Unit unit, int dx, int dy)
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

		public static Position Rotate(Position point, Position pivot, double degrees)
		{
			var relCol = point.X - pivot.X;
			var relRow = point.Y - pivot.Y;

			var x = relCol - (relRow + (relRow & 1)) / 2;
			var z = relRow;
			var y = -x-z;

			var nx = -z;
			var ny = -x;
			var nz = -y;

			var col = nx + (nz + (nz & 1)) / 2;
			var row = nz;

			var r = new Position { X = pivot.X + col, Y = pivot.Y + row };
			return r;
		}
	}
}

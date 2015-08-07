using Board.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Engine
{
	internal static class Moving
	{
		internal static Unit Translate(Unit unit, MoveDirection direction)
		{
			switch (direction)
			{
				case MoveDirection.SE:
					return Translate(unit, (unit.Pivot.Y % 2) == 0 ? 0 : 1, 1);
				case MoveDirection.SW:
					return Translate(unit, (unit.Pivot.Y % 2) == 0 ? -1 : 0, 1);
				case MoveDirection.E:
					return Translate(unit, 1, 0);
				case MoveDirection.W:
					return Translate(unit, -1, 0);
				default:
					return unit;
			}
		}

		internal static Unit Rotate(Unit unit, double degrees)
		{
			return new Unit
			{
				Pivot = unit.Pivot,
				Members = unit.Members.Select(member => Rotate(member, unit.Pivot, degrees)).ToArray()
			};
		}

		private static Unit Translate(Unit unit, int dx, int dy)
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

		private static Position Rotate(Position point, Position pivot, double degrees)
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

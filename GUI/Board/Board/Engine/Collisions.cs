using Board.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Engine
{
	internal static class Collisions
	{
		internal static CollisionType GetCollision(Input input, Unit unit)
		{
			var filledHash = new HashSet<Position>(input.Filled);

			foreach (var member in unit.Members)
			{
				if (member.X < 0 || member.Y < 0 || member.X >= input.Width || member.Y >= input.Height)
				{
					return CollisionType.Background;
				}
				if (filledHash.Contains(member))
				{
					return CollisionType.Filled;
				}
			}
			return CollisionType.None;
		}
	}
}

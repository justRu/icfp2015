namespace Solver
{
	internal static class Collisions
	{
		internal static CollisionType GetCollision(Field field, Unit unit)
		{
			foreach (var member in unit.Members)
			{
				if (member.X < 0 || member.Y < 0 || member.X >= field.Width || member.Y >= field.Height)
				{
					return CollisionType.Edge;
				}
				if (field[member.X, member.Y])
				{
					return CollisionType.Filled;
				}
			}
			return CollisionType.None;
		}
	}
}

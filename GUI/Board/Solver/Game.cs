namespace Solver
{
	public static class Game
	{
		public static Snapshot MakeMove(Snapshot prevSnapshot, MoveDirection move)
		{
			var snapshot = new Snapshot(prevSnapshot); // clone
			var unit = snapshot.CurrentUnit.Translate(move);
			if (Collisions.GetCollision(snapshot.Field, unit) != CollisionType.None)
			{
				LockUnit(snapshot, snapshot.CurrentUnit);
			}
			else
			{
				snapshot.CurrentUnit = unit;
			}
			return snapshot;
		}

		private static void LockUnit(Snapshot snapshot, Unit unit)
		{
			foreach (var pos in unit.Members)
			{
				snapshot.Field[pos.X, pos.Y] = true; // lock position
			}
			// TODO: clear row, update cleared rows count, move rows down!
			snapshot.UnitIndex++;
			if (snapshot.UnitIndex >= snapshot.UnitsQueue.MaxUnits)
			{
				snapshot.Finished = true;
			}
			else
			{
				var nextUnit = snapshot.Field.Spawn(snapshot.UnitsQueue.Get(snapshot.UnitIndex));
				if (Collisions.GetCollision(snapshot.Field, nextUnit) != CollisionType.None)
				{
					snapshot.Finished = true;
				}
				else
				{
					snapshot.CurrentUnit = nextUnit;
				}
			}
		}
	}
}
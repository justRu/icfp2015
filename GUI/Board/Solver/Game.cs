using System.Collections.Generic;
using System.Linq;

namespace Solver
{
    public static class Game
    {
		/// <returns>null if move is illegal, otherwise snapshot with new game state</returns>
		public static Snapshot MakeMove(Snapshot prevSnapshot, MoveDirection move)
        {
            var snapshot = new Snapshot(prevSnapshot); // clone
            var unit = snapshot.CurrentUnit.Translate(move);
			if (snapshot.UnitHistory.Any(u => unit.Equals(u)))
				return null;
            if (Collisions.GetCollision(snapshot.Field, unit) != CollisionType.None)
            {
                LockUnit(snapshot, snapshot.CurrentUnit);
            }
            else
            {
				snapshot.UnitHistory.Add(snapshot.CurrentUnit);
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
			snapshot = DeleteLines(snapshot, unit);
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
					snapshot.UnitHistory.Clear();
                }
            }
        }

        // TODO: implement
        private static Snapshot DeleteLines(Snapshot snapshot, Unit lockedUnit)
        {
            var filled = new List<int>();
            for (int i = lockedUnit.GetMinY(); i <= lockedUnit.GetMaxY(); i++)
            {
                if (snapshot.Field.IsLineFull(i))
                    filled.Add(i);
            }
            if (filled.Any())
            {
                const int size = sizeof (byte);
                var cleared = snapshot.Field
                    .GetEnumerable()
                    .Select((val, i) => new {val, i})
                    .GroupBy((pair) => pair.i/size)
                    .Where((group, i) => !filled.Contains(i))
                    .SelectMany(group => group.Select(pair => pair.val));
                snapshot.Field.SaveEnumerable(Enumerable.Repeat(true, filled.Count*size).Concat(cleared));
            }
            // TODO: update score
            // TOOD: save prev deleted lines to multiply score
            return snapshot;
        }


    }
}
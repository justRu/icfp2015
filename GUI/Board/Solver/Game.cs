using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace Solver
{
    public static class Game
    {
		/// <returns>null if move is illegal, otherwise snapshot with new game state</returns>
		public static Snapshot MakeMove(Snapshot prevSnapshot, MoveDirection move)
        {
            var snapshot = new Snapshot(prevSnapshot); // clone
            var unit = snapshot.CurrentUnit.Translate(move);
			if (snapshot.UnitHistory.Any(u => unit.Equals(u)) || unit.Equals(snapshot.CurrentUnit))
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
	        var field = snapshot.Field;
            var filled = new List<int>();
            for (int y = lockedUnit.GetMinY(); y <= lockedUnit.GetMaxY(); y++)
            {
                if (snapshot.Field.IsLineFull(y))
                    filled.Add(y);
            }
	        if (filled.Count > 0)
	        {
		        int removed = 0;
		        foreach (var row in filled.OrderByDescending(i => i))
		        {
					// from this row up shift all rows down
			        for (int y = row + removed - 1; y >= 0; y--)
			        {
						for (int x = 0; x < field.Width; x++)
						{
							field[x, y+1] = field[x, y];
						}
			        }
			        removed++;
		        }
	        }
			snapshot.Score += GetMoveScore(snapshot, lockedUnit, filled.Count);
			snapshot.PrevUnitClearedLines = filled.Count;
            return snapshot;
        }

		private static int GetMoveScore(Snapshot snapshot, Unit lockedUnit, int linesFilled)
	    {
			int points = lockedUnit.Members.Length + 100 * (1 + linesFilled) * linesFilled / 2;
			int lineBonus = snapshot.PrevUnitClearedLines == 0
				? 0
				: (int)Math.Floor((snapshot.PrevUnitClearedLines - 1) * points / (double)10);
			return points + lineBonus;
	    }
    }
}
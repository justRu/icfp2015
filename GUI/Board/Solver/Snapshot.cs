namespace Solver
{
	public sealed class Snapshot
	{
		public Field Field { get; set; }

		public Unit CurrentUnit { get; set; }

		public UnitsQueue UnitsQueue { get; set; }

		public int Score { get; set; }

		public bool Finished { get; set; }

		public int UnitIndex { get; set; }

		private Snapshot(Snapshot other)
		{
			CurrentUnit = other.CurrentUnit;
			Field = new Field(other.Field);
			Finished = other.Finished;
			Score = other.Score;
			UnitIndex = other.UnitIndex;
			// TODO: move index from queue to this class
			UnitsQueue = other.UnitsQueue;
		}

		public Snapshot(Input input, uint seed)
		{
			Field = new Field(input);
			UnitsQueue = new UnitsQueue(input.Units, seed, input.SourceLength);
			CurrentUnit = Field.Spawn(UnitsQueue.Get(0));
			Score = 0;
		}

		public Snapshot New(MoveDirection move)
		{
			var snapshot = new Snapshot(this); // clone
			var unit = CurrentUnit.Translate(move);
			if (Collisions.GetCollision(Field, unit) != CollisionType.None)
			{
				LockUnit(snapshot, CurrentUnit);
			}
			else
			{
				snapshot.CurrentUnit = unit;
			}
			return snapshot;
		}

		private void LockUnit(Snapshot snapshot, Unit unit)
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
				var nextUnit = Field.Spawn(UnitsQueue.Get(snapshot.UnitIndex));
				if (Collisions.GetCollision(Field, nextUnit) != CollisionType.None)
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
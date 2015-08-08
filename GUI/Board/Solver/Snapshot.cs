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

		/// <summary>
		/// Initial constructor.
		/// </summary>
		public Snapshot(Input input, uint seed)
		{
			Field = new Field(input);
			UnitsQueue = new UnitsQueue(input.Units, seed, input.SourceLength);
			CurrentUnit = Field.Spawn(UnitsQueue.Get(0));
			Score = 0;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		internal Snapshot(Snapshot other)
		{
			CurrentUnit = other.CurrentUnit;
			Field = new Field(other.Field);
			Finished = other.Finished;
			Score = other.Score;
			UnitIndex = other.UnitIndex;
			// TODO: move index from queue to this class
			UnitsQueue = other.UnitsQueue;
		}
	}
}
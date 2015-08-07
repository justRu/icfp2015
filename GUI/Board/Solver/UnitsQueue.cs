namespace Solver
{
	public sealed class UnitsQueue
	{
		public Unit[] AllUnits { get; set; }

		public uint Seed { get; set; }

		public int MaxUnits { get; set; }

		public UnitsQueue(Unit[] units, uint seed, int maxUnits)
		{
			AllUnits = units;
			Seed = seed;
			MaxUnits = maxUnits;
		}

		public Unit Get(int index)
		{
			return Lcg.Get(AllUnits, Seed, index);
		}
	}
}
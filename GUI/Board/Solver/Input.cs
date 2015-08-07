namespace Solver
{
	public class Input
	{
		public int Height { get; set; }

		public int Width { get; set; }

		public Unit[] Units { get; set; }

		public Position[] Filled { get; set; }

		public int SourceLength { get; set; }

		public uint[] SourceSeeds { get; set; }
	}
}

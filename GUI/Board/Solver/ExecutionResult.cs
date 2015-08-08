namespace Solver
{
	public sealed class ExecutionResult
	{
		public MoveDirection[] Commands { get; set; }

		public double Estimation { get; set; }

		public Snapshot Snapshot { get; set; }

		public Unit CurrentUnit { get; set; }
	}
}
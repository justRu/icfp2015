namespace Solver
{
	public sealed class ExecutionResult
	{
		public MoveDirection[] Commands { get; set; }

		public double Estimate { get; set; }

		public Snapshot Snapshot { get; set; }
	}
}
namespace Solver
{
	public sealed class ExecutionRequest
	{
		public Snapshot Snapshot { get; set; }

		public ExecutionOptions Options { get; set; }

		public MoveDirection[] PreviousMoves { get; set; }
	}
}
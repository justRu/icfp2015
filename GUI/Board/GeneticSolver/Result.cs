using Solver;

namespace GeneticSolver
{
	internal sealed class Result
	{
		public ExecutionOptions Options { get; set; }

		public double Score { get; set; }

		public MoveDirection[] Commands { get; set; }

		public int UnitIndex { get; set; }
	}
}
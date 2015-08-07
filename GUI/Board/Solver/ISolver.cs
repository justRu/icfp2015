namespace Solver
{
	public interface ISolver
	{
		ExecutionResult[] Solve(ExecutionRequest request);
	}
}
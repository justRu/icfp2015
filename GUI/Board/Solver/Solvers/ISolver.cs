using System.Collections.Generic;
namespace Solver
{
	public interface ISolver
	{
		IEnumerable<ExecutionResult> Solve(ExecutionRequest request);
	}
}
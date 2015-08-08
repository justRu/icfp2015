using System;
using System.Collections.Generic;
using System.Linq;

namespace Solver
{
	public class SerialSolver : ISolver
    {
	    public ExecutionResult[] Solve(ExecutionRequest request)
	    {
		    return Calculate(request.Snapshot, null, request.Options.MaxWidth, request.Options.MaxHeight).ToArray();
	    }

	    private static IEnumerable<ExecutionResult> Calculate(
			Snapshot baseSnapshot, MoveDirection? move, int maxWidth, int depth)
	    {
			var snapshot = move.HasValue ? baseSnapshot.New(move.Value) : baseSnapshot;
			// stop recursion
			if (snapshot.Finished || depth <= 0)
		    {
			    yield return new ExecutionResult
			    {
				    Commands = new MoveDirection[0],
				    Estimation = SnapshotEvaluate(snapshot)
			    };
				yield break;
		    }
			// TODO: check for illegal moves (e.g. E-W)
			var candidateMoves = (MoveDirection[])Enum.GetValues(typeof (MoveDirection));
			var childResults = candidateMoves
				.SelectMany(m => Calculate(snapshot, m, maxWidth, depth - 1)) // recursion
				.OrderByDescending(r => r.Estimation)
				.Take(maxWidth);
		    foreach (var result in childResults)
		    {
				var estimate = SnapshotEvaluate(snapshot);
				yield return new ExecutionResult
				{
					Commands = move.HasValue
						? result.Commands.Prepend(move.Value).ToArray()
						: result.Commands,
					Estimation = estimate,
				};
		    }
	    }

	    private static double SnapshotEvaluate(Snapshot snapshot, params MoveDirection[] nextMoves)
	    {
		    int depth = snapshot.Field.Height - snapshot.CurrentUnit.Members.Min(p => p.Y);
			return snapshot.Score - depth + nextMoves.Length;
	    }
    }
}

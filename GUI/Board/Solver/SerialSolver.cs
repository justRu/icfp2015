using System;
using System.Collections.Generic;
using System.Linq;

namespace Solver
{
	public class SerialSolver : ISolver
    {
		public IEnumerable<ExecutionResult> Solve(ExecutionRequest request)
		{
			return Calculate(
				request.Snapshot,
				null,
				request.Options.MaxWidth,
				request.Options.MaxHeight,
				request.Options.MinEstimation);
		}

	    private static IEnumerable<ExecutionResult> Calculate(
			Snapshot baseSnapshot, MoveDirection? move, int maxWidth, int depth, double minEstimate)
	    {
			var snapshot = move.HasValue ? baseSnapshot.New(move.Value) : baseSnapshot;
            var estimate = SnapshotEvaluate(snapshot);
            // stop recursion
            if (snapshot.Finished || depth <= 0 || estimate < minEstimate) // TODO: some diff
		    {
			    yield return new ExecutionResult
			    {
				    Commands = new MoveDirection[0],
				    Estimation = estimate,
					Snapshot = snapshot
			    };
				yield break;
		    }

            // TODO: check for illegal moves (e.g. E-W)
            var candidateMoves = (MoveDirection[])Enum.GetValues(typeof (MoveDirection));
			var childResults = candidateMoves
				.SelectMany(m => Calculate(snapshot, m, maxWidth, depth - 1, estimate)) // recursion
				.OrderByDescending(r => r.Estimation)
				.Take(maxWidth);
		    foreach (var result in childResults)
		    {
				
				yield return new ExecutionResult
				{
					Commands = move.HasValue
						? result.Commands.Prepend(move.Value).ToArray()
						: result.Commands,
					Estimation = result.Estimation, // TODO: combine somehow?
					Snapshot = result.Snapshot
				};
		    }
	    }

	    private static double SnapshotEvaluate(Snapshot snapshot, params MoveDirection[] nextMoves)
	    {
            int minX = snapshot.CurrentUnit.Members.Min(p => p.X);
            int maxX = snapshot.CurrentUnit.Members.Max(p => p.X);
            int minY = snapshot.CurrentUnit.Members.Min(p => p.Y);
            int maxY = snapshot.CurrentUnit.Members.Max(p => p.Y);
            int depth = snapshot.Field.Height - minY;

            int width = maxX - maxY;
            int marginBottom = snapshot.Field.Height - maxY;

            int center = (maxX + minX) / 2;

            int centerPenalty = 0;
            // TODO: get max height of filled cells
            if (snapshot.Field.Width - maxX < marginBottom || minX < marginBottom)
            {
                centerPenalty = Math.Abs(center - snapshot.Field.Width / 2) * 3;
            }

			return snapshot.Score - depth - centerPenalty;
	    }
    }
}

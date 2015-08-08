using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;

namespace Solver
{
	public class SearchSolver : ISolver
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
			Snapshot baseSnapshot,
			MoveDirection? move,
			int maxWidth,
			int depth,
			double minEstimate)
	    {
			var snapshot = move.HasValue
				? Game.MakeMove(baseSnapshot, move.Value)
				: baseSnapshot;
			if (snapshot == null) // illegal move
				yield break;
            var estimate = SnapshotEvaluate(snapshot);
            // stop recursion
            if (snapshot.Finished || depth <= 0 || estimate < minEstimate) // TODO: some diff
		    {
			    yield return new ExecutionResult
			    {
				    Commands = new [] { move.Value },
				    Estimate = estimate,
					Snapshot = snapshot
			    };
				yield break;
		    }
            var candidateMoves = (MoveDirection[])Enum.GetValues(typeof (MoveDirection));
			var childResults = candidateMoves
				.SelectMany(m => Calculate(snapshot, m, maxWidth, depth - 1, estimate)) // recursion
				.OrderByDescending(r => r.Estimate)
				.Take(maxWidth);
		    foreach (var result in childResults)
		    {
				yield return new ExecutionResult
				{
					Commands = move.HasValue
						? result.Commands.Prepend(move.Value).ToArray()
						: result.Commands,
					Estimate = result.Estimate, // TODO: combine somehow?
					Snapshot = result.Snapshot
				};
		    }
	    }

	    private static double SnapshotEvaluate(Snapshot snapshot, params MoveDirection[] nextMoves)
	    {
            int minX = snapshot.CurrentUnit.GetMinX();
			int maxX = snapshot.CurrentUnit.GetMaxX();
            int minY = snapshot.CurrentUnit.GetMinY();
            int maxY = snapshot.CurrentUnit.GetMaxY();
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

			// TODO: calculate the number of adjacent edges

			return snapshot.Score - depth - centerPenalty;
	    }
    }
}

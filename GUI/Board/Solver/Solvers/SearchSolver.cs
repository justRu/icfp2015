using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Threading.Tasks;

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
            
			bool nextUnit = snapshot.UnitIndex > baseSnapshot.UnitIndex;
			// stop recursion
			if (snapshot.Finished || depth <= 0 || estimate < minEstimate || nextUnit) // TODO: some diff
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

		    IEnumerable<ExecutionResult> childResults;
			//if (depth >= 8)
			//{
			//	var tasks = candidateMoves
			//		.Select(m => Task.Run(() => Calculate(snapshot, m, maxWidth, depth - 1, estimate)))
			//		.ToArray();

			//	Task.WaitAll(tasks);
			//	childResults = tasks.SelectMany(t => t.Result);

			//}
			//else
		    {
				childResults = candidateMoves
					.SelectMany(m => Calculate(snapshot, m, maxWidth, depth - 1, estimate)); // recursion;
		    }
			childResults = childResults.OrderByDescending(r => r.Estimate).Take(maxWidth);
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
			return snapshot.Score - GetHiddenHoles(snapshot.Field) * 20
				+ GetUnitPositionBonus(snapshot.Field, snapshot.CurrentUnit);
	    }

		private static double GetUnitPositionBonus(Field field, Unit unit)
		{
			int minX = unit.GetMinX();
			int maxX = unit.GetMaxX();
			int minY = unit.GetMinY();
			int maxY = unit.GetMaxY();
			int depth = field.Height - minY;

			int width = maxX - maxY;

			int marginBottom = field.Height - maxY;

			int center = (maxX + minX) / 2;

			var attractor = GetBottomOpenPosition(field);

			double attractorDistance = attractor.DistanceTo(unit.Pivot);
			double attractorPenalty = attractorDistance < 4
				? 0
				: attractor.DistanceTo(unit.Pivot);

			int centerPenalty = 0;
			// TODO: get max height of filled cells
			//if (field.Width - maxX < marginBottom || minX < marginBottom)
			//{
			//	centerPenalty = Math.Abs(center - field.Width / 2) ;
			//}

			// TODO: calculate the number of adjacent cells
			int adjacencyBonus = 0; // TODO: get max height of filled cells
			if ((maxY == field.Height - 1))
			{
				adjacencyBonus += 3;
			}

			// TODO: check if maxY members touch edge or filled cell
			foreach (var position in unit.Members)
			{
				if (position.X == 0 || position.X == field.Width - 1)
					adjacencyBonus += 3;
			}

			return adjacencyBonus - depth * 5 - centerPenalty * 1 - attractorPenalty * 10;
		}

		private static Position GetBottomOpenPosition(Field field)
		{
			Position min = new Position(-1, field.Height);
			for (int x = 0; x < field.Width; x++)
			{
				int y = 0;
				while (y < field.Height && !field[x, y])
				{
					y++;
				}
				if (y <= min.Y)
				{
					min = new Position(x, y);
				}
			}
			return min;
		}

		private static double GetHiddenHoles(Field field)
		{
			int result = 0;
			for (int y = 0; y < field.Height - 1; y++)
			{
				for (int x = 0; x < field.Width - 1; x++)
				{
					if (field[x, y] && field[x + 1, y])
					{
						var leftUnit = new Unit {Members = new[] {new Position(x, y)}};
						var pos = leftUnit.Translate(MoveDirection.SouthEast).Members[0];
						if (!field[pos.X, pos.Y])
							result ++;
					}
				}
			}
			return result;
		}
    }
}

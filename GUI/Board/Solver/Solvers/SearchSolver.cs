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
				request.Options,
				(int)Math.Ceiling(request.Options.MaxHeight),
				request.Options.MinEstimation);
		}

	    private static IEnumerable<ExecutionResult> Calculate(
			Snapshot baseSnapshot,
			MoveDirection? move,
			ExecutionOptions options,
			int depth,
			double minEstimate)
	    {
			var snapshot = move.HasValue
				? Game.MakeMove(baseSnapshot, move.Value)
				: baseSnapshot;
			if (snapshot == null) // illegal move
				yield break;
			var estimate = SnapshotEvaluate(snapshot, options);
            
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

		    var childResults = candidateMoves
			    .SelectMany(m => Calculate(snapshot, m, options, depth - 1, estimate))
				.OrderByDescending(r => r.Estimate)
				.Take((int)Math.Ceiling(options.MaxWidth));
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

	    private static double SnapshotEvaluate(Snapshot snapshot, ExecutionOptions options)
	    {
			return snapshot.Score
				+ GetFieldEstimate(snapshot.Field, options)
				+ GetUnitPositionBonus(snapshot.Field, snapshot.CurrentUnit, options);
	    }

		private static double GetUnitPositionBonus(
			Field field, Unit unit, ExecutionOptions options)
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

			double attractorDistance = Math.Abs(attractor.X - unit.Pivot.X); // attractor.DistanceTo(unit.Pivot);

			double depthPenalty = 0;
			foreach (var member in unit.Members)
			{
				depthPenalty += field.Height - 1 - member.Y;
			}

			int edgeBonus = 0;
			foreach (var position in unit.Members)
			{
				if (position.X == 0 || position.X == field.Width - 1)
					edgeBonus += 1;
			}

			double adjacencyBonus = GetAdjacencyBonus(field, unit, options);

			return edgeBonus * options.EdgeRatio
				+ adjacencyBonus
				- depthPenalty * options.DepthPenaltyRatio
				- attractorDistance * options.AttractorRatio;
		}

		private static double GetAdjacencyBonus(Field field, Unit unit, ExecutionOptions options)
		{
			double downBonus = options.AdjacencyDownRatio;
			double sideBonus = options.AdjacencySideRatio;
			double result = 0;
			foreach (var m in unit.Members)
			{
				if (m.Y < field.Height - 1)
				{
					if (field[m.Translate(MoveDirection.SouthEast)])
					{
						result += downBonus;
					}
					else
					{
						result -= downBonus;
					}
					if (field[m.Translate(MoveDirection.SouthWest)])
					{
						result += downBonus;
					}
					else
					{
						result -= downBonus;
					}
				}
				if (m.Y < field.Width - 1 && field[m.Translate(MoveDirection.East)])
					result += sideBonus;
				if (m.Y > 0 && field[m.Translate(MoveDirection.West)])
					result += sideBonus;
			}
			return result;
		}

		private static Position GetBottomOpenPosition(Field field)
		{
			Position min = new Position(-1, field.Height);
			for (int x = 0; x < field.Width; x++)
			{
				int y = 0;
				// drop down until cells is empty
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

		private static double GetFieldEstimate(Field field, ExecutionOptions options)
		{
			return CornerCellsBonus(field) * options.CornerCellsBonus
				- GetHiddenHoles(field) * options.HiddenHolesPenalty;
		}

		private static double CornerCellsBonus(Field field)
		{
			double result = 0;
			if (field[field.Width - 1, field.Height - 1])
				result += 1;
			if (field[0, field.Height - 1])
				result += 1;
			return result;
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
						var underCell = new Position(x, y).Translate(MoveDirection.SouthEast);
						if (!field[underCell])
							result++;
					}
				}
			}
			return result;
		}
    }
}

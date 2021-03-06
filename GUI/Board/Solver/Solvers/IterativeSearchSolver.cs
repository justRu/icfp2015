﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Solver
{
	public sealed class IterativeSearchSolver : ISolver
	{
		public int MaxIterations { get; set; }

		public IterativeSearchSolver(int maxIterations)
		{
			MaxIterations = maxIterations;
		}

		public IEnumerable<ExecutionResult> Solve(ExecutionRequest request)
		{
			var moves = new List<MoveDirection>();
			ExecutionResult best = null;
			for (int i = 0; i < MaxIterations; i++) // while (true)
			{
				best = new SearchSolver().Solve(request).First();
				moves.AddRange(best.Commands);
				// select best move and move from it
				if (best.Snapshot.Finished)
				{
					break;
				};
				request = new ExecutionRequest
				{
					Snapshot = best.Snapshot,
					Options = request.Options
				};
				request.Options.MinEstimation = double.MinValue;
			}
			return new[]
			{
				new ExecutionResult
				{
					Snapshot = best.Snapshot,
					Commands = moves.ToArray(),
					Estimate = best.Estimate
				}
			};
		}
	}
}
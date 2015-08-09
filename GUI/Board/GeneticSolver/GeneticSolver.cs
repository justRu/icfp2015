using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Solver;

namespace GeneticSolver
{
	internal static class GeneticSolver
	{
		public static readonly Random Random = new Random();

		public static Result Run(Snapshot snapshot)
		{
			int iterations = 3;
			int populationSize = 12;
			int bestSize = 6;
			double mutationPercent = 0.3;

			Result globalBest = null;
			// Initialize population:
			var population = Enumerable.Range(0, populationSize)
				.Select(_ => Generate())
				.ToList();
			for (int i = 0; i < iterations; i++)
			{
				mutationPercent = mutationPercent * (iterations - i) / iterations;
				Console.WriteLine("========= Generation #" + i);
				var tasks = population
					.ToDictionary(o => o, o => Task.Run(() => Calculate(snapshot, o)));
				Task.WaitAll(tasks.Values.ToArray());

				var results = tasks.Select(
					item => new Result
					{
						Options = item.Key,
						Score = item.Value.Result.Snapshot.Score,
						UnitIndex = item.Value.Result.Snapshot.UnitIndex,
						Commands = item.Value.Result.Commands
					}).ToArray();

				int pos = 0;
				foreach (var result in results.OrderByDescending(r => r.Score))
				{
					++pos;
					PrintResult(pos, result);
				}

				var bestResults = results
					.OrderByDescending(r => r.Score).Take(bestSize)
					.ToArray();

				if (globalBest == null || globalBest.Score < bestResults.First().Score)
				{
					globalBest = bestResults.First();
				}
				
				population = new List<ExecutionOptions>(bestResults.Select(r => r.Options));
			
				for (int j = 0; j < populationSize - bestSize; j++)
				{
					int firstIndex;
					int secondIndex;
					do
					{
						// TODO: carousel?
						firstIndex = Random.Next(0, bestSize);
						secondIndex = Random.Next(0, bestSize);
					} while (firstIndex == secondIndex);
					var child = Crossover(population[firstIndex], population[secondIndex]);
					population.Add(child);
				}
				foreach (var options in population)
				{
					Mutate(options, mutationPercent);
				}
			}
			return globalBest;
		}

		private static void PrintResult(int pos, Result result)
		{
			Console.WriteLine("#{0} Score: {1}, Units: {2}, Commands: {3}",
				pos,
				result.Score,
				result.UnitIndex,
				result.Commands.Length);
		}

		private static ExecutionResult Calculate(Snapshot snapshot, ExecutionOptions options)
		{
			var request = new ExecutionRequest
			{
				Snapshot = snapshot,
				Options = options
			};
			var solver = new IterativeSearchSolver(1200); //new TraverseSolver();
			return solver.Solve(request).First();
		}

		private static void Mutate(ExecutionOptions options, double percent)
		{
			var propsDict = typeof(ExecutionOptions).GetProperties().ToDictionary(p => p.Name);
			foreach (var variable in Variables)
			{
				var property = propsDict[variable.Key];
				double oldValue = (double)property.GetValue(options);
				double newValue = variable.Value.Mutate(oldValue, percent);
				property.SetValue(options, newValue);
			}
		}

		private static ExecutionOptions Crossover(
			ExecutionOptions first, ExecutionOptions second)
		{
			var result = new ExecutionOptions { MinEstimation = Double.MinValue };
			var props = typeof(ExecutionOptions)
				.GetProperties()
				.ToDictionary(p => p.Name);
			foreach (var variable in Variables)
			{
				var property = props[variable.Key];
				double value = Random.Next(2) == 0
					? (double) property.GetValue(first)
					: (double) property.GetValue(second);
				//double firstValue = (double)property.GetValue(first);
				//double secondValue = (double)property.GetValue(second);
				//double mean = (firstValue + secondValue) / 2;
				property.SetValue(result, value);
			}
			return result;
		}

		private static ExecutionOptions Generate()
		{
			var result = new ExecutionOptions
			{
				MinEstimation = Double.MinValue
			};
			var props = typeof(ExecutionOptions)
				.GetProperties()
				.ToDictionary(p => p.Name);
			foreach (var variable in Variables)
			{
				var property = props[variable.Key];
				double value = variable.Value.GetRandomValue();
				property.SetValue(result, value);
			}
			return result;
		}

		class Variable
		{
			private static readonly Random Random = new Random();

			public double Min { get; set; }
			public double Max { get; set; }

			public Variable(double min, double max)
			{
				if (max < min)
					throw new ArgumentException();
				Max = max;
				Min = min;
			}

			public double Mutate(double value, double percent)
			{
				double sign = Random.Next(0, 2) == 1 ? +1 : -1;
				double newValue = value + sign * percent * (Max - Min);
				if (newValue < Min)
					return Min;
				if (newValue > Max)
					return Max;
				return newValue;
			}

			public double GetRandomValue()
			{
				return Min + Random.NextDouble() * (Max - Min);
			}
		}

		private static readonly Dictionary<string, Variable> Variables = new Dictionary<string, Variable>
		{
			{"MaxWidth", new Variable(1.5, 6)}, // use with ceiling
			{"MaxHeight", new Variable(1.5, 8)}, // use with ceiling
			{"AttractorRatio", new Variable(0, 20)},
			{"DepthPenaltyRatio", new Variable(0, 20)},
			{"HiddenHolesPenalty", new Variable(0, 100)},
			{"AdjacencyDownRatio", new Variable(2, 15)},
			{"AdjacencySideRatio", new Variable(1, 7)},
			{"EdgeRatio", new Variable(0, 20)},
			{"CornerCellsBonus", new Variable(0, 100)}
		}; 
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Board.Helpers;
using Newtonsoft.Json;
using Solver;

namespace GeneticSolver
{
	class Program
	{
		static void Main(string[] args)
		{
			int[] numberToSolve = args.Length == 1
				? new[] {Convert.ToInt32(args[0])}
				: Enumerable.Range(0, 25).ToArray();

			var outputs = new List<Output>();

			foreach (var number in numberToSolve)
			{
				var file = string.Format(@"inputs\problem_{0}.json", number);
				Console.WriteLine("Processing file " + file);
				var input = JsonConvert.DeserializeObject<Input>(File.ReadAllText(file));
				Console.WriteLine("Seeds: " + input.SourceSeeds.Length);
				foreach (var seed in input.SourceSeeds)
				{
					Console.WriteLine("Seed: " + seed);
					Console.WriteLine("-----------------");
					var snapshot = new Snapshot(input, seed);
					var result = GeneticSolver.Run(snapshot);
					Console.WriteLine("Score: " + result.Score);
					Console.WriteLine("Options:");
					Console.WriteLine(JsonConvert.SerializeObject(result.Options));
					Console.WriteLine("Commands:");
					//Console.WriteLine(string.Join("", result.Commands.Select(c => ((int)c).ToString())));
					HashSet<string> usedWords;
					var commandsStr = CommandEncoding.Encode(result.Commands, out usedWords);
					Console.WriteLine("Different words: " + string.Join(", ", usedWords));
					Console.WriteLine(commandsStr);
					
					outputs.Add(new Output
					{
						Commands = commandsStr,
						ProblemId = number,
						Seed = seed
					});
				}
				Console.WriteLine("__________________");
			}
			File.WriteAllText("solution.json", JsonConvert.SerializeObject(outputs.ToArray()));
			Console.WriteLine("Press ENTER to exit...");
			Console.ReadLine();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solver;

namespace Board.Helpers
{
	public static class CommandEncoding
	{
		private static readonly Dictionary<MoveDirection, char[]> MovesChars;
		private static readonly Dictionary<string, MoveDirection[]> TranslatedPhrases;

		static CommandEncoding()
		{
			MovesChars = new Dictionary<MoveDirection, char[]>
			{
				{MoveDirection.West, new[] {'p', '\'', '!', '.', '0', '3'}},
				{MoveDirection.East, new[] {'b', 'c', 'e', 'f', 'y', '2'}},
				{MoveDirection.SouthWest, new[] {'a', 'g', 'h', 'i', 'j', '4'}},
				{MoveDirection.SouthEast, new[] {'l', 'm', 'n', 'o', ' ', '5'}},
				{MoveDirection.RotateClockwise, new[] {'d', 'q', 'r', 'v', 'z', '1'}},
				{MoveDirection.RotateCounterClockwise, new[] {'k', 's', 't', 'u', 'w', 'x'}},
				//\t, \n, \r (ignored)
			};
			var reverseDict = Reverse(MovesChars);
			var powerPhrases = new[] { "Ei!" };
			TranslatedPhrases = powerPhrases.ToDictionary(
				word => word,
				word => word.Select(ch => reverseDict[char.ToLowerInvariant(ch)]).ToArray());

		}

		public static string Encode(MoveDirection[] moves)
		{
			var sb = new StringBuilder(moves.Length);
			for (int i = 0; i < moves.Length;)
			{
				string word;
				if (TryFindWord(moves, i, out word))
				{
					sb.Append(word);
					i += word.Length;
				}
				else
				{
					sb.Append(MovesChars[moves[i]].First()); // maybe random?
					i++;
				}
			}
			return sb.ToString();
		}

		private static bool TryFindWord(MoveDirection[] moves, int startIndex, out string word)
		{
			foreach (var pair in TranslatedPhrases.OrderByDescending(d => d.Key.Length))
			{
				if (moves.Skip(startIndex).Take(pair.Value.Length).SequenceEqual(pair.Value))
				{
					word = pair.Key;
					return true;
				}
			}
			word = null;
			return false;
		}


		private static Dictionary<T2, T1> Reverse<T1, T2>(Dictionary<T1, T2[]> dict)
		{
			return dict
				.SelectMany(pair => pair.Value.Select(v => Tuple.Create(v, pair.Key)))
				.ToDictionary(t => t.Item1, t => t.Item2);
		}
	}
}

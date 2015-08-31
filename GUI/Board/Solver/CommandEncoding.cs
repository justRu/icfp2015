using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solver;

namespace Board.Helpers
{
	public static class CommandEncoding
	{
		internal class Phrase
		{
			internal int MovesLen;
			internal int KeyLen;

			internal string Key;
			internal MoveDirection[] Moves;
		}

		private static readonly Dictionary<MoveDirection, char[]> MovesChars;
		private static readonly Dictionary<string, MoveDirection[]> TranslatedPhrases;

		private static readonly Phrase[] TranslatedPhrasesContainer;

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
			var powerPhrases = new[]
			{
				"bigboote",
				"big booty",
				"bigbooty",
				"bigboo tay",
				"bigbootay",

				// From levels:
				"aleister",
				"yuggoth",
				"ia! ia!",
				"ei!", // YES
				"r'lyeh", // YES

				"miskatonic",
				"the deep ones",
				"deep ones",
				"the old ones",
				"old ones",
				//"AS2H2", // NO
				"the turing squad",
				"turing squad",
				"cthulhu",
				"cthulhu cthulhu",
				"cthulhu cthulhu cthulhu",
				"In his house at R'lyeh dead Cthulhu waits dreaming",
				"dead cthulhu waits dreaming",
				"strong ai golem",
				"blue blaze irregular",
				"digital dark arts",

				"2015",
				"12345",
				"icfp2015",
				"icfp 2015",
			};
			TranslatedPhrases = powerPhrases.ToDictionary(
				word => word,
				word => word.Select(ch => reverseDict[char.ToLowerInvariant(ch)]).ToArray());

			TranslatedPhrasesContainer = powerPhrases.Select(word => new Phrase
				{
					MovesLen = word.Select(ch => reverseDict[char.ToLowerInvariant(ch)]).ToArray().Length,
					KeyLen = word.Length,
					Key= word,
					Moves = word.Select(ch => reverseDict[char.ToLowerInvariant(ch)]).ToArray()
				}).ToArray();
		}

		public static string Encode(MoveDirection[] moves, out HashSet<string> usedWords)
		{
			usedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var sb = new StringBuilder(moves.Length);
			for (int i = 0; i < moves.Length;)
			{
				string word;
				if (TryFindWord(moves, i, out word))
				{
					sb.Append(word);
					usedWords.Add(word);
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

		public static int GetWordsPower(MoveDirection[] moves)
		{
			HashSet<string> words;
			Encode(moves, out words);
			return words.Sum(w => w.Length);
		}

		/*private static bool TryFindWord(MoveDirection[] moves, int startIndex, out string word)
		{
			foreach (var pair in TranslatedPhrases
				.Where(d => d.Value.Length <= moves.Length)
				.OrderByDescending(d => d.Key.Length))
			{
				if (moves.Skip(startIndex).Take(pair.Value.Length).SequenceEqual(pair.Value))
				{
					word = pair.Key;
					return true;
				}
			}
			word = null;
			return false;
		}*/

		private static bool TryFindWord(MoveDirection[] moves, int startIndex, out string word)
		{
			foreach (var pair in TranslatedPhrasesContainer
				.Where(d => d.MovesLen <= moves.Length)
				.OrderByDescending(d => d.KeyLen))
			{
				if (moves.Skip(startIndex).Take(pair.Moves.Length).SequenceEqual(pair.Moves))
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

using System.Linq.Expressions;
using System.Text;

namespace ConsoleApp1
{
	internal class Day13 : Path
	{
		private static int DayID = 13;
		public static void run()
		{
			string path = $"{PATH}day{DayID}.txt";
			//path = $"{PATH}day{DayID}example.txt";
			//path = $"{PATH}day{DayID}example2.txt";
			//path = $"{PATH}day{DayID}example3.txt";
			string[] lines = File.ReadAllLines(path);

			_Task2(lines);
		}

		private static void _Task(string[] lines)
		{
			int reflections = 0;
			int sum = 0;
			List<string> pattern = new List<string>();
			foreach (string line in lines)
			{
				if (!string.IsNullOrWhiteSpace(line))
				{
					pattern.Add(line);
					continue;
				}
				if (pattern.Count == 0) continue;

				Console.WriteLine($"Reflection {reflections + 1}:");
				ReflectionPattern reflection = new ReflectionPattern(pattern);
				int value = reflection.GetReflectionValue();
				sum += value;
				reflections++;
				Console.WriteLine($"Reflection {reflections} value: {value}");

				pattern.Clear();
			}

			if (pattern.Count > 0)
			{
				Console.WriteLine($"Reflection {reflections + 1}:");
				ReflectionPattern reflection = new ReflectionPattern(pattern);
				int value = reflection.GetReflectionValue();
				sum += value;
				reflections++;
				Console.WriteLine($"Reflection {reflections} value: {value}");

			}

			Console.WriteLine($"Finished. The sum of all numbers of rows/columns of {reflections} patterns is {sum}");
		}

		private static void _Task2(string[] lines)
		{
			bool useSmudge = true;
			int reflections = 0;
			int sum = 0;
			List<string> pattern = new List<string>();
			foreach (string line in lines)
			{
				if (!string.IsNullOrWhiteSpace(line))
				{
					pattern.Add(line);
					continue;
				}
				if (pattern.Count == 0) continue;

				Console.WriteLine($"Reflection {reflections + 1}:");
				var reflection = new BitboardReflectionPattern(pattern, useSmudge);
				int value = reflection.GetReflectionValue();
				sum += value;
				reflections++;
				Console.WriteLine($"Reflection {reflections} value: {value}");

				pattern.Clear();
			}

			if (pattern.Count > 0)
			{
				Console.WriteLine($"Reflection {reflections + 1}:");
				var reflection = new BitboardReflectionPattern(pattern, useSmudge);
				int value = reflection.GetReflectionValue();
				sum += value;
				reflections++;
				Console.WriteLine($"Reflection {reflections} value: {value}");

			}

			Console.WriteLine($"Finished. The sum of all numbers of rows/columns of {reflections} patterns is {sum}");
		}

		internal class ReflectionPattern
		{
			private List<string> Pattern, TransposedPattern;

			internal ReflectionPattern(List<string> pattern)
			{
				Pattern = pattern;
				TransposedPattern = TransposePattern(Pattern);

				PrintList(Pattern);
				PrintList(TransposedPattern);
			}

			private void PrintList(List<string> list)
			{
				Console.WriteLine($"------------( {list.Count} x {list[0].Length} )-------------");
				foreach (string line in list)
				{
					Console.WriteLine("  " + line);
				}
				Console.WriteLine("------------------------------");
			}

			private const int VerticalMultiplier = 1;
			private const int HorizontalMultiplier = 100;

			internal int GetReflectionValue()
			{
				int reflection = 0;

				reflection += ReflectionPatternAnalysis(Pattern) * HorizontalMultiplier;
				reflection += ReflectionPatternAnalysis(TransposedPattern) * VerticalMultiplier;
				
				return reflection;
			}

			private int ReflectionPatternAnalysis(List<string> Pattern)
			{
				if (Pattern.Count <= 1) return 0;
				if (Pattern.Count == 2) return Pattern[0] == Pattern[1] ? 1 : 0;

				int max = Pattern.Count - 1;

				for (int axis = 0; axis < max; axis++)
				{
					bool pattern = true;
					// axis = above reflection
					for (int delta = 0; axis + delta + 1 < Pattern.Count; delta++)
					{
						if (axis - delta < 0) break;	// Index too low
						if (Pattern[axis - delta] == Pattern[axis + delta + 1]) continue;
						pattern = false;
						break;
					}
					if (!pattern) continue;
					
					// Return axis number (not index) of left/above the mirror.
					return axis + 1;
				}

				return (int) PatternResult.NO_PATTERN;
			}

			private List<string> TransposePattern(List<string> Pattern)
			{
				List<string> transposedPattern = new List<string>();
				for (int col = 0; col < Pattern[0].Length; col++)
				{
					transposedPattern.Add(ToStr(col));
				}
				return transposedPattern;
			}

			private string ToStr(int column)
			{
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < Pattern.Count; i++)
				{
					if (column >= Pattern[i].Length) builder.Append("@");
					else builder.Append(Pattern[i][column]);
				}
				return builder.ToString();
			}
		}


		internal class BitboardReflectionPattern
		{
			private List<string> s_Pattern, s_TransposedPattern;
			private List<long> Pattern, TransposedPattern;

			private bool UseSmudge;

			internal BitboardReflectionPattern(List<string> pattern, bool useSmudge = true)
			{
				Pattern = ToBitboards(pattern);
				TransposedPattern = ToBitboards(TransposePattern(pattern));

				UseSmudge = useSmudge;

				PrintList(Pattern);
				PrintList(TransposedPattern);
			}

			#region Bitboard conversion

			private const char HDEF_0 = '.';
			private const char HDEF_1 = '#';

			List<long> ToBitboards(List<string> list)
			{
				List<long> bitboards = new List<long>();
				foreach (string entry in list)
				{
					bitboards.Add(ToBitboard(entry));
				}
				return bitboards;
			}

			long ToBitboard(string s)
			{
				long bb = 0;
				foreach (char c in s)
				{
					if (c == HDEF_1) bb |= 1;
					else if (c == HDEF_0) bb |= 0;
					bb <<= 1;
				}

				return bb;
			}

			List<string> ToStrings(List<long> list)
			{
				List<string> strings = new List<string>();
				foreach (long entry in list)
				{
					strings.Add(ToString(entry));
				}
				return strings;
			}

			string ToString(long bb)
			{
				string s = "";
				while (bb > 0)
				{
					byte b = (byte) (bb & 1);
					if (b == 1) s = HDEF_1 + s;
					else if (b == 0) s = HDEF_0 + s;
					bb >>= 1;
				}
				return s;
			}

			#endregion

			private void PrintList(List<long> bitboards)
			{
				List<string> list = ToStrings(bitboards);
				Console.WriteLine($"------------( {list.Count} x {list[0].Length} )-------------");
				foreach (string line in list)
				{
					Console.WriteLine("  " + line);
				}
				Console.WriteLine("------------------------------");
			}

			private const int VerticalMultiplier = 1;
			private const int HorizontalMultiplier = 100;

			internal int GetReflectionValue()
			{
				int reflection = 0;

				reflection += ReflectionPatternAnalysis(Pattern) * HorizontalMultiplier;
				reflection += ReflectionPatternAnalysis(TransposedPattern) * VerticalMultiplier;

				return reflection;
			}

			// Assume there is 1 smudge

			private int ReflectionPatternAnalysis(List<long> Pattern)
			{
				if (Pattern.Count <= 1) return 0;
				if (Pattern.Count == 2) return Pattern[0] == Pattern[1] ? 1 : 0;

				int max = Pattern.Count - 1;

				for (int axis = 0; axis < max; axis++)
				{
					bool pattern = true;
					bool usedHammingDiff = !UseSmudge;		// Usually false, but if we don't use smudge we just pretend we already used it
					// axis = above reflection
					for (int delta = 0; axis + delta + 1 < Pattern.Count; delta++)
					{
						if (axis - delta < 0) break;    // Index too low

						long bitboard = Pattern[axis - delta];
						long other = Pattern[axis + delta + 1];

						if (bitboard == other) continue;
						// Check for Hamming Distance
						if (!usedHammingDiff && IsHammingDistanceOne(bitboard, other))
						{
							usedHammingDiff = true;
							continue;
						}
						pattern = false;
						break;
					}
					if (!pattern || !usedHammingDiff) continue;

					// Return axis number (not index) of left/above the mirror.
					return axis + 1;
				}

				return (int)PatternResult.NO_PATTERN;
			}

			public static bool IsHammingDistanceOne(long bitboard, long other)
			{
				long diff = bitboard ^ other;
				// Now get if there is exactly one 1
				byte index = (byte)System.Numerics.BitOperations.TrailingZeroCount(diff);
				return (diff ^ (1L << index)) == 0;
			}

			private List<string> TransposePattern(List<string> Pattern)
			{
				List<string> transposedPattern = new List<string>();
				for (int col = 0; col < Pattern[0].Length; col++)
				{
					transposedPattern.Add(ToStr(Pattern, col));
				}
				return transposedPattern;
			}

			private string ToStr(List<string> Pattern, int column)
			{
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < Pattern.Count; i++)
				{
					if (column >= Pattern[i].Length) builder.Append("@");
					else builder.Append(Pattern[i][column]);
				}
				return builder.ToString();
			}
		}


		enum PatternResult
		{
			NO_PATTERN = 0
		}
	}
}

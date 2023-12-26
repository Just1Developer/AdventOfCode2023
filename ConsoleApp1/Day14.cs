using System.Linq.Expressions;
using System.Text;

namespace ConsoleApp1
{
	internal class Day14 : Path
	{
		private static int DayID = 14;
		public static void run()
		{
			string path = $"{PATH}day{DayID}.txt";
			//path = $"{PATH}day{DayID}example.txt";
			//path = $"{PATH}day{DayID}example2.txt";
			//path = $"{PATH}day{DayID}example3.txt";
			string[] lines = File.ReadAllLines(path);

			_Task(lines);
		}

		private static void _Task(string[] lines)
		{
			List<string> platform = new List<string>();
			foreach (string line in lines)
			{
				if (!string.IsNullOrWhiteSpace(line))
				{
					platform.Add(line);
				}
			}

			Console.WriteLine($"Starting Platform:");
			Console.WriteLine($"Cycling 1 Billion Times...");
			var reflection = new MirrorPlatform(platform);
			var value = reflection.CycleRef(1000000000, true);
			var weightValue = reflection.CalculateLoad(value);
			Console.WriteLine("Cycled Platform:");
			MirrorPlatform.PrintList(value);

			Console.WriteLine($"Finished. Total load weight: {weightValue}");
		}

		internal class MirrorPlatform
		{
			private List<char[]> Platform = new();

			public MirrorPlatform(List<string> platform)
			{
				foreach (string line in platform)
				{
					Platform.Add(line.ToCharArray());
				}

				//PrintList(Platform);
				//var current = Cycle(Platform);
				//PrintList(current);
			}

			public int GetLoadOfRolledNorth()
			{
				var tilted = TiltNorth(Platform);
				return CalculateLoad(tilted);
			}

			internal List<char[]> Cycle(long times)
			{
				var current = Platform;
				List<List<char[]>> FamiliarFaces = new List<List<char[]>>();
				for (long i = 0; i < times; i++)
				{
					current = Cycle(current);
					if (i % 1000000 == 0) Console.WriteLine($"Finished {i / 1000000} Million of 1 Billion cycles.");

					int index = FamiliarFaces.IndexOf(current);
					if (index == -1)
					{
						FamiliarFaces.Add(current);
						continue;
					}

					// Familiar Face found
					Console.WriteLine($"Found Familiar Face at index {index}, current is {i}");
					long deltaSteps = i - index;
					long remaining = times - i;

					if (deltaSteps >= remaining)
					{
						Console.WriteLine($"DeltaSteps: {deltaSteps}, remaining: {remaining}. Distance too short. Will not skip.");
					}

					long skips = remaining / deltaSteps;
					long skipsAll = skips * deltaSteps;
					i += skipsAll;
					Console.WriteLine($"DeltaSteps: {deltaSteps}, remaining: {remaining}, skips: {skips}, all skipped rounds: {skipsAll}, remaining: {times - i}");
				}
				return current;
			}
			
			internal List<char[]> CycleRef(long times, bool remember = true)
			{
				var current = Platform;
				List<int> FamiliarFaces = new List<int>();
				DateTime start = DateTime.Now;
				DateTime startFinal = DateTime.Now;
				for (long i = 0; i < times; i++)
				{
					CycleRef(ref current);
					if (i % 1000000 == 0)
					{
						DateTime dt = DateTime.Now;
						Console.WriteLine($"Finished {i / 1000000} Million of 1 Billion cycles. ({(dt - start).TotalSeconds}s, {(int) (dt - startFinal).TotalMinutes} mins and {(dt - startFinal).Seconds}s Total)");
						start = dt;
					}

					if (!remember) continue;

					int index = FamiliarFaces.IndexOf(ComputeHash(current));
					if (index == -1)
					{
						FamiliarFaces.Add(ComputeHash(current));
						continue;
					}

					// Familiar Face found
					Console.WriteLine($"Found Familiar Face at index {index}, current is {i}");
					long deltaSteps = i - index;
					long remaining = times - i;

					if (deltaSteps >= remaining)
					{
						Console.WriteLine($"DeltaSteps: {deltaSteps}, remaining: {remaining}. Distance too short. Will not skip.");
					}

					long skips = remaining / deltaSteps;
					long skipsAll = skips * deltaSteps;
					i += skipsAll;
					Console.WriteLine($"DeltaSteps: {deltaSteps}, remaining: {remaining}, skips: {skips}, all skipped rounds: {skipsAll}, remaining: {times - i}");
				}
				return current;
			}

			internal List<char[]> Cycle(List<char[]> list)
			{
				var current = TiltNorth(list);
				current = TiltWest(current);
				current = TiltSouth(current);
				return TiltEast(current);
			}

			internal void CycleRef(ref List<char[]> Platform)
			{
				// Tilt North
				for (int i = 1; i < Platform.Count; i++)
				{
					for (int col = 0; col < Platform[i].Length; col++)
					{
						if (Platform[i][col] != ROLLING_ROCK_CHAR) continue;
						int newRow = i;
						while (newRow > 0)
						{
							newRow--;
							if (Platform[newRow][col] != GROUND_CHAR) break; // Hit an obstacle
							Platform[newRow + 1][col] = GROUND_CHAR;
							Platform[newRow][col] = ROLLING_ROCK_CHAR;
						}
					}
				}

				// Tilt west
				for (int i = 1; i < Platform[0].Length; i++)
				{
					for (int row = 0; row < Platform.Count; row++)
					{
						if (Platform[row][i] != ROLLING_ROCK_CHAR) continue;
						int newCol = i;
						while (newCol > 0)
						{
							newCol--;
							if (Platform[row][newCol] != GROUND_CHAR) break; // Hit an obstacle
							Platform[row][newCol + 1] = GROUND_CHAR;
							Platform[row][newCol] = ROLLING_ROCK_CHAR;
						}
					}
				}

				// Tilt south
				for (int i = Platform.Count - 2; i >= 0; i--)
				{
					for (int col = 0; col < Platform[i].Length; col++)
					{
						if (Platform[i][col] != ROLLING_ROCK_CHAR) continue;
						int newRow = i;
						while (newRow < Platform.Count - 1)
						{
							newRow++;
							if (Platform[newRow][col] != GROUND_CHAR) break; // Hit an obstacle
							Platform[newRow - 1][col] = GROUND_CHAR;
							Platform[newRow][col] = ROLLING_ROCK_CHAR;
						}
					}
				}

				// Tilt east
				for (int i = Platform[0].Length - 2; i >= 0; i--)
				{
					for (int row = 0; row < Platform.Count; row++)
					{
						if (Platform[row][i] != ROLLING_ROCK_CHAR) continue;
						int newCol = i;
						while (newCol < Platform[row].Length - 1)
						{
							newCol++;
							if (Platform[row][newCol] != GROUND_CHAR) break; // Hit an obstacle
							Platform[row][newCol - 1] = GROUND_CHAR;
							Platform[row][newCol] = ROLLING_ROCK_CHAR;
						}
					}
				}
			}

			private int ComputeHash(List<char[]> platform)
			{
				int hash = 17;
				foreach (var row in platform)
				{
					foreach (var c in row)
					{
						hash = hash * 31 + c.GetHashCode();
					}
				}
				return hash;
			}

			internal static void PrintList(List<char[]> list)
			{
				Console.WriteLine($"------------( {list.Count} x {list[0].Length} )-------------");
				foreach (var line in list)
				{
					Console.Write("  ");
					foreach (char c in line) Console.Write(c);
					Console.WriteLine();
				}
				Console.WriteLine("------------------------------");
			}

			private const char GROUND_CHAR = '.';
			private const char SHARP_ROCK_CHAR = '#';
			private const char ROLLING_ROCK_CHAR = 'O';
			
			private List<char[]> TiltNorth(List<char[]> Platform)
			{
				List<char[]> newPlatform = new();
				// Add all
				for (int i = 0; i < Platform.Count; i++)
				{
					char[] c = new char[Platform[i].Length];
					for (int j = 0; j < Platform[i].Length; j++)
					{
						c[j] = Platform[i][j];
					}
					newPlatform.Add(c);
				}

				// Let's do this the right way actually
				for (int i = 1; i < newPlatform.Count; i++)
				{
					for (int col = 0; col < newPlatform[i].Length; col++)
					{
						if (newPlatform[i][col] != ROLLING_ROCK_CHAR) continue;
						int newRow = i;
						while (newRow > 0)
						{
							newRow--;
							if (newPlatform[newRow][col] != GROUND_CHAR) break;	// Hit an obstacle
							newPlatform[newRow + 1][col] = GROUND_CHAR;
							newPlatform[newRow][col] = ROLLING_ROCK_CHAR;
						}
					}
				}

				return newPlatform;
			}
			
			private List<char[]> TiltSouth(List<char[]> Platform)
			{
				List<char[]> newPlatform = new();
				// Add all
				for (int i = 0; i < Platform.Count; i++)
				{
					char[] c = new char[Platform[i].Length];
					for (int j = 0; j < Platform[i].Length; j++)
					{
						c[j] = Platform[i][j];
					}
					newPlatform.Add(c);
				}

				// Let's do this the right way actually
				for (int i = newPlatform.Count - 2; i >= 0; i--)
				{
					for (int col = 0; col < newPlatform[i].Length; col++)
					{
						if (newPlatform[i][col] != ROLLING_ROCK_CHAR) continue;
						int newRow = i;
						while (newRow < newPlatform.Count - 1)
						{
							newRow++;
							if (newPlatform[newRow][col] != GROUND_CHAR) break;	// Hit an obstacle
							newPlatform[newRow - 1][col] = GROUND_CHAR;
							newPlatform[newRow][col] = ROLLING_ROCK_CHAR;
						}
					}
				}

				return newPlatform;
			}
			
			private List<char[]> TiltEast(List<char[]> Platform)
			{
				List<char[]> newPlatform = new();
				// Add all
				for (int i = 0; i < Platform.Count; i++)
				{
					char[] c = new char[Platform[i].Length];
					for (int j = 0; j < Platform[i].Length; j++)
					{
						c[j] = Platform[i][j];
					}
					newPlatform.Add(c);
				}

				// Let's do this the right way actually
				for (int i = newPlatform[0].Length - 2; i >= 0; i--)
				{
					for (int row = 0; row < newPlatform.Count; row++)
					{
						if (newPlatform[row][i] != ROLLING_ROCK_CHAR) continue;
						int newCol = i;
						while (newCol < newPlatform[row].Length - 1)
						{
							newCol++;
							if (newPlatform[row][newCol] != GROUND_CHAR) break;	// Hit an obstacle
							newPlatform[row][newCol - 1] = GROUND_CHAR;
							newPlatform[row][newCol] = ROLLING_ROCK_CHAR;
						}
					}
				}

				return newPlatform;
			}
			
			private List<char[]> TiltWest(List<char[]> Platform)
			{
				List<char[]> newPlatform = new();
				// Add all
				for (int i = 0; i < Platform.Count; i++)
				{
					char[] c = new char[Platform[i].Length];
					for (int j = 0; j < Platform[i].Length; j++)
					{
						c[j] = Platform[i][j];
					}
					newPlatform.Add(c);
				}

				// Let's do this the right way actually
				for (int i = 1; i < newPlatform[0].Length; i++)
				{
					for (int row = 0; row < newPlatform.Count; row++)
					{
						if (newPlatform[row][i] != ROLLING_ROCK_CHAR) continue;
						int newCol = i;
						while (newCol > 0)
						{
							newCol--;
							if (newPlatform[row][newCol] != GROUND_CHAR) break;	// Hit an obstacle
							newPlatform[row][newCol + 1] = GROUND_CHAR;
							newPlatform[row][newCol] = ROLLING_ROCK_CHAR;
						}
					}
				}

				return newPlatform;
			}

			internal int CalculateLoad(List<char[]> Platform)
			{
				int totalLoad = 0;
				int currentLoad = Platform.Count;
				for (int i = 0; i < Platform.Count; i++, currentLoad--)
				{
					foreach (char c in Platform[i])
					{
						if (c == ROLLING_ROCK_CHAR) totalLoad += currentLoad;
					}
				}

				return totalLoad;
			}
		}

	}
}

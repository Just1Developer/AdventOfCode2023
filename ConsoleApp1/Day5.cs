namespace ConsoleApp1
{

	// Answer for Task 1: 323142486
	// Answer for Task 2 (because with this code it took like 45 mins to run): 79874951			| (1.702.217.569 runs)

	internal class Day5
	{
		static List<Map> AllMaps = new List<Map>();

		public static void run()
		{
			// Day 4 Task 1:
			string path = "E:\\Coding\\AdventOfCode\\one\\ConsoleApp1\\ConsoleApp1\\input\\day5.txt";
			//path = "E:\\Coding\\AdventOfCode\\one\\ConsoleApp1\\ConsoleApp1\\input\\day5example.txt";
			string[] lines = File.ReadAllLines(path);

			Task1(lines);
		}

		static void Task1(string[] lines)
		{
			List<long> seeds = new List<long>();
			Map? currentMap = null;
			foreach (string line in lines)
			{
				if (line.StartsWith("seeds"))
				{
					foreach (string s in line.Split(": ")[1].Trim().Split(" "))
					{
						try { seeds.Add(long.Parse(s)); } catch { Console.WriteLine($"Error. Could not parse seed: '{s}'"); }
					}
					continue;
				}
				if (string.IsNullOrWhiteSpace(line))
				{
					if (currentMap == null) continue;
					AllMaps.Add(currentMap);
					currentMap = null;
					// Finished setting up a map
					continue;
				}
				if (line.Contains("map"))
				{
					currentMap = new Map(line);
					continue;
				}
				if (currentMap == null) continue;
				currentMap.AddMapEntry(line);
			}
			if (currentMap != null) AllMaps.Add(currentMap);

			// Data loaded, now find the lowest location.

			long lowestLocation = long.MaxValue;

			foreach (long seed in seeds)
			{
				long locNumber = MapValue("location", seed);
				if (locNumber < lowestLocation) lowestLocation = locNumber;
			}

			Console.WriteLine($"The lowest location number is {lowestLocation}.");
		}

		static void Task2(string[] lines)
		{
			List<(long, long)> seeds = new ();
			Map? currentMap = null;
			foreach (string line in lines)
			{
				if (line.StartsWith("seeds"))
				{
					string[] numbers = line.Split(": ")[1].Trim().Split(" ");
					for (int i = 0; i < numbers.Length; i += 2)
					{
						long start = 0L, end = 0L;
						try { start = long.Parse(numbers[i]); } catch { Console.WriteLine($"Error. Could not parse seed: '{numbers[i]}'"); }
						try { end = start + long.Parse(numbers[i + 1]); } catch { Console.WriteLine($"Error. Could not parse seed: '{numbers[i + 1]}'"); }
						seeds.Add((start, end));
					}
					continue;
				}
				if (string.IsNullOrWhiteSpace(line))
				{
					if (currentMap == null) continue;
					AllMaps.Add(currentMap);
					currentMap = null;
					// Finished setting up a map
					continue;
				}
				if (line.Contains("map"))
				{
					currentMap = new Map(line);
					continue;
				}
				if (currentMap == null) continue;
				currentMap.AddMapEntry(line);
			}
			if (currentMap != null) AllMaps.Add(currentMap);

			// Data loaded, now find the lowest location.

			long lowestLocation = long.MaxValue;

			long runs = 0;
			foreach (var seedPair in seeds)
			{
				Console.WriteLine($"[Total runs: {runs}] Starting with range ({seedPair.Item1} - {seedPair.Item2})");
				for (long i = seedPair.Item1; i < seedPair.Item2; i++)
				{
					runs++;
					long locNumber = MapValue("location", i);
					if (locNumber < lowestLocation) lowestLocation = locNumber;
				}
			}

			Console.WriteLine($"The lowest location number is {lowestLocation}.");
		}


		static long MapValue(string to, long x) => MapValue("seed", to, x);
		static long MapValue(string from, string to, long x)
		{
			long value = x;
			string currentSource = from.ToLower();
			foreach (Map map in AllMaps)
			{
				if (map.MapDest == currentSource) break;
				// Maps are in order, which is good for us. No real complexity here.
				if (map.MapFrom == currentSource)
				{
					value = map.GetDestinationMapping(value);
					currentSource = map.MapDest;
				}
			}
			if (currentSource != to)
				Console.WriteLine($"ERROR. Could not trace value. From: {from}, To: {to}, Value: {x}, Mapped Value: {value}");
			return value;
		}

		static List<(long, long)> MapValueRange(string to, (long, long) x) => MapValueRange("seed", to, x);
		static List<(long, long)> MapValueRange(string from, string to, (long, long) x)
		{
			List<(long, long)> valueList = new ();
			valueList.Add(x);
			string currentSource = from.ToLower();

			foreach (Map map in AllMaps)
			{
				if (map.MapDest == currentSource) break;
				// Maps are in order, which is good for us. No real complexity here.
				if (map.MapFrom == currentSource)
				{
					valueList = map.GetDestinationMappingRange(valueList);
					currentSource = map.MapDest;
				}
			}
			if (currentSource != to)
				Console.WriteLine($"ERROR. Could not trace value. From: {from}, To: {to}, Value: ({x.Item1}, {x.Item2}), Mapped Value: ? -> {valueList}");
			return valueList;
		}

		class Map
		{
			public string MapFrom { get; private set; }
			public string MapDest { get; private set; }

			List<MapEntry> Entries;

			public Map(string MapTitleLine)
			{
				string[] s = MapTitleLine.Trim().Split("-to-");
				MapFrom = s[0].Trim().ToLower();
				MapDest = s[1].Split("map")[0].Trim().ToLower();
				Entries = new();
			}

			public void AddMapEntry(string mapEntryLine)
			{
				string[] nums = mapEntryLine.Trim().Split(' ');
				if (nums.Length != 3) throw new ArgumentException($"Number list length was not 3 >> '{mapEntryLine}'");
				MapEntry entry = new MapEntry(long.Parse(nums[0]), long.Parse(nums[1]), long.Parse(nums[2]));
				this.Entries.Add(entry);
			}

			public bool IsInSourceRange(long x)
			{
				foreach (MapEntry entry in Entries)
				{
					if (entry.IsInSourceRange(x)) return true;
				}
				return false;
			}

			public long GetDestinationMapping(long x)
			{
				foreach (MapEntry entry in Entries)
				{
					if (entry.IsInSourceRange(x)) return entry.GetDestinationMapping(x);
				}
				return x;
			}

			public List<(long, long)> GetDestinationMappingRange(List<(long, long)> x)
			{
				List<(long, long)> newMapped = new List<(long, long)> ();
				foreach (MapEntry entry in Entries)
				{
					for (int i = 0; i < x.Count; i++)
					{
						var mappedEntry = x[i];
						// Map whatever is in this entry and enter the rest as a range back into x for later processing

						// No overlap
						if (entry.SourceRangeStart >= mappedEntry.Item1 + mappedEntry.Item2) continue;	// If entire range is below the entry range
						if (entry.SourceRangeStart + entry.RangeLength < mappedEntry.Item1) continue;   // If entire range is above the entry range

						// Overlap. Now get this overlap
						// First, check if the entire range is inside the entry.
						if (entry.SourceRangeStart <= mappedEntry.Item1
							&& entry.SourceRangeStart + entry.RangeLength <= mappedEntry.Item1 + mappedEntry.Item2)
						{




							// TODO yeah no we'd need to store the actual beginning and end, not the length
							
							
							
							
							newMapped.Add((entry.GetDestinationMapping(mappedEntry.Item1), entry.GetDestinationMapping(mappedEntry.Item2)));
							i--;
							continue;
						}
					}
					//if (entry.IsInSourceRange(x)) return entry.GetDestinationMapping(x);
				}
				return x;
			}
		}

		class MapEntry
		{
			public long SourceRangeStart { get; private set; }
			public long DestinationRangeStart { get; private set; }
			public long RangeLength { get; private set; }

			public bool IsInSourceRange(long x)
			{
				return x >= SourceRangeStart && x < SourceRangeStart + RangeLength;
			}

			public long GetDestinationMapping(long x)
			{
				if (!IsInSourceRange(x)) return x;
				return x - SourceRangeStart + DestinationRangeStart;
			}

			public MapEntry(long destStart, long srcStart, long length)
			{
				this.SourceRangeStart = srcStart;
				this.DestinationRangeStart = destStart;
				this.RangeLength = length;
			}
			private MapEntry() { }
		}
	}
}

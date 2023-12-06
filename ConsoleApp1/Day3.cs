using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp1
{
	internal class Day3
	{

		public static void run()
		{
			// Day 3 Task 1:
			string path = "E:\\Coding\\AdventOfCode\\one\\ConsoleApp1\\ConsoleApp1\\input\\day3.txt";
			//string path = "E:\\Coding\\AdventOfCode\\one\\ConsoleApp1\\ConsoleApp1\\input\\day3example.txt";
			var layout = new EngineLayout(path);
			Console.WriteLine($"Value: {layout.CalculateValueForAdjacent()}");
			Console.WriteLine($"[T2] Gear Value: {EnginePart.CalculateValueOfGears()}");
		}

		class EngineLayout
		{

			internal string[] layout;

			internal EngineLayout(string filepath)
			{
				layout = File.ReadAllLines(filepath);
			}


			// Task 1 of Day 3
			public unsafe int CalculateValueForAdjacent()
			{
				int row = 0, col = 0;
				var next = NextPart(&row, &col);
				int sum = 0;
				
				while (next.Item2)
				{
					// Just calculate the sum of all valid parts
					if (next.Item1 != null)
						sum += next.Item1.NumberValue;

					next = NextPart(&row, &col);
				}

				return sum;
			}

			private unsafe (EnginePart?, bool) NextPart(int* rowPtr, int* columnPtr)
			{
				EnginePart? part = null;
				int row = *rowPtr;
				int column = *columnPtr;

				while (row < layout.Length)
				{
					if (layout[row][column] >= '0' && layout[row][column] <= '9')
					{
						part ??= new EnginePart(row, column);
						part.Append(layout[row][column]);
					}
					else if (part != null) break;


					// Increment & Check for next line
					if (++(*columnPtr) >= layout[row].Length)
					{
						(*rowPtr)++;
						*columnPtr = 0;
					}
					row = *rowPtr;
					column = *columnPtr;
				}

				return (part?.Validate2(layout), part != null);
			}

		}

		class EnginePart
		{
			internal static Dictionary<int, int> GearValues = new ();
			internal static Dictionary<int, List<int>> GearNeighbors = new ();
			internal static List<int> ProcessedNumbers = new ();

			private StringBuilder contentBuilder;
			private int row, startLocationColumn;

			internal int Length { get => contentBuilder.Length; }
			internal int NumberValue { get => int.Parse(Content); }
			internal string Content { get => contentBuilder.ToString(); }

			internal int Row { get => row; }
			internal int ColumnStart { get => startLocationColumn; }
			internal int ColumnEnd { get => startLocationColumn + Length; }

			private EnginePart() {}

			internal EnginePart(int row, int col, string content = "")
			{
				this.row = row;
				this.startLocationColumn = col;
				contentBuilder = new StringBuilder(content);
			}

			internal void Append(string s)
			{
				contentBuilder.Append(s);
			}

			internal void Append(char c)
			{
				contentBuilder.Append(c);
			}

			internal EnginePart? Validate(string[] content)
			{
				Console.WriteLine($"Validating Content {Content}, value {NumberValue}...");
				int rowCurrent = row - 1, colCurrent = ColumnStart == 0 ? ColumnStart : ColumnStart - 1;
				
				// Check next, same row and different cols.
				{
					char c = content[row][colCurrent];
					if ((c <= '0' || c >= '9') && c != '.')
						return this;
				}
				if (ColumnEnd < content[row].Length) {
					char c = content[row][ColumnEnd];
					if ((c <= '0' || c >= '9') && c != '.')
						return this;
				}

				// Check above
				while (rowCurrent < content.Length && rowCurrent >= 0 && colCurrent < ColumnEnd + 1 && colCurrent < content[rowCurrent].Length)
				{
					char c = content[rowCurrent][colCurrent++];
					if (c >= '0' && c <= '9' || c == '.') continue;

					// c is a symbol, validated. return.
					return this;
				}

				rowCurrent = row + 1;
				colCurrent = ColumnStart == 0 ? ColumnStart : ColumnStart - 1;
				
				// Check below
				while (rowCurrent < content.Length && rowCurrent >= 0 && colCurrent < ColumnEnd + 1 && colCurrent < content[rowCurrent].Length)
				{
					char c = content[rowCurrent][colCurrent++];
					if (c >= '0' && c <= '9' || c == '.') continue;

					// c is a symbol, validated. return.
					return this;
				}

				Console.WriteLine($"Scratching for {Content}, no neighbor found.");

				return null;
			}

			internal EnginePart? Validate2(string[] content)
			{
				bool validated = false;
				int startCoordValue = HashValue(this.Row, this.ColumnStart);
				int NumberValue = this.NumberValue;

				Console.WriteLine($"[T2] Validating Content {Content}, value {NumberValue}...");
				int rowCurrent = row - 1, colCurrent = ColumnStart == 0 ? ColumnStart : ColumnStart - 1;
				
				// Check next, same row and different cols.
				{
					char c = content[row][colCurrent];
					if ((c <= '0' || c >= '9') && c != '.')
						validated = true;

					if (c == '*') GearPlusPlus(row, colCurrent, NumberValue, startCoordValue);
				}
				if (ColumnEnd < content[row].Length) {
					char c = content[row][ColumnEnd];
					if ((c <= '0' || c >= '9') && c != '.')
						validated = true;

					if (c == '*') GearPlusPlus(row, ColumnEnd, NumberValue, startCoordValue);
				}

				// Check above
				while (rowCurrent < content.Length && rowCurrent >= 0 && colCurrent < ColumnEnd + 1 && colCurrent < content[rowCurrent].Length)
				{
					char c = content[rowCurrent][colCurrent++];
					if (c >= '0' && c <= '9' || c == '.') continue;

					// c is a symbol, validated.
					if (c == '*') GearPlusPlus(rowCurrent, colCurrent - 1, NumberValue, startCoordValue);
					validated = true;
				}

				rowCurrent = row + 1;
				colCurrent = ColumnStart == 0 ? ColumnStart : ColumnStart - 1;
				
				// Check below
				while (rowCurrent < content.Length && rowCurrent >= 0 && colCurrent < ColumnEnd + 1 && colCurrent < content[rowCurrent].Length)
				{
					char c = content[rowCurrent][colCurrent++];
					if (c >= '0' && c <= '9' || c == '.') continue;

					// c is a symbol, validated.
					if (c == '*') GearPlusPlus(rowCurrent, colCurrent - 1, NumberValue, startCoordValue);
					validated = true;
				}

				return validated ? this : null;
			}

			static int HashValue(int row, int col)
			{
				return (row << 8) + col;
			}
			static (int, int) UnhashValue(int hashed)
			{
				int row = hashed >> 8;
				int col = hashed & 0x00FF;
				return (row, col);
			}

			static void GearPlusPlus(int row, int col, int numberValue, int startCoordValue)
			{
				int hash = HashValue(row, col);
				if (GearValues.ContainsKey(hash))
				{
					GearValues[hash]++;
					GearNeighbors[hash].Add(numberValue);
				}
				else
				{
					GearValues.Add(hash, 1);
					GearNeighbors.Add(hash, new List<int>());
					GearNeighbors[hash].Add(numberValue);
				}
			}

			public static int CalculateValueOfGears()
			{
				int sum = 0;
				foreach (var gear in GearValues)
				{
					if(gear.Value != 2) continue;

					var coords = UnhashValue(gear.Key);
					Console.WriteLine($"Found gear at location: row: {coords.Item1}, col: {coords.Item2}");

					var neighbors = GearNeighbors[gear.Key];

					int product = 1;
					foreach (int factor in neighbors)
						product *= factor;

					sum += product;
				}
				return sum;
			}
		}

	}
}

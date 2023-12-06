using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ConsoleApp1
{
	internal class Day2_1
	{
		public static void run()
		{
			string path = "E:\\Coding\\AdventOfCode\\one\\ConsoleApp1\\ConsoleApp1\\day2-2.txt";
			string[] file = File.ReadAllLines(path);

			int maxRed = 12, maxGreen = 13, maxBlue = 14;

			int totalSum = 0;

			foreach (string line in file)
			{
				// Parse some values
				string values = line.Split(": ")[1];
				string[] runs = values.Split("; ");
				
				bool _break = false;
				foreach (string run in runs)
				{
					if (_break) break;
					string[] colors = run.Split(", ");
					foreach (string color in colors)
					{
						int amt = int.Parse(color.Split(" ")[0]);
						if (color.EndsWith("red") && amt > maxRed)
						{
							// Impossible.
							_break = true;
							break;
						}

						if (color.EndsWith("green") && amt > maxGreen)
						{
							// Impossible.
							_break = true;
							break;
						}

						if (color.EndsWith("blue") && amt > maxBlue)
						{
							// Impossible.
							_break = true;
							break;
						}
					}
				}

				if (_break) continue;

				int gameID = int.Parse(line.Split(": ")[0].Split(" ")[1]);
				totalSum += gameID;
			}

			Console.WriteLine("Total sum: " + totalSum);
		}
	}

	internal class Day2_2
	{
		public static void run()
		{
			string path = "E:\\Coding\\AdventOfCode\\one\\ConsoleApp1\\ConsoleApp1\\day2-2.txt";
			string[] file = File.ReadAllLines(path);

			int totalSum = 0;

			foreach (string line in file)
			{
				// Parse some values
				string values = line.Split(": ")[1];
				string[] runs = values.Split("; ");

				int minimumRed = 0, minimumGreen = 0, minimumBlue = 0;

				foreach (string run in runs)
				{
					string[] colors = run.Split(", ");
					foreach (string color in colors)
					{
						int amt = int.Parse(color.Split(" ")[0]);
						if (color.EndsWith("red") && amt > minimumRed)
						{
							minimumRed = amt;
							continue;
						}

						if (color.EndsWith("green") && amt > minimumGreen)
						{
							minimumGreen = amt;
							continue;
						}

						if (color.EndsWith("blue") && amt > minimumBlue)
						{
							minimumBlue = amt;
						}
					}
				}

				int power = minimumRed * minimumGreen * minimumBlue;
				totalSum += power;
			}

			Console.WriteLine("Total sum: " + totalSum);
		}

	}
}

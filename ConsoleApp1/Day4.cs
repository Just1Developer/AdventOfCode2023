using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	internal class Day4
	{
		public static void run()
		{
			// Day 4 Task 1:
			string path = "E:\\Coding\\AdventOfCode\\one\\ConsoleApp1\\ConsoleApp1\\input\\day4.txt";
			//path = "E:\\Coding\\AdventOfCode\\one\\ConsoleApp1\\ConsoleApp1\\input\\day4example.txt";
			string[] lines = File.ReadAllLines(path);
			int sum = 0;

			// Build all cards
			foreach (string line in lines)
			{
				new Scratchcard(line);
			}

			for (int currentLine = 0; currentLine < Scratchcard.Scratchcards.Count; currentLine++)
			{
				var card = Scratchcard.Scratchcards[currentLine];
				sum += card.Copies;
				// Generate new copies
				// 1. How many cards?
				int newCards = Math.Min(card.MatchingNums, Scratchcard.Scratchcards.Count - currentLine);
				for (int i = currentLine + 1; i <= currentLine + newCards; i++)
				{
					Scratchcard.Scratchcards[i].Copies += card.Copies;
				}
			}

			Console.WriteLine("Sum of all Scratchcards: " + sum);
		}

		class Scratchcard
		{
			internal static List<Scratchcard> Scratchcards = new List<Scratchcard>();

			private List<int> Winners;
			private string[] numbers;
			internal int Value;
			internal int Copies = 1;
			internal int MatchingNums = 1;

			public Scratchcard(string inputLine)
			{
				string[] lists = inputLine.Split(": ")[1].Trim().Split(" | ");
				string[] winners = lists[0].Split(" ");
				numbers = lists[1].Split(" ");

				Winners = new ();
				foreach (string winner in winners)
				{
					if (string.IsNullOrWhiteSpace(winner)) continue;
					Winners.Add(int.Parse(winner.Trim()));
				}
				Value = GetValue();

				Scratchcards.Add(this);
			}

			public int GetValue()
			{
				int correct = -1;
				foreach (string number in numbers)
				{
					if (string.IsNullOrWhiteSpace(number)) continue;
					if(Winners.Contains(int.Parse(number.Trim())))
						correct++;
				}
				MatchingNums = correct + 1;

				if (correct == -1) return 0;
				return (int) Math.Pow(2, correct);
			}
		}
	}
}

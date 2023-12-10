using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	internal class Day9 : Path
    {
        public static void run()
        {
            string path = PATH + "day9.txt";
            //path = PATH + "day9example.txt";
            path = PATH + "day9example2.txt";
            string[] lines = File.ReadAllLines(path);

            Task(lines);
        }

        static void Task(string[] lines)
		{
            int sum = 0;
            foreach (string line in lines)
			{
                NumHistory history = new NumHistory(line);
                //Histories.Add(history);

                // Task 1:
                //int next = history.ExtrapolateNext();
                // Task 2:
                int next = history.ExtrapolatePrevious();

                // Print History as a test
                NumHistory? temp = history;
                string prefix = "";
                while (temp != null)
				{
                    foreach (int num in temp.Numbers)
                    {
                        Console.Write(prefix + num + "  ");
                    }
                    Console.Write("\n");
                    prefix += " ";
                    temp = temp.Derivative;
				}
                Console.WriteLine("----------------------- Next Extrapolated: " + next);
                sum += next;
            }

            Console.WriteLine("Sum of all extrapolated values: " + sum);
		}

        internal class NumHistory
		{
            internal List<int> Numbers;

            internal NumHistory? Derivative;

            internal NumHistory(string sequence)
			{
                while (sequence.Contains("  ")) sequence = sequence.Replace("  ", " ");
                string[] nums = sequence.Split(' ');
                Numbers = new List<int>();
                foreach (string num in nums)
				{
                    Numbers.Add(int.Parse(num));
				}

                BuildDerivative();
            }
            internal NumHistory(List<int> history)
			{
                Numbers = new ();
                Numbers.AddRange(history);

                BuildDerivative();
            }

            internal void BuildDerivative()
			{
                if(IsNullHistory())
				{
                    Derivative = null;
                    return;
				}

                List<int> deltas = new List<int>();
                for (int i = 1; i < Numbers.Count; i++)
				{
                    deltas.Add(Numbers[i] - Numbers[i - 1]);
				}
                Derivative = new NumHistory(deltas);
			}

            internal int ExtrapolateNext()
			{
                if(Derivative == null)
				{
                    // Current Value is null.
                    return 0;
				}
                // Get last value and add the value of the derivative
                return Derivative.ExtrapolateNext() + Numbers[Numbers.Count - 1];
			}

            internal int ExtrapolatePrevious()
			{
                if(Derivative == null)
				{
                    // Current Value is null.
                    return 0;
				}
                // Get last value and add the value of the derivative
                return Numbers[0] - Derivative.ExtrapolatePrevious();
			}

            internal bool IsNullHistory()
			{
                foreach (int num in Numbers)
                {
                    if (num != 0) return false;
                }
                return true;
			}
        }
    }
}

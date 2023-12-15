using System.Collections.Concurrent;
using System.Text;

namespace ConsoleApp1;

public class Day12 : Path
{
    private const int skip = 0;
    
    public static void run()
    {
        string path = PATH + "day12.txt";
        //path = PATH + "day12example.txt";
        //path = PATH + "day12example2.txt";
        string[] lines = File.ReadAllLines(path);

        //_Task(lines);
        TaskMultithreaded(lines).Wait();
    }

    private static void _Task(string[] lines)
    {
        int l = 0;
        int sum = 0;
        foreach (string line in lines)
        {
            SpringRow row = new SpringRow(line, ++l);
            Console.WriteLine($"Calculating line {l}/{lines.Length} with {row.NumbersString}...");
            int arrangements = row.GetPossibilityCount();
            sum += arrangements;
            Console.WriteLine($"Finished line {l}/{lines.Length}, arrangements: {arrangements}");
        }
        Console.WriteLine($"Finished. The sum of all possible arrangements is {sum}");
    }

    private static int sum = 0, linesLength;
    static ConcurrentBag<SpringRow> AllRows = new ConcurrentBag<SpringRow>();
    
    private static async Task TaskMultithreaded(string[] lines)
    {
        int l = 0;
        linesLength = lines.Length;
        coreCount = (int)(Environment.ProcessorCount / TOTAL_CORE_DIVIDER);
        
        foreach (string line in lines)
        {
            // For skipping stuff we already calculated
            if (l < skip)
            {
                l++;
                continue;
            }
            SpringRow row = new SpringRow(line, ++l);
            AllRows.Add(row);
        }

        await CalculateAllRowsSimul();
        
        Console.WriteLine($"Finished. The sum of all possible arrangements is {sum}");
    }
    
    // The start and workerThreads logic is abstracted from ChatGPT code
    private static List<Task> threadPoolTasks;
    private static int coreCount;
    private const double TOTAL_CORE_DIVIDER = 1;  // On my pc with 16 Cores: 1.1 = 14 Cores, 1.15 = 13 Cores, 1.3 = 12 Cores
    public static async Task CalculateAllRowsSimul()
    {
        // Create a task for each core in the system.
        threadPoolTasks = new List<Task>(coreCount);

        for (int i = 0; i < coreCount; i++)
        {
            var task = Task.Run(() => ThreadTask());
            threadPoolTasks.Add(task);
        }

        // No need for Run = false since the Tasks only end when Run = false
        await Task.WhenAll(threadPoolTasks);
        Console.WriteLine("All Finished.");
    }

    private static void ThreadTask()
    {
        while (!AllRows.IsEmpty)
        {
            SpringRow? row;
            if (!AllRows.TryTake(out row)) continue;
            // Calculate
            Console.WriteLine($"Calculating line {row.ID}/{linesLength} with {row.NumbersString}...");
            int arrangements = row.GetPossibilityCount();
            sum += arrangements;
            Console.WriteLine($"Finished line {row.ID}/{linesLength}, arrangements: {arrangements}");
        }
    }

    class SpringRow
    {
        private string RawInput = "";
        private string Springs;
        internal string NumbersString = "";
        private int[] Numbers;
        private int QuestionCounts;
        public readonly int ID;
        
        internal SpringRow(string raw, int id)
        {
            this.ID = id;
            RawInput = raw;
            string[] split = raw.Split(' ');
            //Springs = split[0];
            Springs = Task2TransformSpring(split[0]); // Task 2

            if (split.Length == 1) return;  // No numbers, no questionsmarks
            
            //string[] nums = split[1].Split(',');
            NumbersString = Task2TransformNums(split[1]);
            string[] nums = NumbersString.Split(',');    // Task 2
            
            Numbers = new int[nums.Length];
            int i = 0;
            foreach (var num in nums)
            {
                Numbers[i++] = int.Parse(num.Trim());   // If something here doesn't work, I wanna know. No error handling
            }

            foreach (char c in Springs)
            {
                if (c == '?') QuestionCounts++;
            }
        }
        
        //region Task 2 Transforms

        static string Task2TransformSpring(string s)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                builder.Append(s);
                if (i < 4) builder.Append('?');
            }
            return builder.ToString();
        }

        static string Task2TransformNums(string s)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                builder.Append(s);
                if (i < 4) builder.Append(',');
            }
            return builder.ToString();
        }
        
        //endregion

        internal int GetPossibilityCount()
        {
            if (QuestionCounts == 0) return 1;
            
            int possibilities = 0;
            long bitboard = (long) Math.Pow(2, QuestionCounts) - 1;
            while (bitboard >= 0)
            {
                // Just so I have it while debugging
                if (IsValid(bitboard--)) possibilities++;
            }

            return possibilities;
        }
        
        // So, there is a smart way and a dumb way to approach this.
        // I say we do bruteforce and see if it gets us anywhere.
        // If not, we have to get a little smarter.
        private bool IsValid(long constellation)
        {
            // Here is how this works:
            // constellation is a bitboard, 0 = . (healthy), 1 = # (damaged)
            // Put it in, see if it adds up with the numbers. If it does, it's valid, so return true.

            StringBuilder constructor = new StringBuilder();
            int shiftIndex = QuestionCounts;

            foreach (char c in Springs)
            {
                if (c == '.' || c == '#')
                {
                    constructor.Append(c);
                    continue;
                }
                if (c != '?') throw new Exception($"IsValid(long): Unknown character {c} in {Springs}");
                
                // c is ?
                if (((constellation >> --shiftIndex) & 1) == 0) constructor.Append('.');
                else constructor.Append('#');
            }
            
            bool isValid = IsValid(constructor.ToString());
            
            //if(isValid) printConstellation(Springs, constructor.ToString(), constellation, isValid);

            return isValid;
        }

        private bool IsValid(string possibility)
        {
            int brokens = 0;
            int currentIndex = 0;
            foreach (var c in possibility)
            {
                if (c == '?') throw new Exception("IsValid(string): ? is illegal character here");
                if (c == '#')
                {
                    // Broken
                    brokens++;
                    continue;
                }
                if (c != '.') throw new Exception($"IsValid(string): Unknown symbol in {possibility}: {c}");
                
                // Healthy
                if (brokens == 0) continue;

                if (currentIndex >= Numbers.Length) return false;   // Invalid. Too many brokens
                
                // Current index to compare.
                int shouldBeBroken = Numbers[currentIndex++];
                if (shouldBeBroken != brokens) return false;
                brokens = 0;
            }
            
            // Now it's the end.
            if (brokens > 0)
            {
                // unprocessed brokens
                if (currentIndex >= Numbers.Length) return false;   // Invalid. Too many brokens
                
                // Current index to compare.
                int shouldBeBroken = Numbers[currentIndex++];
                if (shouldBeBroken != brokens) return false;
            }
            
            // Everything passed. If we've also handled everything from the numbers list, we should be good.
            return currentIndex == Numbers.Length;
        }

        private void printConstellation(string question, string finished, long mask, bool valid)
        {
            // Style: Const: (question) (long mask) -> (finished) (valid)(invalid)
            Console.Write("Const: ");
            WriteString(question);
            Console.Write(" (via ");
            WriteString(Convert.ToString(mask, 2));
            Console.Write(")  ->  ");
            WriteString(finished);
            Console.Write("  (");
            if (valid)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("valid");   
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("invalid");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(")\n");
        }

        private void WriteString(string spring)
        {
            foreach (char c in spring)
            {
                if (c == '.') Console.ForegroundColor = ConsoleColor.Green;
                else if (c == '#') Console.ForegroundColor = ConsoleColor.Red;
                else if (c == '?') Console.ForegroundColor = ConsoleColor.Yellow;
                else if (c == '0') Console.ForegroundColor = ConsoleColor.DarkGreen;
                else if (c == '1') Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(c);
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
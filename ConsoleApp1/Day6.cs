namespace ConsoleApp1;

public class Day6 : Path
{
    
    
    public static void run()
    {
        // Day 4 Task 1:
        
        string path = PATH + "day6.txt";
        //path = PATH + "day6example.txt";
        string[] lines = File.ReadAllLines(path);

        Task(lines);
    }

    static void Task(string[] lines)
    {
        List<Race> races = new();
        string[]? distances = null, times = null;

        /*
        // For Task 1:
        foreach (string s in lines)
        {
            if (s.StartsWith("Time"))
            {
                String temp = s.Split(": ")[1].Trim();
                while (temp.Contains("  ")) temp = temp.Replace("  ", " ");
                times = temp.Split(' ');
                continue;
            }
            if (s.StartsWith("Distance"))
            {
                String temp = s.Split(": ")[1].Trim();
                while (temp.Contains("  ")) temp = temp.Replace("  ", " ");
                distances = temp.Split(' ');
            }
        }
        //*/
        
        //*
        // For Task 2:
        foreach (string s in lines)
        {
            if (s.StartsWith("Time"))
            {
                times = new string[] { s.Split(": ")[1].Replace(" ", "") };
                continue;
            }
            if (s.StartsWith("Distance"))
            {
                distances = new string[] { s.Split(": ")[1].Replace(" ", "") };
            }
        }
        //*/

        if (distances == null || times == null)
        {
            Console.Error.WriteLine("Error: Could not parse distances or times.");       // For Task 1, replace "  " with " "
            return;
        }
        if (distances.Length != times.Length)
        {
            Console.Error.WriteLine("Error: Distances and Times arrays have different lengths!");
            return;
        }

        for (int i = 0; i < distances.Length; i++)
        {
            Race race = new Race(times[i], distances[i]);
            races.Add(race);
            Console.WriteLine($"Race {races.Count} - Time: {race.Time}, Record: {race.RecordDistance}, Possibilities: {race.Possibilities}");
        }

        long product = 1;
        
        foreach (Race race in races)
        {
            product *= race.Possibilities;
        }
        
        Console.WriteLine($"The product of all possibilities of all races is {product}");
    }

    class Race
    {
        public long Time { get; private set; }
        public long RecordDistance { get; private set; }
        
        public long Possibilities { get; private set; }
        
        public Race(long time, long recordDistance)
        {
            this.Time = time;
            this.RecordDistance = recordDistance;
            CalculateWinningPossibilities();
        }
        public Race(string time, string recordDistance) : this(long.Parse(time), long.Parse(recordDistance))
        { }

        void CalculateWinningPossibilities()
        {
            long wins = 0;
            for (int hold = 1; hold < Time; hold++)
            {
                // speed/ms = hold
                long distance = hold * (Time - hold);    // speed * remainingTime
                if (distance > RecordDistance) wins++;
                else if (wins > 0) break;   // If it was possible but not anymore, we can stop
            }
            Possibilities = wins;
        }
    }
}
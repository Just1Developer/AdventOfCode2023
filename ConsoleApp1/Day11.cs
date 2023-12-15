namespace ConsoleApp1;

public class Day11 : Path
{
    public static void run()
    {
        string path = PATH + "day11.txt";
        //path = PATH + "day11example.txt";
        string[] lines = File.ReadAllLines(path);

        Task(lines);
    }

    private static void Task(string[] lines)
    {
        Space space = new Space(lines);
        ulong sum = 0;
        for (int i = 0; i < space.Galaxies.Count - 1; i++)
        {
            for (int j = i + 1; j < space.Galaxies.Count; j++)
            {
                ulong distance = space.Galaxies[i].Distance(space.Galaxies[j]);
                sum += distance;
                Console.WriteLine($"Distance between Galaxy {i} and {j}: {distance}");
            }
        }
        Console.WriteLine($"Sum of all distances: {sum}");
    }

    struct Space
    {
        private const int EMPTY_SPACE_SIZE = 1000000 - 1;
        
        private List<int> EmptyColumns = new ();
        internal List<Galaxy> Galaxies = new ();

        public Space(string[] lines)
        {
            // Assuming all lines are the same length
            for (int col = 0; col < lines[0].Length; col++)
            {
                bool found = false;
                for (int row = 0; row < lines.Length; row++)
                {
                    if (lines[row][col] != '#') continue;
                    found = true;
                    break;
                }
                if (found) continue;
                EmptyColumns.Add(col);
            }
            // Now we have every empty column
            // Now go through all the lines
            ulong spaceRow = 0;
            for (int row = 0; row < lines.Length; row++, spaceRow++)
            {
                ulong spaceCol = 0;
                bool found = false;
                for (int col = 0; col < lines[row].Length; col++, spaceCol++)
                {
                    if (EmptyColumns.Contains(col))
                    {
                        spaceCol += EMPTY_SPACE_SIZE;
                        continue;
                    }
                    if (lines[row][col] != '#') continue;
                    Galaxies.Add(new Galaxy(spaceCol, spaceRow));
                    found = true;
                }

                if (!found) spaceRow += EMPTY_SPACE_SIZE; // Found empty row, double the space from here on out
            }
        }
    }

    struct Galaxy
    {
        private readonly ulong X, Y;

        internal Galaxy(ulong x, ulong y)
        {
            X = x;
            Y = y;
        }

        internal ulong Distance(Galaxy other)
        {
            ulong xdiff = X < other.X ? other.X - X : X - other.X;
            ulong ydiff = Y < other.Y ? other.Y - Y : Y - other.Y;
            return xdiff + ydiff;
        }
    }
}
namespace ConsoleApp1
{
	internal class Day10 : Path
    {
        public static void run()
        {
            string path = PATH + "day10.txt";
            //path = PATH + "day10example.txt";
            //path = PATH + "day10example2.txt";
            //path = PATH + "day10example3.txt";
            //path = PATH + "day10example4.txt";
            //path = PATH + "day10example5.txt";
            string[] lines = File.ReadAllLines(path);

            Task(lines);
        }

        static void Task(string[] lines)
		{
            PipelineMap map = new PipelineMap(lines);
            Console.WriteLine("Furthest pipeline tile: " + Math.Ceiling(map.LoopLength / 2.0));
            Console.WriteLine("Trapped tiles: " + map.CalculateEnclosedTiles());
		}

        class PipelineMap
		{
            internal static readonly Coordinate
                NUL = new (0, 0),
                NORTH = new (-1, 0),
                SOUTH = new (1, 0),
                EAST = new (0, 1),
                WEST = new(0, -1);
            internal static readonly List<char>
                IS_NORTH_CONNECTED = new List<char> { 'F', '|', '7' },   // Right, Straight, Left
                IS_SOUTH_CONNECTED = new List<char> { 'L', '|', 'J' },   // Right, Straight, Left
                IS_EAST_CONNECTED = new List<char> { '7', '-', 'J' },    // Down, Straight, Up
                IS_WEST_CONNECTED = new List<char> { 'F', '-', 'L' };    // Down, Straight, Up

            private List<char> parallel_UpDown = new() { 'F', 'L', 'J', '7', '|' };
            private List<char> parallel_LeftRight = new() { 'F', 'L', 'J', '7', '-' };
            
            List<char> GetParallelTiles(char parallel)
            {
                return null;
            }

            MapWrapper Map;
            Coordinate? Start;
            internal int LoopLength = -1;
            List<int> PipelineTilesHashes = new List<int>();

            internal PipelineMap(string[] lines)
			{
                Map = new MapWrapper(lines);

                // Find start
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    for (int j = 0; j < line.Length; j++)
					{
                        char c = line[j];
                        if (c == 'S')
						{
                            Start = new Coordinate(i, j);
                            i = lines.Length;
                            break;
						}
					}
                }
                if (Start == null) { Start = new(0, 0); return; }

                // Go through the Loop and count steps, the farthest is the middle of the number of steps
                char currentChar;
                int steps = 0;
                Coordinate current = Start, previousDir = NUL;
                do
                {
                    Coordinate nextDir = NextTileDirection(current, previousDir);
                    if (nextDir == NUL) throw new PathTooLongException($"Found no valid path from here; Coordinate = {current}");
                    previousDir = nextDir;
                    current += nextDir;
                    currentChar = Map[current];
                    steps++;
                    PipelineTilesHashes.Add(current.GetHashCode());
                } while (currentChar != 'S');
                LoopLength = steps;
            }

            internal Coordinate NextTileDirection(Coordinate coord, Coordinate previousDirection)
			{
                char currentChar = Map[coord];
                // Todo look if my part can actually go there too tho
                // Structure:
                // If (Not the direction from which i came   AND   (Tile in direction is connected OR Next location is Start point (finished))
                if (previousDirection != SOUTH && (IS_NORTH_CONNECTED.Contains(Map[coord + NORTH]) || Map[coord + NORTH] == 'S')
                    && (IS_SOUTH_CONNECTED.Contains(currentChar) || currentChar == 'S')) return NORTH;
                if (previousDirection != NORTH && (IS_SOUTH_CONNECTED.Contains(Map[coord + SOUTH]) || Map[coord + SOUTH] == 'S')
                    && (IS_NORTH_CONNECTED.Contains(currentChar) || currentChar == 'S')) return SOUTH;
                if (previousDirection != WEST && (IS_EAST_CONNECTED.Contains(Map[coord + EAST]) || Map[coord + EAST] == 'S')
                    && (IS_WEST_CONNECTED.Contains(currentChar) || currentChar == 'S')) return EAST;
                if (previousDirection != EAST && (IS_WEST_CONNECTED.Contains(Map[coord + WEST]) || Map[coord + WEST] == 'S')
                    && (IS_EAST_CONNECTED.Contains(currentChar) || currentChar == 'S')) return WEST;
                return NUL;
			}

            internal int CalculateEnclosedTiles()
			{
                // Go through each tile. Every tile will be assigned one of these values:
                // [NEW]:
                // -3: Squeezable (insert) (1), 1s have squeezed through here. This counts as 1. Maybe scrap this and directly make into 1
                // -2: Squeezable (insert), 1s can squeeze through here. To be inserted between pipelines
                // -1: Tile is a pipeline
                // 0: Tile might be trapped: This is because this is the default value.
                // 1: Tile is definitely free
                int[,] statusMap = new int[Map.Width, Map.Height];
                Coordinate coord = new Coordinate(0, 0);
                for (int x = 0; x < Map.Width; x++, coord += EAST, coord.X = 0)
                {
                    for (int y = 0; y < Map.Height; y++, coord += SOUTH)
                    {
                        if (PipelineTilesHashes.Contains(coord.GetHashCode()))
						{
                            statusMap[x, y] = -1;
                            // Check if right is parallel
                            if (y < Map.Height - 1)
                            {
                                y++;
                                
                            }
                            continue;
                        }
                        if (Map.IsOnEdge(coord))
                        {
                            statusMap[x, y] = 1;
                            continue;
                        }
                    }
                }
                print();

                for (int run = 0; run < Math.Max(Map.Height, Map.Width) - 2; run++)
                {
                    //print();
                    // Every run, go through the entire thing and set all neighbors of 1s to also be 1s
                    for (int x = 0; x < Map.Width; x++, coord += EAST)
                    {
                        for (int y = 0; y < Map.Height; y++, coord += SOUTH)
                        {
                            if (statusMap[x, y] != 1) continue;
                            // Now, actually check the neighbors. There is more logic behind this, but it can be simplified down to: Has free Neighbor? True
                            // This tile is guaranteed to not be on the edge, so it has something in every direction.
                            // This could be done with a loop
                            for (int x1 = -1; x1 < 2; x1++)
                            {
                                if (x + x1 < 0) continue;
                                if (x + x1 >= Map.Width) break;

                                for (int y1 = -1; y1 < 2; y1++)
                                {
                                    if (y + y1 < 0) continue;
                                    if (y + y1 >= Map.Height) break;

                                    //if (x1 == 0 && y1 == 0) continue;
                                    if (statusMap[x + x1, y + y1] == 0)
                                    {
                                        statusMap[x + x1, y + y1] = 1;
                                    }
                                }
                            }
                        }
                    }
                }
                print();

                int trapped = 0;
                foreach (int i in statusMap)
				{
                    if (i == 0) trapped++;
                }

                void print()
				{
                    Console.WriteLine("StatusMap:");
                    for (int y = 0; y < Map.Height; y++)
                    {
                        for (int x = 0; x < Map.Width; x++)
                        {
                            int i = statusMap[x, y];
                            if (i == -1)
                            {
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write("#");
                            }
                            else if (i == -2)
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write(":");
                            }
                            else if (i == -3)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.Write("1");
                            }
                            else
                            {
                                if (i == 0) Console.ForegroundColor = ConsoleColor.Red;
                                else if (i == 1) Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.Write(statusMap[x, y]);
                            }
                        }
                        Console.Write('\n');
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                return trapped;
            }
		}

        internal class Coordinate
		{
            public int X, Y;

            public Coordinate(int x, int y)
			{
                this.X = x;
                this.Y = y;
            }

            public static bool Equals(Coordinate coord, Coordinate other)
            {
                return coord == other;
            }

            public override int GetHashCode()
            {
                return (X << 15) + Y;
            }

            public Coordinate FromHashCode(int hash)
            {
                X = hash >> 12;
                Y = hash & ((1 << 15) - 1);
                return new Coordinate(X, Y);
            }

            public static Coordinate operator +(Coordinate? coord, Coordinate? other)
            {
                if (coord == null)
                {
                    if (other == null) return new(0, 0);
                    return other;
                }
                if (other == null)
                {
                    return coord;
                }
                return new Coordinate(coord.X + other.X, coord.Y + other.Y);
            }

            public static Coordinate operator -(Coordinate? coord, Coordinate? other)
            {
                if (coord == null)
				{
                    if (other == null) return new(0, 0);
                    return other;
				}
                if (other == null)
				{
                    return coord;
				}
                return new Coordinate(coord.X - other.X, coord.Y - other.Y);
			}

            public static bool operator ==(Coordinate? coord, Coordinate? other)
            {
                if (object.Equals(coord, null) && object.Equals(other, null)) return true;
                if (object.Equals(coord, null)) return false;
                if (object.Equals(other, null)) return false;
                return coord.X == other.X && coord.Y == other.Y;
			}

            public static bool operator !=(Coordinate? coord, Coordinate? other)
            {
                if (coord == null && other == null) return true;
                if (other == null) return false;
                if (coord == null) return false;
                return coord.X != other.X || coord.Y != other.Y;
			}

            public override bool Equals(object? other)
			{
                if (other == null) return false;
                if (other.GetType() != typeof(Coordinate)) return false;
                return this == (Coordinate) other;
			}

			public override string ToString()
			{
                return $"({X}, {Y})";
			}

            public static int GetCoordHash(int x, int y)
            {
                // Same calculation, but without coordinate object
                return (x << 15) + y;
            }
		}

        internal class MapWrapper
        {
            private string[] Map;
            internal readonly int Width, Height;

            public MapWrapper(string[] map)
            {
                this.Map = map ?? throw new ArgumentNullException(nameof(map));
                Height = map.Length;
                Width = Height == 0 ? 0 : map[0].Length;  // Assume map is not an empty array.
            }

            internal char this[Coordinate coord]
            {
                get
                {
                    if (coord == null)
                    {
                        throw new ArgumentNullException(nameof(coord));
                    }

                    if (coord.X < 0 || coord.X >= Map.Length || coord.Y < 0 || coord.Y >= Map[coord.X].Length)
                    {
                        // Coordinate is out of range, return char that will definitely return false in the contains check
                        return 'ö';
                    }

                    return Map[coord.X][coord.Y];
                }
            }

            internal bool IsOnEdge(Coordinate coord)
			{
                if (coord.X == 0) return true;
                if (coord.X == Height - 1) return true;
                if (coord.Y == 0) return true;
                if (coord.Y == Width - 1) return true;
                return false;
			}
        }
        
        internal class DoubleSizeMapWrapper
        {
            private char[][] CharMap;
            private int[][] Map;
            internal readonly int Width, Height;

            public DoubleSizeMapWrapper(string[] map, List<int> loopTileHashes)
            {
                // Insertions
                Map = new int[map.Length * 2 - 1][];
                CharMap = new char[map.Length * 2 - 1][];
                int arrLen = map[0].Length * 2 - 1;
                
                Height = map.Length;
                Width = Height == 0 ? 0 : map[0].Length;  // Assume map is not an empty array.
                
                // Integer codes:
                // 0: Potentially enclosed tile.
                // 1: Free tile
                // -1: Loop Tile - also in between
                // -2: Squeezable tile (in between) (enclosed)
                // -3: Squeezable tile (free)
                
                // First, fill in the normal map
                for (int y = 0; y < map.Length; y += 2)
                {
                    // New line
                    Map[y] = new int[arrLen];
                    CharMap[y] = new char[arrLen];
                    
                    for (int x = 0; x < map[y].Length; x += 2)
                    {
                        // If this should be != 0, set value
                        if (loopTileHashes.Contains(Coordinate.GetCoordHash(x / 2, y / 2)))
                        {
                            Map[y][x] = -1; // Loop tile
                            CharMap[y][x] = map[y / 2][x / 2];
                            continue;
                        }
                        if (IsOnEdge(x, y))
                        {
                            Map[y][x] = 1;  // Free tile
                        }
                        CharMap[y][x] = '.';
                    }
                }
                
                // Now that we have the normal map, let's fill in the gap rows and columns
                // This is for the rows only:
                for (int y = 1; y < map.Length; y += 2)
                {
                    // New line
                    Map[y] = new int[arrLen];
                    CharMap[y] = new char[arrLen];
                    
                    for (int x = 1; x < map[y].Length; x += 2)
                    {
                        if (x == y)
                        {
                            Map[y][x] = -3;  // Not countable i think
                            CharMap[y][x] = '*';
                            continue;
                        }
                        // Determine squeeze-ability: Are the map tiles below squeezable?
                        // Better yet, are they NOT squeezable?
                        if (PipelineMap.IS_NORTH_CONNECTED.Contains(CharMap[y - 1][x])
                            && PipelineMap.IS_SOUTH_CONNECTED.Contains(CharMap[y + 1][x]))
                        {
                            Map[y][x] = -3; // Loop tile
                            CharMap[y][x] = map[y / 2][x / 2];
                            continue;
                        }
                        Map[y][x] = -2;  // Free tile, squeezable
                        CharMap[y][x] = '*';
                    }
                }
                
                for (int i = 0; i < Map.Length; i++)
                {
                    // New line
                    Map[i] = new int[arrLen];

                    if (i % 2 == 0)
                    {
                        // Normal line
                        //for (int x = 0; x < arrLen; x += 2)
                        //{
                        //    // Fill
                        //}
                    }
                    else
                    {
                        // In-between line
                    }
                }
            }

            internal void Insert(int x, int y)
            {
                
            }
            
            internal int this[int x, int y]
            {
                get
                {
                    if (x < 0 || x >= Map.Length || y < 0 || y >= Map[x * 2 - 1].Length)
                    {
                        // Coordinate is out of range, return char that will definitely return false in the contains check
                        return 'ö';
                    }

                    return Map[x * 2 - 1][y * 2 - 1];
                }
            }

            internal bool IsOnEdge(Coordinate coord)
            {
                if (coord.X == 0) return true;
                if (coord.X == Height - 1) return true;
                if (coord.Y == 0) return true;
                if (coord.Y == Width - 1) return true;
                return false;
            }

            internal bool IsOnEdge(int x, int y)
            {
                // Adjust x and y:
                x /= 2;
                y /= 2;
                if (x == 0) return true;
                if (x == Height - 1) return true;
                if (y == 0) return true;
                if (y == Width - 1) return true;
                return false;
            }
        }
    }
}

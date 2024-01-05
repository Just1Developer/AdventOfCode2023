using System.Text;

namespace ConsoleApp1;

public class Day17 : Path
{
    private static int DayID = 17;
    public static void run()
    {
        string path = $"{PATH}day{DayID}.txt";
        path = $"{PATH}day{DayID}example.txt";		// Should be 102 or 109
        var file = File.ReadAllLines(path);

        _Task1(file);
    }

    private static void _Task1(string[] content)
    {
        var pathfinder = new DijkstrasPathfinder(content);
        int lowestHeatLoss = pathfinder.GetLowestHeatLoss();
        
        Console.WriteLine($"Finished. Path of lowest heat loss has heat loss of: {lowestHeatLoss}");
    }

    internal class DijkstrasPathfinder
    {
	    private const int MAX_CONSECUTIVE_MOVEMENTS = 3;

	    // Index using Direction enum
	    private static Coordinate[] DefaultCoords =
	    {
		    new Coordinate(-1, 0),	// UP
		    new Coordinate(1, 0),	// DOWN
		    new Coordinate(0, -1),	// LEFT
		    new Coordinate(0, 1),	// RIGHT
	    };

	    private static Direction[] OppositeDirections =
	    {
		    Direction.DOWN,
		    Direction.UP,
		    Direction.RIGHT,
		    Direction.LEFT
	    };
	    
	    public class NodeState
	    {
		    public Coordinate Position { get; set; }
		    private List<Direction> MovementHistory { get; set; }

		    // Constructor and other methods
		    internal NodeState(Coordinate coordinate)
		    {
			    this.Position = coordinate;
			    MovementHistory = new();
		    }
		    
		    // Override Equals and GetHashCode
		    public override bool Equals(object obj)
		    {
			    return obj is NodeState other &&
			           Position.Equals(other.Position) &&
			           Enumerable.SequenceEqual(MovementHistory, other.MovementHistory);
		    }

		    public override int GetHashCode()
		    {
			    unchecked
			    {
				    int hash = 17;
				    hash = hash * 23 + Position.GetHashCode();
				    foreach (var direction in MovementHistory)
				    {
					    hash = hash * 23 + direction.GetHashCode();
				    }
				    return hash;
			    }
		    }

		    private void Move(Direction direction)
		    {
			    if (!IsAllowed(direction)) return;
			    AddRecentDirection(direction);
			    Position += DefaultCoords[(int) direction];
		    }
		    
		    private void AddRecentDirection(Direction direction)
		    {
			    while (MovementHistory.Count >= MAX_CONSECUTIVE_MOVEMENTS)
			    {
				    MovementHistory.RemoveAt(0);
			    }
			    MovementHistory.Add(direction);
		    }

		    internal bool IsAllowed(MapWrapper map, Direction direction)
		    {
			    if (map.IsOffMap(Position + DefaultCoords[(int)direction])) return false;
			    return IsAllowed(direction);
		    }
		    private bool IsAllowed(Direction direction)
		    {
			    if (MovementHistory.Count == 0) return true;
			    // Check if last move was in opposite direction
			    if (MovementHistory[MovementHistory.Count - 1] == OppositeDirections[(int) direction]) return false;
			    if (MovementHistory.Count < MAX_CONSECUTIVE_MOVEMENTS) return true;
			    foreach (var dir in MovementHistory)
				    if (dir != direction)
					    return true;
			    return false;
		    }

		    private NodeState Clone()
		    {
			    List<Direction> History = new(MovementHistory);
			    return new NodeState(Position)
			    {
				    MovementHistory = History,
			    };
		    }

		    internal NodeState Clone(Direction move)
		    {
			    NodeState newNode = Clone();
			    newNode.Move(move);
			    return newNode;
		    }
	    }
	    
	    private MapWrapper Map;
	    private int[,] distances;
	    private bool[,] visited;
	    private int width, height;

	    public DijkstrasPathfinder(string[] map)
	    {
		    this.Map = new MapWrapper(map);
		    this.width = Map.Width;
		    this.height = Map.Height;
		    this.distances = new int[width, height];
		    this.visited = new bool[width, height];

		    for (int i = 0; i < width; i++)
			    for (int j = 0; j < height; j++)
				    distances[i, j] = int.MaxValue;
		    
		    // Print
		    PrintMap();
	    }

	    void PrintMap()
	    {
		    Console.WriteLine("Map:");
		    Console.Write(Map.ToString());
	    }

	    void PrintPath(List<Coordinate> path)
	    {
		    Console.Write("Path: ");
		    for (int i = 0; i < path.Count; i++)
		    {
			    Console.Write(path[i].ToString());
			    if (i < path.Count - 1) Console.Write(" => ");
		    }
		    Console.WriteLine();
	    }

	    internal int GetLowestHeatLoss()
		    => GetLowestHeatLoss(new Coordinate(0, 0), new Coordinate(Map.Height - 1, Map.Width - 1));
	    internal int GetLowestHeatLoss(Coordinate start, Coordinate destination)
	    {
		    int heatloss = 0;

		    var path = FindShortestPath(start, destination);
		    foreach (var coord in path)
		    {
			    heatloss += Map[coord];
		    }
		    
		    Console.WriteLine("Start: " + start);
		    Console.WriteLine("Dest: " + destination);
		    PrintPath(path);

		    heatloss -= Map[start];	// Should not be counted. I am uncertain if the start is in the list
		    
		    return heatloss;
	    }

	    // By GPT-4
	    private List<Coordinate> FindShortestPath(Coordinate start, Coordinate destination)
	    {
		    var pq = new PriorityQueue<NodeState, int>();
		    var prev = new Dictionary<NodeState, NodeState>();
		    var startState = new NodeState(start);
    
		    distances[start.Row, start.Col] = 0;
		    pq.Enqueue(startState, 0);

		    while (pq.Count > 0)
		    {
			    var currentState = pq.Dequeue();
			    if (currentState.Position.Equals(destination))
			    {
				    return ConstructPath(prev, currentState);
			    }

			    foreach (var neighborState in GetValidNeighbors(currentState))
			    {
				    var neighbor = neighborState.Position;
				    if (!visited[neighbor.Row, neighbor.Col])
				    {
					    int newDist = distances[currentState.Position.Row, currentState.Position.Col] + Map[neighbor];
					    if (newDist < distances[neighbor.Row, neighbor.Col])
					    {
						    distances[neighbor.Row, neighbor.Col] = newDist;
						    prev[neighborState] = currentState;
						    pq.Enqueue(neighborState, newDist);
					    }
				    }
			    }
		    }

		    return new List<Coordinate>(); // Empty list if path not found
	    }

	    private IEnumerable<NodeState> GetValidNeighbors(NodeState currentState)
	    {
		    // Implement logic to return valid neighboring nodes considering the movement history.
		    // Exclude moves that would go back the way you came.
		    // Limit moves in the same direction to a maximum of three consecutive times.

		    List<NodeState> it = new();
		    
		    // We just need to iterate, order doesnt matter, so we can use our collection of opposite directions
		    foreach (Direction dir in OppositeDirections)
		    {
			    if (currentState.IsAllowed(Map, dir))
				    it.Add(currentState.Clone(dir));
		    }
		    
		    return it;
	    }
	    
	    private List<Coordinate> ConstructPath(Dictionary<NodeState, NodeState> prev, NodeState destination)
	    {
		    var path = new List<Coordinate>();
		    for (var at = destination; at != null; at = prev.GetValueOrDefault(at))
		    {
			    path.Add(at.Position);
		    }
		    path.Reverse();
		    return path;
	    }
	    
	    private List<Coordinate> _ConstructPathOld(Dictionary<Coordinate, Coordinate> prev, Coordinate destination)
	    {
		    var path = new List<Coordinate>();
		    for (var at = destination; at != null; at = prev.GetValueOrDefault(at))
		    {
			    path.Add(at);
		    }
		    path.Reverse();
		    return path;
	    }
    }
    
	internal class Coordinate
	{
		public int Row, Col;
		
		public Coordinate(int row, int col)
		{
			this.Row = row;
			this.Col = col;
		}

		public static bool Equals(Coordinate coord, Coordinate other)
		{
			return coord == other;
		}

		public Coordinate Clone()
		{
			return new Coordinate(Row, Col);
		}

		public override int GetHashCode()
		{
			return (Row << 15) + Col;
		}

		public Coordinate FromHashCode(int hash)
		{
			Row = hash >> 12;
			Col = hash & ((1 << 15) - 1);
			return new Coordinate(Row, Col);
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
			return new Coordinate(coord.Row + other.Row, coord.Col + other.Col);
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
			return new Coordinate(coord.Row - other.Row, coord.Col - other.Col);
		}

		public static bool operator ==(Coordinate? coord, Coordinate? other)
		{
			if (object.Equals(coord, null) && object.Equals(other, null)) return true;
			if (object.Equals(coord, null)) return false;
			if (object.Equals(other, null)) return false;
			return coord.Row == other.Row && coord.Col == other.Col;
		}

		public static bool operator !=(Coordinate? coord, Coordinate? other)
		{
			if (coord == null && other == null) return true;
			if (other == null) return false;
			if (coord == null) return false;
			return coord.Row != other.Row || coord.Col != other.Col;
		}

		public override bool Equals(object? other)
		{
			if (other == null) return false;
			if (other.GetType() != typeof(Coordinate)) return false;
			return this == (Coordinate)other;
		}

		public override string ToString()
		{
			return $"({Row}, {Col})";
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

		internal int this[Coordinate coord]
		{
			get
			{
				if (coord == null)
				{
					throw new ArgumentNullException(nameof(coord));
				}

				if (IsOffMap(coord))
				{
					// Coordinate is out of range, return char that will definitely return false in the contains check
					return 0;
				}

				return Map[coord.Row][coord.Col] - '0';		// Guaranteed to be a number char. '0' - '0' = 0, '1' - '0' = 1 usw.
			}
		}
		internal bool IsOffMap(Coordinate coord)
		{
			if (coord.Row < 0) return true;
			if (coord.Row >= Height) return true;
			if (coord.Col < 0) return true;
			if (coord.Col >= Width) return true;
			return false;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			foreach (var s in Map)
			{
				builder.Append(s);
				builder.Append('\n');
			}
			return builder.ToString();
		}
	}

	internal enum Direction
	{
		UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3
	}
}
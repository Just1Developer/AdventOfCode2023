using System.Drawing;
using System.Text;
using System.Xml.Schema;

namespace ConsoleApp1;

public class Day18 : Path
{
	private static int DayID = 18;

	public static void run()
	{
		string path = $"{PATH}day{DayID}.txt";
		//path = $"{PATH}day{DayID}example.txt"; // Should be 102 or 109
		var file = File.ReadAllLines(path);

		_Task1(file);
	}

	private static void _Task1(string[] content)
	{
		CompositeShape shape = new CompositeShape();
		foreach (string line in content)
		{
			Vertex v = Vertex.Parse(line, shape);
			//Console.WriteLine($"Adding Vertex: {v}");
			shape.Add(v);
			//Console.WriteLine($"Shape Dimensions: {shape.GetDimensions()}");
		}
		
		Console.Write(shape.ToString());
		Console.WriteLine("\n----------Filled:-----------\n");
		Console.Write(shape.ToString(true));
		
		Console.WriteLine($"Finished. Total area: {shape.GetTotalAreaTask1()}");
	}

	internal class Vertex
	{
		internal Coordinate Location;
		internal string HexCode;

		internal Vertex(Coordinate loc, string hexCode)
		{
			this.Location = loc;
			this.HexCode = hexCode;
		}

		internal Vertex Next(int delta, Direction deltaDirection, string newHexCode)
		{
			Coordinate newCoord = this.Location + (Coordinate.Defaults[(int) deltaDirection] * delta);
			return new Vertex(newCoord, newHexCode);
		}

		internal static Vertex Next(Vertex previousVertex, int delta, Direction deltaDirection, string newHexCode)
		{
			Coordinate newCoord = previousVertex.Location + (Coordinate.Defaults[(int) deltaDirection] * delta);
			return new Vertex(newCoord, newHexCode);
		}

		internal static Vertex GetFromDelta(int delta, Direction deltaDirection, string newHexCode)
		{
			Coordinate newCoord = Coordinate.Defaults[(int) deltaDirection] * delta;
			return new Vertex(newCoord, newHexCode);
		}

		internal static Vertex Parse(string line, CompositeShape shape)
			=> Parse(line, shape.GetLast());
		internal static Vertex Parse(string line, Vertex? referenceVertex = null)
		{
			//Vertex oldVertex = referenceVertex ?? new Vertex();
			string[] contents = line.Trim().Split(' ');
			Direction dir = GetDirection(contents[0][0]);
			int len = contents[1][0] - '0';
			string hex = contents[2].Substring(1, 7);

			if (referenceVertex == null) return GetFromDelta(len, dir, hex);
			return referenceVertex.Next(len, dir, hex);
		}

		private static Direction GetDirection(char c)
		{
			return c switch
			{
				'U' => Direction.UP,
				'R' => Direction.RIGHT,
				'L' => Direction.LEFT,
				_ => Direction.DOWN
			};
		}

		public override string ToString()
		{
			return $"{{Location: {Location}, Hex: {HexCode}}}";
		}
	}
	
	internal class CompositeShape
	{
		// Although this would be cool, I added found a different, much easier method.
		// May use this for part 2.
		private int MaxWidth, MaxHeight, MinWidth, MinHeight;
		private List<Vertex> Vertices = new ();

		public CompositeShape() : this(0, 0, 0, 0) { }
		public CompositeShape(int maxWidth, int maxHeight, int minWidth, int minHeight)
		{
			this.MaxWidth = maxWidth;
			this.MaxHeight = maxHeight;
			this.MinWidth = minWidth;
			this.MinHeight = minHeight;
		}

		public int GetTotalAreaTask1()
		{
			int area = 0;
			
			for (int row = MinHeight; row <= MaxHeight; row++)
			{
				bool isInShape = false;
				bool wasLastEdge = false;
				for (int col = MinWidth; col <= MaxWidth; col++)
				{
					if (IsOnEdge(row, col))
					{
						// Edge is in area
						area++;
						//isInShape = !isInShape;
						wasLastEdge = true;
						continue;
					}
					
					if (wasLastEdge) isInShape = !isInShape;
					if (isInShape) area++;
					wasLastEdge = false;
				}
			}
			return area;
		}
		
		public void Add(Vertex vertex)
		{
			Vertices.Add(vertex);
			if (vertex.Location.Col > MaxWidth)
				MaxWidth = vertex.Location.Col;
			else if (vertex.Location.Col < MinWidth)
				MinWidth = vertex.Location.Col;
			
			if (vertex.Location.Row > MaxHeight)
				MaxHeight = vertex.Location.Row;
			else if (vertex.Location.Row < MinHeight)
				MinHeight = vertex.Location.Row;
		}

		public Vertex? GetLast()
		{
			if (Vertices.Count == 0) return null;
			return Vertices[Vertices.Count - 1];
		}

		// Not the most efficient
		public bool Contains(Coordinate location)
		{
			return Contains(location.GetHashCode());
		}
		// Not the most efficient
		private bool Contains(int coordHash)
		{
			foreach (var vertex in Vertices)
			{
				if (vertex.Location.GetHashCode() == coordHash) return true;
			}
			return false;
		}

		private bool IsOnEdge(int row, int col)
		{
			// Is between consecutive pair?
			for (int i = 0; i < Vertices.Count; i++)
			{
				Coordinate v1 = Vertices[i].Location, v2 = Vertices[(i + 1) % Vertices.Count].Location;
				// Check row:
				if (v1 == v2)
				{
					if (v1.GetHashCode() == Coordinate.GetCoordHash(row, col)) return true;
					continue;
				}
				if (v1.Row == v2.Row)
				{
					if (row != v1.Row) continue;
					if (Math.Min(v1.Col, v2.Col) > col) continue;
					if (Math.Max(v1.Col, v2.Col) < col) continue;
					return true;
				}

				if (v1.Col == v2.Col)
				{
					if (col != v1.Col) continue;
					if (Math.Min(v1.Row, v2.Row) > row) continue;
					if (Math.Max(v1.Row, v2.Row) < row) continue;
					return true;
				}
			}

			return false;
		}

		private const char SHAPE_CHAR = '#';
		private const char GROUND_CHAR = '.';
		
		public override string ToString() => ToString(false);
		public string ToString(bool filled)
		{
			StringBuilder builder = new StringBuilder($"Shape: Dimensions: {GetDimensions()}; Form:\n");
			
			for (int row = MinHeight; row <= MaxHeight; row++)
			{
				bool isInShape = false;
				bool wasLastEdge = false;
				for (int col = MinWidth; col <= MaxWidth; col++)
				{
					if (IsOnEdge(row, col))
					{
						// Edge is in area
						builder.Append(SHAPE_CHAR);
						//isInShape = !isInShape;
						wasLastEdge = true;
						continue;
					}
					if (wasLastEdge) isInShape = !isInShape;
					if (isInShape && filled) builder.Append(SHAPE_CHAR);
					else builder.Append(GROUND_CHAR);
					wasLastEdge = false;
				}
				builder.Append('\n');
			}
			return builder.ToString();
		}
		
		/*
		public Bitmap ToBitmap(bool filled = false)
		{
			Bitmap bmp = new Bitmap(MaxWidth - MinWidth + 1, MaxHeight - MinHeight + 1);
			StringBuilder builder = new StringBuilder($"Shape: Dimensions: {GetDimensions()}; Form:\n");
			
			for (int row = MinHeight; row <= MaxHeight; row++)
			{
				bool isInShape = false;
				bool wasLastEdge = false;
				for (int col = MinWidth; col <= MaxWidth; col++)
				{
					if (IsOnEdge(row, col))
					{
						// Edge is in area
						builder.Append(SHAPE_CHAR);
						//isInShape = !isInShape;
						wasLastEdge = true;
						continue;
					}
					if (wasLastEdge) isInShape = !isInShape;
					if (isInShape && filled) builder.Append(SHAPE_CHAR);
					else builder.Append(GROUND_CHAR);
					wasLastEdge = false;
				}
				builder.Append('\n');
			}
			return builder.ToString();
		}
		//*/
		
		public string GetDimensions()
		{
			// +1 because 0 is inclusive
			return $"Width: ({MinWidth}-{MaxWidth}) x Height: ({MinHeight}-{MaxHeight}) => {MaxWidth - MinWidth + 1} x {MaxHeight - MinHeight + 1}";
		}
	}

	internal class Coordinate
	{
		// Index using Direction enum
		public static readonly Coordinate[] Defaults = new []
		{
			new Coordinate(-1, 0),	// UP
			new Coordinate(1, 0),	// DOWN
			new Coordinate(0, -1),	// LEFT
			new Coordinate(0, 1),	// RIGHT
		};

		public static readonly Direction[] OppositeDirections = new []
		{
			Direction.DOWN,
			Direction.UP,
			Direction.RIGHT,
			Direction.LEFT
		};
		
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
			return GetCoordHash(Row, Col);
		}

		public Coordinate FromHashCode(int hash)
		{
			Row = hash >> 12;
			Col = hash & ((1 << 16) - 1);
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

		public static Coordinate operator *(Coordinate? coord, int other)
		{
			if (coord == null) return new (0, 0);
			return new Coordinate(coord.Row * other, coord.Col * other);
		}

		public static Coordinate operator *(Coordinate? coord, Coordinate? other)
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
			return new Coordinate(coord.Row * other.Row, coord.Col * other.Col);
		}

		public static Coordinate operator /(Coordinate? coord, int other)
		{
			if (coord == null) return new (0, 0);
			return new Coordinate(coord.Row / other, coord.Col / other);
		}

		public static Coordinate operator /(Coordinate? coord, Coordinate? other)
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
			return new Coordinate(coord.Row / other.Row, coord.Col / other.Col);
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

		public static int GetCoordHash(int row, int col)
		{
			// Same calculation, but without coordinate object
			return (row << 16) + col;
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
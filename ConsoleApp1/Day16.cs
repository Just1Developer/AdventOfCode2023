using System.Text;

namespace ConsoleApp1
{
	internal class Day16 : Path
	{
		private static int DayID = 16;
		public static void run()
		{
			string path = $"{PATH}day{DayID}.txt";
			//path = $"{PATH}day{DayID}example.txt";
			var file = File.ReadAllLines(path);

			_Task1(file);
		}

		private static void _Task1(string[] content)
		{
			//RayTracer tracer = new RayTracer(content);
			int fields = RayTracer.MaximumEnergized(content);
			//Console.WriteLine("\n-------------------------\n");
			//Console.Write(tracer.Map.ToStringHighlighted());

			Console.WriteLine($"Finished. Maximum number of energized Fields: {fields}");
		}

		class RayTracer
		{
			internal static int MaximumEnergized(string[] map)
			{
				int maxEnergized = 0;
				int rows = map.Length;
				int columns = map[0].Length;
				// Now, go along the edges and maximize
				// First, Top and bottom
				for (int col = 0; col < columns; col++)
				{
					RayTracer tracer = new RayTracer(map,
						new Ray(new Coordinate(-1, col), Direction.SOUTH));
					maxEnergized = Math.Max(maxEnergized, tracer.GetHighlightedFields());

					tracer = new RayTracer(map,
						new Ray(new Coordinate(rows, col), Direction.NORTH));
					maxEnergized = Math.Max(maxEnergized, tracer.GetHighlightedFields());
				}
				for (int row = 0; row < rows; row++)
				{
					RayTracer tracer = new RayTracer(map,
						new Ray(new Coordinate(row, -1), Direction.EAST));
					maxEnergized = Math.Max(maxEnergized, tracer.GetHighlightedFields());

					tracer = new RayTracer(map,
						new Ray(new Coordinate(row, columns), Direction.WEST));
					maxEnergized = Math.Max(maxEnergized, tracer.GetHighlightedFields());
				}

				return maxEnergized;
			}

			private const char ENCOUNTER_MIRROR_L = '/';
			private const char ENCOUNTER_MIRROR_R = '\\';
			private const char ENCOUNTER_SPLITTER_HOR = '-';
			private const char ENCOUNTER_SPLITTER_VER = '|';
			private const char ENCOUNTER_NEUTRAL = '.';

			private List<Ray> Rays = new();
			internal MapWrapper Map;

			public RayTracer(string[] map)
			{
				Map = new MapWrapper(map);
				Console.Write(Map);
				Rays.Add(new Ray(new Coordinate(0, -1), Direction.EAST));
			}
			private RayTracer(string[] map, Ray startRay)
			{
				Map = new MapWrapper(map);
				Rays.Add(startRay);
			}

			internal int GetHighlightedFields()
			{
				Trace();
				return Map.HighlightedFields();
			}

			private void Trace()
			{
				Map.ResetHighlights();
				int i = 0;
				while (Rays.Count > 0)
				{
					//Console.WriteLine($"Starting Iteration {++i}");
					Move();
				}
			}

			private static readonly Coordinate
				EAST = new (0, 1),
				WEST = new (0, -1),
				NORTH = new (-1, 0),
				SOUTH = new (1, 0);

			private void Move()
			{
				for (int i = 0; i < Rays.Count; i++)
				{
					Ray ray = Rays[i];
					if (!ray.Go)
					{
						Rays.Remove(ray);
						i--;
						continue;
					}

					// Get next coordinate: Get next tile
					// First: Move
					switch (ray.Direction)
					{
						case Direction.EAST:
							ray.Location += EAST;
							break;
						case Direction.WEST:
							ray.Location += WEST;
							break;
						case Direction.NORTH:
							ray.Location += NORTH;
							break;
						case Direction.SOUTH:
							ray.Location += SOUTH;
							break;
					}

					if (Map.IsOffMap(ray.Location))
					{
						ray.Kill();
						continue;
					}

					// Is on map. Now, update the direction.
					char encounter = Map[ray.Location];
					var newDirection = Reflection(ray.Direction, encounter);
					if (newDirection == Direction.NORTHSOUTH)
					{
						Rays.Add(new Ray(ray.Location.Clone(), Direction.SOUTH));
						ray.Direction = Direction.NORTH;
					}
					else if (newDirection == Direction.EASTWEST)
					{
						Rays.Add(new Ray(ray.Location.Clone(), Direction.WEST));
						ray.Direction = Direction.EAST;
					}
					else
					{
						ray.Direction = newDirection;
					}

					bool success = Map.HighlightField(ray.Location, ray.Direction);
					if (!success) ray.Kill();	// No infinite loops
				}
			}

			static Direction Reflection(Direction currentDirection, char encounterChar)
			{
				if (currentDirection == Direction.EASTWEST || currentDirection == Direction.NORTHSOUTH)
					return currentDirection;

				switch (encounterChar)
				{
					case ENCOUNTER_NEUTRAL:
						return currentDirection;
					case ENCOUNTER_MIRROR_L:
						return currentDirection switch
						{
							Direction.EAST => Direction.NORTH,
							Direction.WEST => Direction.SOUTH,
							Direction.NORTH => Direction.EAST,
							_ => Direction.WEST,
						};
					case ENCOUNTER_MIRROR_R:
						return currentDirection switch
						{
							Direction.EAST => Direction.SOUTH,
							Direction.WEST => Direction.NORTH,
							Direction.NORTH => Direction.WEST,
							_ => Direction.EAST,
						};
					case ENCOUNTER_SPLITTER_HOR:
						return currentDirection switch
						{
							Direction.EAST => Direction.EAST,
							Direction.WEST => Direction.WEST,
							Direction.NORTH => Direction.EASTWEST,
							_ => Direction.EASTWEST,
						};
					case ENCOUNTER_SPLITTER_VER:
						return currentDirection switch
						{
							Direction.EAST => Direction.NORTHSOUTH,
							Direction.WEST => Direction.NORTHSOUTH,
							Direction.NORTH => Direction.NORTH,
							_ => Direction.SOUTH,
						};
					default: return currentDirection;
				}
			}
		}

		class Ray
		{
			internal bool Go { get; private set; } = true;
			internal Coordinate Location { get; set; }
			internal Direction Direction;

			internal Ray(Coordinate location, Direction direction)
			{
				this.Location = location;
				this.Direction = direction;
			}

			internal void Kill()
			{
				Go = false;
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

		internal readonly struct HighlightedFieldWrapper
		{
			internal readonly Coordinate Location;
			internal readonly Direction Direction;

			public HighlightedFieldWrapper(Coordinate coordinate, Direction direction)
			{
				this.Location = coordinate;
				this.Direction = direction;
			}

			public override int GetHashCode()
			{
				return (Location.GetHashCode() << 3) | (int) Direction;
			}

			public override bool Equals(object? obj)
			{
				if (obj == null) return false;
				if (obj.GetType() != typeof(HighlightedFieldWrapper)) return false;
				return GetHashCode() == obj.GetHashCode();
			}
		}

		internal class MapWrapper
		{
			private string[] Map;
			internal readonly int Width, Height;

			private HashSet<HighlightedFieldWrapper> _HighlightedFields;
			private HashSet<int> HighlightHashesOnly;

			public MapWrapper(string[] map)
			{
				_HighlightedFields = new();
				HighlightHashesOnly = new();
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

					if (IsOffMap(coord))
					{
						// Coordinate is out of range, return char that will definitely return false in the contains check
						return 'ö';
					}

					return Map[coord.Row][coord.Col];
				}
			}

			internal bool HighlightField(Coordinate coordinate, Direction direction)
				=> HighlightField(new HighlightedFieldWrapper(coordinate, direction));
			internal bool HighlightField(HighlightedFieldWrapper field)
			{
				if (_HighlightedFields.Contains(field)) return false;   // Program says it's redundant, but we know we need the return value

				_HighlightedFields.Add(field);
				HighlightHashesOnly.Add(field.Location.GetHashCode());
				return true;
			}

			internal int HighlightedFields() => HighlightHashesOnly.Count;

			internal void ResetHighlights()
			{
				HighlightHashesOnly.Clear();
				_HighlightedFields.Clear();
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

			public string ToStringHighlighted()
			{
				StringBuilder builder = new StringBuilder();
				for (int row = 0; row < Height; row++)
				{
					for (int col = 0; col < Width; col++)
					{
						if (HighlightHashesOnly.Contains(Coordinate.GetCoordHash(row, col)))
							builder.Append('#');
						else builder.Append('.');
					}

					builder.Append('\n');
				}
				return builder.ToString();
			}

			private bool FieldHighlighted(int CoordHash)
			{
				foreach (var wrapper in _HighlightedFields)
				{
					if ((wrapper.GetHashCode() >> 3) == CoordHash)
					{
						return true;
					}
				}

				return false;
			}
		}

		internal enum Direction
		{
			NORTH = 0, EAST = 1, SOUTH = 2, WEST = 3, EASTWEST = 4, NORTHSOUTH = 5
		}
	}
}

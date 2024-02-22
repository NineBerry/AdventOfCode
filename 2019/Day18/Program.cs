// #define Sample

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2019\Day18\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2019\Day18\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2019\Day18\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2019\Day18\Full.txt";
#endif

    Grid gridPart1 = new Grid(File.ReadAllLines(fileNamePart1));
    Console.WriteLine("Part 1: " + Part1(gridPart1));

    Grid gridPart2 = new Grid(File.ReadAllLines(fileNamePart2));
    Console.WriteLine("Part 2: " + Part2(gridPart2));

    Console.ReadLine();
}

long Part1(Grid grid)
{
    return grid.GetShortestPath();
}

long Part2(Grid grid)
{
    grid.BuildWalls();
    return grid.GetShortestPathWithRobots();
}


public class Grid
{
    public Grid(string[] input)
    {
        Width = input.First().Length;
        Height = input.Length;

        InitializeMaze(input);

        KeysCount = KeyPositions.Count;
    }

    private void InitializeMaze(string[] input)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Point point = new Point(x, y);
                char ch = input[y][x];

                if (ch == '@')
                {
                    Start = point;
                }
                else if (ch == '#')
                {
                    Walls.Add(point);   
                }
                else if (IsKey(ch))
                {
                    KeyPositions[point] = ch;
                }
                else if (IsDoor(ch))
                {
                    DoorPositions[point] = ch;
                }
            }
        }
    }

    public int Height;
    public int Width;
    public Point Start = new Point(0, 0);

    private HashSet<Point> Walls = [];
    private Dictionary<Point, char> KeyPositions = [];
    private Dictionary<Point, char> DoorPositions = [];
    private int KeysCount;

    public int GetShortestPath()
    {
        HashSet<(Point Point, string FoundKeys)> visited = [];
        PriorityQueue<(Point Point, int Steps, string FoundKeys), int> todos = new();
        todos.Enqueue((Start, 0, ""), 0);

        while (todos.TryDequeue(out var current, out _))
        {
            if (visited.Contains((current.Point, current.FoundKeys))) continue;
            visited.Add((current.Point, current.FoundKeys));

            if(KeyPositions.TryGetValue(current.Point, out var key))
            {
                if (!current.FoundKeys.Contains(key))
                {
                    current.FoundKeys = new string((current.FoundKeys + key).Order().ToArray());
                }
            }

            if (current.FoundKeys.Length == KeysCount)
            {
                return current.Steps;
            }

            foreach (var neighbor in current.Point.GetAllNeighboringPoints())
            {
                if (CanEnter(neighbor, current.FoundKeys))
                {
                    if (!visited.Contains((neighbor, current.FoundKeys)))
                    {
                        todos.Enqueue((neighbor, current.Steps + 1, current.FoundKeys), current.Steps + 1);
                    }
                }
            }
        }

        return int.MaxValue;
    }

    private bool CanEnter(Point point, string foundKeys)
    {
        if(Walls.Contains(point)) return false;

        if(DoorPositions.TryGetValue(point, out var door))
        {
            char key = char.ToLower(door);
            return foundKeys.Contains(key);
        }

        return true;
    }

    private static bool IsKey(char ch)
    {
        return ch is >= 'a' and <= 'z';
    }
    private static bool IsDoor(char ch)
    {
        return ch is >= 'A' and <= 'Z';
    }

    public void BuildWalls()
    {
        Walls.Add(Start);
        foreach (var neighbor in Start.GetAllNeighboringPoints()) 
        {
            Walls.Add(neighbor);
        }
    }

    public long GetShortestPathWithRobots()
    {
        int maxSteps = 0;
        int maxKeys = 0;

        Point robot1Start = Start with { X = Start.X - 1, Y = Start.Y - 1 };
        Point robot2Start = Start with { X = Start.X + 1, Y = Start.Y - 1 };
        Point robot3Start = Start with { X = Start.X - 1, Y = Start.Y + 1 };
        Point robot4Start = Start with { X = Start.X + 1, Y = Start.Y + 1 };

        HashSet<(Point Point, string FoundKeys)> visited = [];
        PriorityQueue<(int ActiveRobot, Point Robot1, Point Robot2, Point Robot3, Point Robot4, int Steps, string FoundKeys), int> todos = new();
        todos.Enqueue((1, robot1Start, robot2Start, robot3Start, robot4Start, 0, ""), 0);
        todos.Enqueue((2, robot1Start, robot2Start, robot3Start, robot4Start, 0, ""), 0);
        todos.Enqueue((3, robot1Start, robot2Start, robot3Start, robot4Start, 0, ""), 0);
        todos.Enqueue((4, robot1Start, robot2Start, robot3Start, robot4Start, 0, ""), 0);

        while (todos.TryDequeue(out var current, out _))
        {
            if (current.Steps > maxSteps + 49 || current.FoundKeys.Length > maxKeys)
            {
                maxSteps = current.Steps;
                maxKeys = current.FoundKeys.Length;
                // Console.Write("\r" + maxSteps + " " + maxKeys);
            }

            bool useRobot1 = true;
            bool useRobot2 = true;
            bool useRobot3 = true;
            bool useRobot4 = true;

            int keysBefore = current.FoundKeys.Length;

            CheckKeyFound(ref current, current.Robot1);
            CheckKeyFound(ref current, current.Robot2);
            CheckKeyFound(ref current, current.Robot3);
            CheckKeyFound(ref current, current.Robot4);

            bool allowSwitching = keysBefore != current.FoundKeys.Length;


            if (current.FoundKeys.Length == KeysCount)
            {
                return current.Steps;
            }


            if (visited.Contains((current.Robot1, current.FoundKeys)))
            {
                useRobot1 = false;
            }

            if (visited.Contains((current.Robot2, current.FoundKeys)))
            {
                useRobot2 = false;
            }

            if (visited.Contains((current.Robot3, current.FoundKeys)))
            {
                useRobot3 = false;
            }

            if (visited.Contains((current.Robot4, current.FoundKeys)))
            {
                useRobot4 = false;
            }

            visited.Add((current.Robot1, current.FoundKeys));
            visited.Add((current.Robot2, current.FoundKeys));
            visited.Add((current.Robot3, current.FoundKeys));
            visited.Add((current.Robot4, current.FoundKeys));


            if (useRobot1 && (current.ActiveRobot == 1 || allowSwitching))
            {
                foreach (var neighbor in current.Robot1.GetAllNeighboringPoints())
                {
                    if (CanEnter(neighbor, current.FoundKeys))
                    {
                        if (!visited.Contains((neighbor, current.FoundKeys)))
                        {
                            todos.Enqueue((1, neighbor, current.Robot2, current.Robot3, current.Robot4, current.Steps + 1, current.FoundKeys), current.Steps + 1);
                        }
                    }
                }
            }


            if (useRobot2 && (current.ActiveRobot == 2 || allowSwitching))
            {
                foreach (var neighbor in current.Robot2.GetAllNeighboringPoints())
                {
                    if (CanEnter(neighbor, current.FoundKeys))
                    {
                        if (!visited.Contains((neighbor, current.FoundKeys)))
                        {
                            todos.Enqueue((2, current.Robot1, neighbor, current.Robot3, current.Robot4, current.Steps + 1, current.FoundKeys), current.Steps + 1);
                        }
                    }
                }
            }

            if (useRobot3 && (current.ActiveRobot == 3 || allowSwitching))
            {
                foreach (var neighbor in current.Robot3.GetAllNeighboringPoints())
                {
                    if (CanEnter(neighbor, current.FoundKeys))
                    {
                        if (!visited.Contains((neighbor, current.FoundKeys)))
                        {
                            todos.Enqueue((3, current.Robot1, current.Robot2, neighbor, current.Robot4, current.Steps + 1, current.FoundKeys), current.Steps + 1);
                        }
                    }
                }
            }

            if (useRobot4 && (current.ActiveRobot == 4 || allowSwitching))
            {
                foreach (var neighbor in current.Robot4.GetAllNeighboringPoints())
                {
                    if (CanEnter(neighbor, current.FoundKeys))
                    {
                        if (!visited.Contains((neighbor, current.FoundKeys)))
                        {
                            todos.Enqueue((4, current.Robot1, current.Robot2, current.Robot3, neighbor, current.Steps + 1, current.FoundKeys), current.Steps + 1);
                        }
                    }
                }
            }
        }

        return int.MaxValue;
    }

    private void CheckKeyFound(ref (int ActiveRobot, Point Robot1, Point Robot2, Point Robot3, Point Robot4, int Steps, string FoundKeys) current, Point checkKeyPoint)
    {
        if (KeyPositions.TryGetValue(checkKeyPoint, out var key))
        {
            if (!current.FoundKeys.Contains(key))
            {
                current.FoundKeys = new string((current.FoundKeys + key).Order().ToArray());
            }
        }
    }
}

public record Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.West => this with { X = this.X - 1 },
            Direction.East => this with { X = this.X + 1 },

            Direction.South => this with { X = this.X, Y = this.Y + 1 },
            Direction.North => this with { X = this.X, Y = this.Y - 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public Point[] GetAllNeighboringPoints()
    {
        return AllDirections.Select(GetNeightboringPoint).ToArray();
    }

    public static Direction[] AllDirections = [Direction.West, Direction.East, Direction.North, Direction.South];
}

public enum Direction
{
    West,
    East,
    North,
    South,
}



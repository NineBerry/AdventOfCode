// #define Sample

using System.Text;
using Python.Runtime;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day21\Sample.txt";
    int part1Steps = 6;
    int part2Steps = 5000;
    int startCycle = 3;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day21\Full.txt";
    int part1Steps = 64;
    int part2Steps = 26501365;
    int startCycle = 0;
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input, part1Steps));
    Console.WriteLine("Part 2: " + Part2(input, part2Steps, startCycle));
    Console.ReadLine();
}

long Part1(string[] input, int steps)
{
    Grid grid1 = new Grid(input, infiniteMap: false);

    return grid1.CountPossiblePlacesAfterSteps(grid1.StartPoint, steps);
}

/// Someone else noticed that there is a pattern:
/// When the visited places outline forms a square diamond,
/// the number of visited places can be calculated 
/// using a quadratic formula. 
/// 
/// To calculate the formula, we need three points. 
/// Then we can calculate the value for the given steps
/// using the formula.
/// 
/// The visited places seem to form a diamond every [Width] 
/// steps starting with the target step number modulo [Width].
/// 
/// In the sample this is not true for lower numbers, 
/// only for higher numbers. But with the full input we 
/// can actually start at the modulo directly.
/// 

long Part2(string[] input, int steps, int startCycle)
{
    Grid grid2 = new Grid(input, infiniteMap: true);

    var decisivePoints = grid2.GetDecisivePointsForFormula(steps, startCycle);
    return PythonHelper.SolveQuadraticFormulaAndCalculateFor(decisivePoints, steps);
}

class Grid
{
    public readonly int Width;
    public readonly int Height;
    private readonly bool InfiniteMap;

    public Point StartPoint { get; set; }

    public HashSet<Point> rocks = new();

    public Grid(string[] text, bool infiniteMap)
    {
        Width = text[0].Length;
        Height = text.Length;
        InfiniteMap = infiniteMap;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                switch (text[y][x])
                {
                    case 'S': 
                        StartPoint = new Point(x, y); 
                        break;
                    case '#':
                        rocks.Add(new Point(x, y)); 
                        break;
                }
            }
        }
    }

    private Dictionary<Point, Dictionary<int, HashSet<Point>>> visited = new();

    public long CountPossiblePlacesAfterSteps(Point startPoint, int maxSteps)
    {
        return PossiblePlacesAfterSteps(startPoint, maxSteps).Count(p => IsInGrid(p));
    }


    public HashSet<Point> PossiblePlacesAfterSteps(Point startPoint, int maxSteps) 
    {
        if (IsRock(startPoint)) return [];

        if (!IsInGrid(startPoint))
        {
            AddVisited(startPoint, startPoint, 0);
            return [startPoint];
        }

        if (maxSteps == 0)
        {
            AddVisited(startPoint, startPoint, 0);
            return [startPoint];
        }

        var cached = GetVisited(startPoint, maxSteps);
        if(cached.Any())
            return cached;

        var previous = PossiblePlacesAfterSteps(startPoint, maxSteps - 1);

        foreach (var previousPoint in previous)
        {
            if (!IsInGrid(previousPoint)) continue;

            foreach (var direction in AllDirections())
            {
                Point nextPoint = previousPoint.GetNeightboringPoint(direction);

                if (!IsRock(nextPoint))
                {
                    AddVisited(startPoint, nextPoint, maxSteps);
                }
            }
        }

        return GetVisited(startPoint, maxSteps);
    }

    private bool IsRock(Point point)
    {
        Point pointInTemplate = new Point(TranslateCoordinate(point.X), TranslateCoordinate(point.Y));

        return rocks.Contains(pointInTemplate);
    }

    private int TranslateCoordinate(int from)
    {
        return ActualMod(from, Width);
    }

    public static int ActualMod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }

    private void AddVisited(Point startPoint, Point point, int steps)
    {
        if(!visited.TryGetValue(startPoint, out var startPointDictionary))
        {
            startPointDictionary = new();
            visited[startPoint] = startPointDictionary;
        }

        if(!startPointDictionary.TryGetValue(steps, out var points))
        {
            points = new();
            startPointDictionary[steps] = points;
        }

        points.Add(point);
    }

    private HashSet<Point> GetVisited(Point startPoint, int steps)
    {
        if(visited.TryGetValue(startPoint, out var startPointDictionary))
        {
            if(startPointDictionary.TryGetValue(steps, out var points))
            {
                return points;
            }
        }

        return [];
    }

    public bool IsInGrid(Point point)
    {
        return 
            InfiniteMap ||
            (point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height);
    }
    private static Direction[] AllDirections() => [Direction.East, Direction.North, Direction.South, Direction.West];


    public Dictionary<int, long> GetDecisivePointsForFormula(int part2Steps, int startCycle)
    {
        int cycle = Width;
        int rest = part2Steps % cycle;

        Dictionary<int, long> calculatedSteps = new();

        foreach (int i in Enumerable.Range(startCycle, 3))
        {
            int steps = rest + (i * cycle);
            long placesReached = CountPossiblePlacesAfterSteps(StartPoint, steps);

            Console.WriteLine($"Places reached after {steps} steps are {placesReached}");
            calculatedSteps[steps] = placesReached;
        }

        return calculatedSteps;
    }
}

record struct Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y + 1 },
            Direction.West => this with { X = this.X - 1 },
            Direction.North => this with { Y = this.Y - 1 },
            Direction.East => this with { X = this.X + 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}

enum Direction
{
    None,
    South,
    West,
    North,
    East
}

static class Extensions
{
    public static Direction TurnRight(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
    public static Direction TurnLeft(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.West,
            Direction.West => Direction.South,
            Direction.South => Direction.East,
            Direction.East => Direction.North,
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}

static class PythonHelper
{
    public static long SolveQuadraticFormulaAndCalculateFor(Dictionary<int, long> calculatedSteps, int part2Steps)
    {
        InitializePythonEngine();
        string code = CreatePythonScript(calculatedSteps, part2Steps);
        long result = RunPythonScript(code);

        return result;
    }

    private static void InitializePythonEngine()
    {
        Runtime.PythonDLL = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python39_64\python39.dll";
        PythonEngine.Initialize();
        PythonEngine.BeginAllowThreads();
    }

    private static string CreatePythonScript(Dictionary<int, long> calculatedSteps, int part2Steps)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("from sympy import symbols, Eq, solve");
        sb.AppendLine();
        sb.AppendLine("a, b, c, x = symbols('a b c x')");
        sb.AppendLine();

        string points = string.Join(",", calculatedSteps.Select(pair => $"({pair.Key},{pair.Value})"));
        sb.AppendLine($"points = [{points}]");
        sb.AppendLine();
        sb.AppendLine("equations = [Eq(a * x**2 + b * x + c, y) for (x, y) in points]");
        sb.AppendLine("solution = solve(equations, (a, b, c))");
        sb.AppendLine();
        sb.AppendLine("va = solution[a]");
        sb.AppendLine("vb = solution[b]");
        sb.AppendLine("vc = solution[c]");
        sb.AppendLine();
        sb.AppendLine($"x = {part2Steps}");
        sb.AppendLine("result = (va * x * x) + vb * x + vc");
        return sb.ToString();
    }

    private static long RunPythonScript(string code)
    {
        long result = 0;

        using (Py.GIL())
        {
            using (PyModule scope = Py.CreateScope())
            {
                scope.Exec(code);
                PyObject obj = scope.Get("result");
                result = long.Parse("" + obj);
            }
        }

        return result;
    }
}
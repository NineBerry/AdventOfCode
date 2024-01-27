// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day17\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day17\Full.txt";
#endif

    string password = File.ReadAllText(fileName);

    Grid grid = new Grid(password);

    Console.WriteLine("Part 1: " + Part1(grid));
    Console.WriteLine("Part 2: " + Part2(grid));
    Console.ReadLine();
}

string Part1(Grid grid)
{
    return grid.GetShortestPath();
}
int Part2(Grid grid)
{
    return grid.GetLongestPath().Length;
}

class Grid
{
    private string Password;

    public readonly int Height;
    public readonly int Width;

    public Point StartPoint;
    public Point EndPoint;

    public Grid(string password)
    {
        Password = password;

        Width = 4;
        Height = 4;

        StartPoint = new Point(0, 0);
        EndPoint = new Point(3, 3);
    }

    public string GetShortestPath()
    {
        Queue<(Point Point, string Path)> todos = [];

        todos.Enqueue((StartPoint, ""));
        
        while(todos.TryDequeue(out var current))
        {
            if(current.Point == EndPoint) return current.Path;

            if (!IsInGrid(current.Point)) continue;

            foreach(var direction in GetPossibleNextDirections(current.Path))
            {
                todos.Enqueue((current.Point.GetNeightboringPoint(direction), current.Path + (char)direction));
            }
        }

        return "";
    }

    public string GetLongestPath()
    {
        List<string> paths = [];
        
        Queue<(Point Point, string Path)> todos = [];

        todos.Enqueue((StartPoint, ""));

        while (todos.TryDequeue(out var current))
        {
            if (current.Point == EndPoint)
            {
                paths.Add(current.Path);
                continue;
            }

            if (!IsInGrid(current.Point)) continue;

            foreach (var direction in GetPossibleNextDirections(current.Path))
            {
                todos.Enqueue((current.Point.GetNeightboringPoint(direction), current.Path + (char)direction));
            }
        }

        return paths.OrderByDescending(s => s.Length).First();
    }


    private Direction[] GetPossibleNextDirections(string currentPath)
    {
        List<Direction> result = [];
        
        string input = Password + currentPath;
        string hash = CreateMD5(input);

        string openCharacters = "bcdef";

        if (openCharacters.Contains(hash[0])) result.Add(Direction.North);
        if (openCharacters.Contains(hash[1])) result.Add(Direction.South);
        if (openCharacters.Contains(hash[2])) result.Add(Direction.West);
        if (openCharacters.Contains(hash[3])) result.Add(Direction.East);

        return result.ToArray();
    }

    private static string CreateMD5(string input)
    {
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            input = Convert.ToHexString(hashBytes).ToLowerInvariant();
            return input.ToLowerInvariant();
        }
    }

    public bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
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
    South = 'D',
    West = 'L',
    North = 'U',
    East = 'R'
}
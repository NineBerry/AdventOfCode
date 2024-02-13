// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day15\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day15\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Grid grid = new Grid(input, expanded: false);
    Grid gridExpanded = new Grid(input, expanded: true);
    
    Console.WriteLine("Part 1: " + grid.GetShortestPathRisk());
    Console.WriteLine("Part 2: " + gridExpanded.GetShortestPathRisk());
    Console.ReadLine();
}


public class Grid
{
    public Grid(string[] input, bool expanded)
    {
        Width = TemplateWidth = input.First().Length;
        Height = TemplateHeight = input.Length;

        InitializeRisks(input);

        ExpandedMap = expanded;

        if (expanded) 
        {
            Height = Height * 5;
            Width = Width * 5;
        }

        StartPoint = new Point(0, 0);
        EndPoint = new Point(Width - 1, Height - 1);
    }

    private void InitializeRisks(string[] input)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Point point = new Point(x, y);
                char ch = input[y][x];
                Risks[point] = (int)char.GetNumericValue(ch);
            }
        }
    }

    public int TemplateHeight;
    public int TemplateWidth;
    public int Height;
    public int Width;
    public Point StartPoint;
    public Point EndPoint;
    public bool ExpandedMap;

    private Dictionary<Point, int> Risks = [];
    private int GetRisk(Point point)
    {
        int templateX = point.X % TemplateWidth;
        int templateY = point.Y % TemplateHeight;
        Point temlatePoint = new Point(templateX, templateY);
        int templateRisk = Risks[temlatePoint];

        int distanceX = point.X / TemplateWidth;
        int distanceY = point.Y / TemplateHeight;
        int manhattanDistance = distanceX + distanceY;

        int risk = templateRisk + manhattanDistance;
        risk %= 9;
        if(risk == 0) risk = 9;

        return risk;    
    }

    private bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }

    public int GetShortestPathRisk()
    {
        HashSet<Point> visited = [];
        PriorityQueue<Point, int> queue = new();
        queue.Enqueue(StartPoint, 0);

        while(queue.TryDequeue(out var point, out int risk))
        {
            if (visited.Contains(point)) continue;
            visited.Add(point);

            if (point == EndPoint) return risk;

            foreach (var neighbor in point.GetAllNeighboringPoints()) 
            { 
                if(!IsInGrid(neighbor)) continue;
                if (visited.Contains(neighbor)) continue;

                int neighborRisk = GetRisk(neighbor);
                queue.Enqueue(neighbor, risk + neighborRisk);
            }
        }

        return 0;
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
        return AllDirections.Select(d => GetNeightboringPoint(d)).ToArray();
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

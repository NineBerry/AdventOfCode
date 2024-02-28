using System.Collections.Concurrent;
using System.Text;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day15\Full.txt";
    string visualizationFileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day15\Visualization.txt";
    
    long[] program = ReadProgram(fileName);
    Grid grid = new Grid(program);
    grid.MapArea();
    grid.SaveMapToFile(visualizationFileName);

    Console.WriteLine("Part 1: " + grid.GetShortestPath());
    Console.WriteLine("Part 2: " + grid.FillOxygene());
    Console.ReadLine();
}

static long[] ReadProgram(string fileNamePart2)
{
    return File.ReadAllText(fileNamePart2).Split(',').Select(long.Parse).ToArray();
}


public class Grid
{
    private long[] Program;
    private Computer Computer;
    private BlockingCollection<long> Reader = [];
    private BlockingCollection<long> Writer = [];
    
    private Dictionary<Point, Tile> Tiles = [];
    
    private Point Start = new Point();

    public Grid(long[] program)
    {
        Program = program;  
        Computer = new Computer(Program);
        Computer.Input += (c) => Writer.Take();
        Computer.Output += (c, value) => Reader.Add(value);
        Task.Factory.StartNew(() => Computer.Run(), TaskCreationOptions.LongRunning);
    }

    public void MapArea()
    {
        Tiles[Start] = Tile.Start;

        Queue<(Point Point, Direction Direction)> queue = [];
        
        foreach(var direction in AllDirections)
        {
            ExploreRecursive(Start, direction);            
        }

        void ExploreRecursive(Point Point, Direction direction)
        {
            Point targetPoint = Point.GetNeightboringPoint(direction);
            if (Tiles.ContainsKey(targetPoint)) return;

            Tile targetTile = TellRobot(direction);
            Tiles[targetPoint] = targetTile;

            if(targetTile != Tile.Wall)
            {
                foreach (var nextdirection in AllDirections)
                {
                    ExploreRecursive(targetPoint, nextdirection);
                }

                TellRobot(direction.Opposite());
            }
        }
    }

    public int GetShortestPath()
    {
        HashSet<Point> visited = [];
        PriorityQueue<Point, int> queue = new();
        queue.Enqueue(Start, 0);

        while(queue.TryDequeue(out var point, out var steps))
        {
            if (visited.Contains(point)) continue;
            visited.Add(point);

            Tile tile = Tiles[point];
            if (tile == Tile.Wall) continue;
            if (tile == Tile.Oxygen) return steps;

            foreach (var direction in AllDirections)
            {
                queue.Enqueue(point.GetNeightboringPoint(direction), steps + 1);
            }
        }

        return 0;
    }

    private Tile TellRobot(Direction direction)
    {
        Writer.Add((long)direction);
        return (Tile)Reader.Take();
    }

    public void SaveMapToFile(string visualizationFileName)
    {
        StringBuilder sb = new StringBuilder();

        long xMin = Tiles.Keys.Min(p => p.X);
        long xMax = Tiles.Keys.Max(p => p.X);
        long yMin = Tiles.Keys.Min(p => p.Y);
        long yMax = Tiles.Keys.Max(p => p.Y);

        sb.AppendLine();
        for (long y = yMin; y <= yMax; y++)
        {
            for (long x = xMin; x <= xMax; x++)
            {
                Tiles.TryGetValue(new Point(x, y), out var tile);

                char ch = tile switch
                {
                    Tile.Wall => '#',
                    Tile.Ground => '.',
                    Tile.Oxygen => 'O',
                    Tile.Start => 'S',
                    _ => throw new NotImplementedException()
                };
                sb.Append(ch);
            }
            sb.AppendLine();
        }

        File.WriteAllText(visualizationFileName,  sb.ToString());
    }

    public int FillOxygene()
    {
        int steps = 0;

        while (Tiles.Any(p => p.Value is Tile.Ground or Tile.Start)) 
        {
            steps++;
            var oxygeneTiles = Tiles.Where(p => p.Value == Tile.Oxygen).Select(p => p.Key).ToArray();

            foreach(var oxygene in oxygeneTiles)
            {
                foreach(var direction in AllDirections)
                {
                    var neighbor = oxygene.GetNeightboringPoint(direction);
                    if (Tiles[neighbor] is Tile.Ground or Tile.Start)
                    {
                        Tiles[neighbor] = Tile.Oxygen;
                    }
                }
            }
        }

        return steps;
    }

    private Direction[] AllDirections = [Direction.North, Direction.East, Direction.South, Direction.West];
}


public record struct Point(long X, long Y)
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


public enum Tile
{
    Ground = 1,
    Wall = 0,
    Oxygen = 2,
    Start = 3,
}

public enum Direction
{
    South = 2,
    West = 3,
    North = 1,
    East = 4
}


public static class Extensions
{
    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.South => Direction.North,
            Direction.West => Direction.East,
            Direction.North => Direction.South,
            Direction.East => Direction.West,
            _ => throw new ArgumentException("Direction", nameof(direction))
        };
    }
}
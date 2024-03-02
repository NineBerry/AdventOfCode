using System.Collections.Concurrent;
using System.Text;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day19\Full.txt";
    string visualizationFileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day19\Visualization.txt";

    long[] program = ReadProgram(fileName);
    Grid grid = new Grid(program, visualizationFileName);

    Point spaceship = grid.FindPlaceForShaceship(100);
    
    Console.WriteLine("Part 1: " + grid.GetPulledPoints(50));
    Console.WriteLine("Part 2: " + (spaceship.X * 10_000 + spaceship.Y));

    Console.ReadLine();
}

static long[] ReadProgram(string fileNamePart2)
{
    return File.ReadAllText(fileNamePart2).Split(',').Select(long.Parse).ToArray();
}

public class Grid
{
    private long[] Program;
    private string VisualizationFileName;

    public Grid(long[] program, string visualizationFileName)
    {
        Program = program;
        VisualizationFileName = visualizationFileName;
    }

    HashSet<Point> Pulled = [];
    HashSet<Point> KnownNonPulled = [];
    HashSet<Point> SpaceShip = [];

    public long GetPulledPoints(int extend)
    {
        return Pulled.Count(p => p.X < extend && p.Y < extend);
    }

    public Point FindPlaceForShaceship(int spaceshipSize)
    {
        const int initRange = 10;
        // Build first 10 as a start point
        for(int x=0; x< initRange; x++)
        {
            for (int y = 0; y < initRange; y++)
            {
                Point point = new Point(x, y);
                IsPointPulled(point);
            }
        }

        // Now continue with upper and lower borders
        long maxX = Pulled.Max(p => p.X);
        Point upperPoint = Pulled.Where(p => p.X == maxX).OrderBy(p => p.Y).First();  
        Point lowerPoint = Pulled.Where(p => p.X == maxX).OrderBy(p => p.Y).Last();

        Point spaceshipPoint = new();

        while (spaceshipPoint == Point.Zero)
        {
            if (IsPointPulled(upperPoint))
            {
                upperPoint = GetLastPulled(upperPoint, Direction.North);
            }
            else
            {
                upperPoint = GetFirstPulled(upperPoint, Direction.South);
            }

            if (IsPointPulled(lowerPoint))
            {
                lowerPoint = GetLastPulled(lowerPoint, Direction.South);
            }
            else
            {
                lowerPoint = GetFirstPulled(lowerPoint, Direction.North);
            }

            // Fill in points in between
            for (long y = upperPoint.Y; y <= lowerPoint.Y; y++)
            {
                Point p = new Point(upperPoint.X, y);
                Pulled.Add(p);
            }

            // Check space for spaceship
            for (long y = upperPoint.Y; y <= lowerPoint.Y; y++)
            {
                Point topright = new Point(upperPoint.X, y);
                Point topleft = topright with { X = topright.X - (spaceshipSize - 1) };
                Point bottomleft = topleft with { Y = topleft.Y + (spaceshipSize - 1) };
                Point bottomright = topright with { Y = topright.Y + (spaceshipSize - 1) };

                if (
                    Pulled.Contains(topleft) &&
                    Pulled.Contains(bottomleft) &&
                    Pulled.Contains(bottomright))
                {
                    spaceshipPoint = topleft;
                    break;
                }
            }

            upperPoint = upperPoint with { X = upperPoint.X + 1 };
            lowerPoint = lowerPoint with { X = lowerPoint.X + 1, Y = lowerPoint.Y + 1};
        }

        for(long x=spaceshipPoint.X; x < spaceshipPoint.X + spaceshipSize; x++)
        {
            for (long y = spaceshipPoint.Y; y < spaceshipPoint.Y + spaceshipSize; y++)
            {
                SpaceShip.Add(new Point(x, y));
            }
        }

        SaveMapToFile(VisualizationFileName);

        return spaceshipPoint;
    }

    private Point GetLastPulled(Point point, Direction direction)
    {
        Point next = point;
        while (IsPointPulled(next))
        {
            point = next;
            next = point.GetNeightboringPoint(direction);
        }

        return point;
    }

    private Point GetFirstPulled(Point point, Direction direction)
    {
        Point next = point;
        while (!IsPointPulled(next))
        {
            point = next;
            next = point.GetNeightboringPoint(direction);
        }

        return next;
    }

    private bool IsPointPulled(Point point)
    {
        if (point.X < 0) return false;
        if (point.Y < 0) return false;

        if (Pulled.Contains(point)) return true;
        if (KnownNonPulled.Contains(point)) return false;

        bool result = IsPointPulledAskComputer(point);

        if (result) Pulled.Add(point);
        if (!result) KnownNonPulled.Add(point);

        return result;
    }

    private bool IsPointPulledAskComputer(Point point)
    {
        BlockingCollection<long> reader = [];
        BlockingCollection<long> writer = [];
        writer.Add(point.X);
        writer.Add(point.Y);

        Computer computer = new Computer(Program);
        computer.Input += (c) => writer.Take();
        computer.Output += (c, value) => reader.Add(value);
        computer.Run();

        return reader.Take() == 1;
    }

    private void SaveMapToFile(string visualizationFileName)
    {
        StringBuilder sb = new StringBuilder();

        long xMin = Pulled.Min(p => p.X);
        long xMax = Pulled.Max(p => p.X);
        long yMin = Pulled.Min(p => p.Y);
        long yMax = Pulled.Max(p => p.Y);

        for (long y = yMin; y <= yMax; y++)
        {
            for (long x = xMin; x <= xMax; x++)
            {
                Point point = new Point(x, y);

                if (SpaceShip.Contains(point))
                {
                    sb.Append('O');
                }
                else if(Pulled.Contains(point))
                {
                    sb.Append('#');
                }
                else
                {
                    sb.Append('.');
                }
            }
            sb.AppendLine();
        }

        File.WriteAllText(visualizationFileName, sb.ToString());
    }
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

    public static Point Zero = new Point(0, 0);
}

public enum Direction
{
    South,
    West,
    North,
    East
}



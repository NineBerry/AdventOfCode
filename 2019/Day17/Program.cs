using System.Collections.Concurrent;
using System.Text;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day17\Full.txt";
    string visualizationFileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day17\Visualization.txt";

    long[] program = ReadProgram(fileName);
    Grid grid = new Grid(program, visualizationFileName);
    Console.WriteLine("Part 1: " + grid.GetAlignmentSum());
    Console.WriteLine("Part 2: " + grid.SweepRun());
    
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

    private Dictionary<Point, Tile> Tiles = [];
    private HashSet<Point> Paths = [];

    public Grid(long[] program, string visualizationFileName)
    {
        VisualizationFileName = visualizationFileName;
        Program = program;

        MapArea();
        SaveMapToFile(Tiles, VisualizationFileName);
    }

    public void MapArea()
    {
        (var reader, var writer) = RunProgram(Program);
        Tiles = ReadMapFromRobot(reader, out _);

        Paths = Tiles.Where(p => p.Value == Tile.Path).Select(p => p.Key).ToHashSet();
    }

    private static Dictionary<Point, Tile> ReadMapFromRobot(BlockingCollection<long> reader, out long suffix)
    {
        suffix = 0;
        
        Dictionary<Point, Tile> tiles = [];
        Point point = new Point();
        foreach (var item in reader.GetConsumingEnumerable())
        {
            if(item > 255)
            {
                suffix = item;
                continue;
            }
            
            char ch = (char)item;

            if (ch == '\n')
            {
                point = new Point(0, point.Y + 1);
            }
            else
            {
                tiles.Add(point, (Tile)ch);
                point = new Point(point.X + 1, point.Y);
            }
        }

        return tiles;
    }

    public long SweepRun()
    {
        string[] fullPath = GetFullPath();
        // Console.WriteLine("Full Path: " + string.Join(",", fullPath));
        var code = CreateRobotCode(fullPath);

        long[] modifiedProgram = [2, ..Program.Skip(1)];
        (var reader, var writer) = RunProgram(modifiedProgram);

        SendLineToRobot(writer, code.Main);
        SendLineToRobot(writer, code.FunctionA);
        SendLineToRobot(writer, code.FunctionB);
        SendLineToRobot(writer, code.FunctionC);
        SendLineToRobot(writer, "n");

        var map = ReadMapFromRobot(reader, out var suffix);
        SaveMapToFile(map, VisualizationFileName);
        return suffix;
    }

    private string[] GetFullPath()
    {
        List<string> fullPath = new List<string>();

        var roboter = Tiles.Where(p => p.Value is not Tile.Ground and not Tile.Path).Single();
        var roboterPoint = roboter.Key;
        var roboterDirection = (Direction)roboter.Value;
        int distance = 0;

        while (true)
        {
            Point next = roboterPoint.GetNeightboringPoint(roboterDirection);
            if (Paths.Contains(next))
            {
                distance++;
                roboterPoint = next;
            }
            else
            {
                if (distance != 0) fullPath.Add(distance.ToString());
                distance = 0;

                var leftTurnDirection = roboterDirection.TurnLeft();
                var leftTurnNext = roboterPoint.GetNeightboringPoint(leftTurnDirection);
                if (Paths.Contains(leftTurnNext))
                {
                    fullPath.Add("L");
                    roboterDirection = leftTurnDirection;
                }
                else
                {
                    var rightTurnDirection = roboterDirection.TurnRight();
                    var rightTurnNext = roboterPoint.GetNeightboringPoint(rightTurnDirection);
                    if (Paths.Contains(rightTurnNext))
                    {
                        fullPath.Add("R");
                        roboterDirection = rightTurnDirection;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return fullPath.ToArray();
    }

    private (string Main, string FunctionA, string FunctionB, string FunctionC) CreateRobotCode(string[] fullPath)
    {
        /// Manual : 
        /// L,6,R,12,R,8,R,8,R,12,L,12,R,8,R,12,L,12,L,6,R,12,R,8,R,12,L,12,L,4,L,4,L,6,R,12,R,8,R,12,L,12,L,4,L,4,L,6,R,12,R,8,R,12,L,12,L,4,L,4,R,8,R,12,L,12
        /// 
        /// A,B,B,A,C,A,C,A,C,B
        /// 
        /// A: L,6,R,12,R,8
        /// B: R,8,R,12,L,12
        /// C: R,12,L,12,L,4,L,4

        // TODO: Automatic derivation
        
        return ("A,B,B,A,C,A,C,A,C,B", "L,6,R,12,R,8", "R,8,R,12,L,12", "R,12,L,12,L,4,L,4");
    }

    private void SaveMapToFile(Dictionary<Point, Tile> map, string visualizationFileName)
    {
        StringBuilder sb = new StringBuilder();

        long xMin = map.Keys.Min(p => p.X);
        long xMax = map.Keys.Max(p => p.X);
        long yMin = map.Keys.Min(p => p.Y);
        long yMax = map.Keys.Max(p => p.Y);

        for (long y = yMin; y <= yMax; y++)
        {
            for (long x = xMin; x <= xMax; x++)
            {
                map.TryGetValue(new Point(x, y), out var tile);

                if(tile != 0) sb.Append((char)tile);
            }
            sb.AppendLine();
        }

        File.WriteAllText(visualizationFileName, sb.ToString());
    }

    public long GetAlignmentSum()
    {
        return 
            Paths
            .Where(p => p.GetAllNeightboringPoints().All(pp => Paths.Contains(pp)))
            .Select(p => p.Alignment())
            .Sum();
    }

    private static
        (BlockingCollection<long> Reader,
        BlockingCollection<long> Writer)
        RunProgram(long[] program)
    {
        Computer computer;
        BlockingCollection<long> reader = [];
        BlockingCollection<long> writer = [];
        computer = new Computer(program);
        computer.Input += (c) => writer.Take();
        computer.Output += (c, value) => reader.Add(value);
        Task.Factory.StartNew(() =>
        {
            computer.Run();
            reader.CompleteAdding();
        }, TaskCreationOptions.LongRunning);

        return (reader, writer);
    }

    private void SendLineToRobot(BlockingCollection<long> writer, string line)
    {
        foreach(var ch in line)
        {
            writer.Add(ch);
        }
        writer.Add(10);
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
    public Point[] GetAllNeightboringPoints()
    {
        var me = this;
        return AllDirections.Select(me.GetNeightboringPoint).ToArray();
    }

    public long Alignment() => X * Y;
    
    private static Direction[] AllDirections = [Direction.North, Direction.East, Direction.South, Direction.West];
}


public enum Tile
{
    Ground = '.',
    Path = '#',
    South = Direction.South,
    North = Direction.North,
    West = Direction.West,  
    East = Direction.East,
}

public enum Direction
{
    South = 'v',
    West = '<',
    North = '^',
    East = '>'
}


public static class Extensions
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
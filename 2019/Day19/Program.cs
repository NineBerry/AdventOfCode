using System.Collections.Concurrent;
using System.Text;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day19\Full.txt";
    string visualizationFileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day19\Visualization.txt";

    long[] program = ReadProgram(fileName);
    Grid grid = new Grid(program, visualizationFileName);
    Console.WriteLine("Part 1: " + grid.GetPulledPoints(50));
    Console.WriteLine("Part 2: " + grid.GetPulledPoints(50));

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


    public long GetPulledPoints(int extend)
    {
        for(int x=0; x<extend; x++)
        {
            for (int y = 0; y < extend; y++)
            {
                Point point = new Point(x, y);
                IsPointPulled(point);
            }
        }

        /*Point upperPoint = new Point(0,0);
        Point lowerPoint = new Point(0, 0);

        while(upperPoint.X < extend)
        {
            // TODO


            upperPoint = upperPoint with { X = upperPoint.X + 1 };
            lowerPoint = lowerPoint with { X = lowerPoint.X + 1 };
        }*/

        SaveMapToFile(Pulled, VisualizationFileName);
        return Pulled.Count;
    }


    private bool IsPointPulled(Point point)
    {
        if (KnownNonPulled.Contains(point)) return false;
        if (Pulled.Contains(point)) return true;

        (var reader, var writer) = RunProgram(Program);
        
        writer.Add(point.X);
        writer.Add(point.Y);

        bool result = reader.Take() == 1;

        if (result) Pulled.Add(point);
        if (!result) KnownNonPulled.Add(point);

        return result;
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

    private void SaveMapToFile(HashSet<Point> pulled, string visualizationFileName)
    {
        StringBuilder sb = new StringBuilder();

        long xMin = pulled.Min(p => p.X);
        long xMax = pulled.Max(p => p.X);
        long yMin = pulled.Min(p => p.Y);
        long yMax = pulled.Max(p => p.Y);

        for (long y = yMin; y <= yMax; y++)
        {
            for (long x = xMin; x <= xMax; x++)
            {
                if(pulled.Contains(new Point(x, y)))
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
}

public enum Direction
{
    South,
    West,
    North,
    East
}



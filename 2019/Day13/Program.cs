using System.Text;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day13\Full.txt";
    string visualizationFileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day13\Visualization.txt";
    string visualizationFileName2 = @"D:\Dropbox\Work\AdventOfCode\2019\Day13\Visualization2.txt";
    long[] program = ReadProgram(fileName);

    Console.WriteLine("Part 1: " + Part1(program, visualizationFileName));
    Console.WriteLine("Part 2: " + Part2(program, visualizationFileName2));

    Console.ReadLine();
}


long Part1(long[] program, string visualizationFileName)
{
    (var blocksLeft, _) = Play(program, out var canvas);
    SavePainting(visualizationFileName, canvas);
    return blocksLeft;
}

long Part2(long[] program, string visualizationFileName)
{
    program = [2, .. program.Skip(1)];

    (var blocksLeft, var playerScore) = Play(program, out var canvas);
    SavePainting(visualizationFileName, canvas);

    if(blocksLeft == 0)
    {
        return playerScore;
    }

    Console.WriteLine("Lost! Still " + blocksLeft + " blocks left");
    return 0;
}

// 28512 too high
// 17280 too low
// 21057 too low

static long[] ReadProgram(string fileNamePart2)
{
    return File.ReadAllText(fileNamePart2).Split(',').Select(long.Parse).ToArray();
}

(long BlocksLeft, long PlayerScore) Play(long[] program, out Dictionary<Point, Tile> outCanvas)
{
    Dictionary<Point, Tile> canvas = [];

    Computer computer = new Computer(program);

    int awaitingInput = 0;
    long x = 0;
    long y = 0;
    long playerScore = 0;


    computer.Input += (c) => 0;
    
    computer.Output += (c, value) =>
    {
        if (awaitingInput == 0)
        {
            x = value;
            awaitingInput = 1;
        }
        else if (awaitingInput == 1)
        {
            y = value;
            awaitingInput = 2;
        }
        else if (awaitingInput == 2)
        {
            if (x == -1 && y == 0)
            {
                playerScore = value;
            }
            else
            {
                Point point = new Point(x, y);
                canvas[point] = (Tile)value;
            }
            awaitingInput = 0;
        }
    };

    computer.Run();

    outCanvas = canvas;

    return (canvas.Count(p => p.Value == Tile.Block), playerScore);
}


string ShowPainting(Dictionary<Point, Tile> canvas, long playerScore)
{
    StringBuilder sb = new StringBuilder();

    long xMin = canvas.Keys.Min(p => p.X);
    long xMax = canvas.Keys.Max(p => p.X);
    long yMin = canvas.Keys.Min(p => p.Y);
    long yMax = canvas.Keys.Max(p => p.Y);

    sb.AppendLine();
    for (long y = yMin; y <= yMax; y++)
    {
        for (long x = xMin; x <= xMax; x++)
        {
            canvas.TryGetValue(new Point(x, y), out var tile);

            char ch = tile switch
            {
                Tile.Empty => ' ',
                Tile.Paddle => '_',
                Tile.Wall => '█',
                Tile.Block => '#',
                Tile.Ball => 'O'
            };
            sb.Append(ch);
        }
        sb.AppendLine();
    }

    if(playerScore != 0) sb.Append(playerScore);

    return sb.ToString();
}

void SavePainting(string fileName, Dictionary<Point, Tile> canvas)
{
    File.WriteAllText(fileName, ShowPainting(canvas, 0));
}

record struct Point(long X, long Y)
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
    Empty = 0,
    Wall = 1,
    Block = 2,
    Paddle = 3,
    Ball = 4,
}

public enum Direction
{
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

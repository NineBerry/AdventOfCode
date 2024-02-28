using System.Text;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day13\Full.txt";
    string visualizationFileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day13\Visualization.txt";
    long[] program = ReadProgram(fileName);

    Console.WriteLine("Part 1: " + Part1(program, visualizationFileName));
    Console.WriteLine("Part 2: " + Part2(program, onScreen: false)); // Set true to see game live

    Console.ReadLine();
}

long Part1(long[] program, string visualizationFileName)
{
    (var blocksLeft, _) = Play(program, out var canvas, false);
    SavePainting(visualizationFileName, canvas);
    return blocksLeft;
}

long Part2(long[] program, bool onScreen)
{
    program = [2, .. program.Skip(1)];

    (var blocksLeft, var playerScore) = Play(program, out var canvas, onScreen);

    if(blocksLeft == 0)
    {
        return playerScore;
    }

    Console.WriteLine("Lost! Still " + blocksLeft + " blocks left");
    return 0;
}

static long[] ReadProgram(string fileNamePart2)
{
    return File.ReadAllText(fileNamePart2).Split(',').Select(long.Parse).ToArray();
}

(long BlocksLeft, long PlayerScore) Play(long[] program, out Dictionary<Point, Tile> outCanvas, bool onScreen)
{
    Dictionary<Point, Tile> canvas = [];

    Computer computer = new Computer(program);

    int awaitingInput = 0;
    long x = 0;
    long y = 0;

    long playerScore = 0;
    Point ballPoint = new Point();
    Point paddlePoint = new Point();
    
    computer.Input += (c) =>
    {
        if (onScreen)
        {
            Console.Clear();
            Console.WriteLine(ShowPainting(canvas, playerScore));
            Thread.Sleep(1);
        }

        return Math.Sign(ballPoint.X - paddlePoint.X);
    };

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
                Tile tile = (Tile)value;
                Point point = new Point(x, y);
                canvas[point] = tile;

                if (tile == Tile.Paddle)
                {
                    paddlePoint = point;
                }

                if (tile == Tile.Ball)
                {
                    ballPoint = point;
                }
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
                Tile.Paddle => '─',
                Tile.Wall => '█',
                Tile.Block => '#',
                Tile.Ball => 'O',
                _ => throw new NotImplementedException()
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

record struct Point(long X, long Y);

public enum Tile
{
    Empty = 0,
    Wall = 1,
    Block = 2,
    Paddle = 3,
    Ball = 4,
}
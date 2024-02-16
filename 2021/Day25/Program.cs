// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day25\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day25\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.ReadLine();
}

long Part1(string[] input)
{
    Grid grid = new Grid(input);
    int step = 0;


    string previousRepresentation = "";
    string representation = grid.ToString();

    while (previousRepresentation != representation)
    {
        grid.RunGameOfSeaCucumbers();
        
        previousRepresentation = representation;
        representation = grid.ToString();
        step++;
    }

    return step;

}

class Grid
{
    public int Width;
    public int Height;

    public Grid(string[] lines)
    {
        Width = lines[0].Length;
        Height = lines.Length;

        InitSeaCucumbers(lines);
    }

    private void InitSeaCucumbers(string[] lines)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                char tile = lines[y][x];
                Point point = new Point { X = x, Y = y };

                if (tile == '>')
                {
                    EastMovingCucumbers.Add(point);
                }
                else if(tile == 'v')
                {
                    SouthMovingCucumbers.Add(point);
                }
            }
        }
    }

    private HashSet<Point> EastMovingCucumbers = [];
    private HashSet<Point> SouthMovingCucumbers = [];

    public void RunGameOfSeaCucumbers()
    {
        var oldEastMoving = new HashSet<Point>(EastMovingCucumbers);
        var oldSouthMoving = new HashSet<Point>(SouthMovingCucumbers);

        EastMovingCucumbers.Clear();
        SouthMovingCucumbers.Clear();

        foreach(var point in oldEastMoving) 
        {
            var target = point.GetEasternNeighbor(Width);
            if(oldEastMoving.Contains(target) || oldSouthMoving.Contains(target))
            {
                EastMovingCucumbers.Add(point);
            }
            else
            {
                EastMovingCucumbers.Add(target);
            }
        }

        foreach (var point in oldSouthMoving)
        {
            var target = point.GetSouthernNeighbor(Height);
            if (EastMovingCucumbers.Contains(target) || oldSouthMoving.Contains(target))
            {
                SouthMovingCucumbers.Add(point);
            }
            else
            {
                SouthMovingCucumbers.Add(target);
            }
        }
    }

    private char[]? emptyMap = null;
    public override string ToString()
    {
        if (emptyMap == null)
        {
            string line = new string(Enumerable.Repeat('.', Width).ToArray());
            emptyMap = string.Join('\n', Enumerable.Range(0, Height).Select(s => line)).ToCharArray();
        }

        char[] map = (char[])emptyMap.Clone();

        foreach (var obj in SouthMovingCucumbers)
        {
            // +1 for line break
            map[obj.Y * (Width + 1) + obj.X] = 'v';
        }

        foreach (var obj in EastMovingCucumbers)
        {
            // +1 for line break
            map[obj.Y * (Width + 1) + obj.X] = '>';
        }

        return new string(map);
    }
}

public record class Point
{
    public int X;
    public int Y;

    public Point GetEasternNeighbor(int width)
    {
        int newX = X + 1 == width ? 0 : X + 1;
        return this with { X = newX };
    }

    public Point GetSouthernNeighbor(int height)
    {
        int newY = Y + 1 == height ? 0 : Y + 1;
        return this with { Y = newY };
    }
}
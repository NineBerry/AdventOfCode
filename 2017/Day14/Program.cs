// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day14\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day14\Full.txt";
#endif

    string key = File.ReadAllText(fileName);

    string[] grid = MakeGrid(key);

    Console.WriteLine("Part 1: " + Part1(grid));
    Console.WriteLine("Part 2: " + Part2(grid));
    Console.ReadLine();
}

int Part1(string[] grid)
{
    return 
        grid
        .Select(s => s.Count(ch => ch =='#'))
        .Sum();
}

int Part2(string[] grid)
{
    HashSet<Point> allPoints = [];

    for (int x = 0; x < 128; x++)
    {
        for (int y = 0; y < 128; y++)
        {
            if (grid[y][x] == '#')
            {
                allPoints.Add(new Point(x, y));
            }
        }
    }

    int regions = 0;

    while (allPoints.Any())
    {
        regions++;
        FloodRemove(allPoints, allPoints.First());
    }
    
    return regions;
}

void FloodRemove(HashSet<Point> allPoints, Point point)
{
    if (allPoints.Contains(point))
    {
        allPoints.Remove(point);

        foreach(var direction in Point.AllDirections)
        {
            FloodRemove(allPoints, point.GetNeightboringPoint(direction));
        }
    }
}

string[] MakeGrid(string key)
{
    List<string> grid = new List<string>();

    for(int row=0; row < 128; row++)
    {
        string input = key + "-" + row.ToString();
        string hash = KnotHash(input);
        grid.Add(MakeRowFromHash(hash));
    }
    return grid.ToArray();
}


string MakeRowFromHash(string HexString)
{
    string row = "";

    foreach(var ch in HexString)
    {
        byte asByte = Convert.ToByte("" + ch, 16);
        row += asByte.ToString("b4");
    }

    return row.Replace("0", ".").Replace("1", "#");
}


string KnotHash(string input)
{
    const int arrayLength = 256;

    byte[] lengths = input.Select(ch => (byte)ch).ToArray();
    lengths = [.. lengths, 17, 31, 73, 47, 23];

    byte[] array = new byte[arrayLength];
    for (int i = 0; i < arrayLength; i++)
    {
        array[i] = (byte)i;
    }
    int currentPosition = 0;
    int skipSize = 0;

    foreach (var _ in Enumerable.Range(0, 64))
    {
        foreach (int length in lengths)
        {
            ReverseArray(array, currentPosition, length);

            currentPosition = currentPosition + length + skipSize;
            skipSize++;
        }
    }

    byte[] dense = new byte[16];

    for (int i = 0; i < 16; i++)
    {
        dense[i] = array.Skip(i * 16).Take(16).Aggregate((a, b) => (byte)(a ^ b));
    }

    return Convert.ToHexString(dense).ToLowerInvariant();
}

void ReverseArray(byte[] array, int currentPosition, int length)
{
    for (int i = 0; i < length / 2; i++)
    {
        int lowerIndex = currentPosition + i;
        int upperIndex = currentPosition + length - 1 - i;

        byte temp = array[ReduceToArray(lowerIndex, array.Length)];
        array[ReduceToArray(lowerIndex, array.Length)] = array[ReduceToArray(upperIndex, array.Length)];
        array[ReduceToArray(upperIndex, array.Length)] = temp;
    }
}

int ReduceToArray(int inValue, int arrayLength)
{
    return inValue % arrayLength;
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

    public static Direction[] AllDirections = [Direction.North, Direction.South, Direction.West, Direction.East];
}
enum Direction
{
    South,
    West,
    North,
    East
}

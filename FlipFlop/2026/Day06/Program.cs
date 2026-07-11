// #define Sample

using System.Numerics;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day06\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day06\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(lines));
    Console.WriteLine("Part 2: " + Part2(lines));
    Console.WriteLine("Part 3: " + Part3(lines));

    Console.ReadLine();
}

BigInteger Part1(string[] lines)
{
    Map map = new Map(lines, false, false);
    map.ChangeTile(map.StartPoint, Map.Tile.CounterClockwise);
    return map.MakeNumberFromLights();
}

BigInteger Part2(string[] lines)
{
    Map map = new Map(lines, true, false);
    map.ChangeTile(map.StartPoint, Map.Tile.CounterClockwise);
    return map.MakeNumberFromLights();
}

BigInteger Part3(string[] lines)
{
    Map map = new Map(lines, true, true);
    map.ChangeTile(map.StartPoint, Map.Tile.CounterClockwise);
    return map.MakeNumberFromLights();
}

class Map
{
    private Dictionary<Point, Tile> tiles = [];
    private int Height { get; }
    private int Width { get; }

    private bool EnableBluetooth { get; }
    private bool ApplyPrimeRule { get; }

    public Point StartPoint { get; }

    public Map(string[] lines, bool enableBluetooth, bool applyPrimeRule)
    {
        Height = lines.Length;
        Width = lines.First().Length;
        EnableBluetooth = enableBluetooth;
        ApplyPrimeRule = applyPrimeRule;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                Tile tile = (Tile)lines[y][x];
                Point point = new Point(x, y);
                tiles[point] = tile;

                if (tile == Tile.Start) StartPoint = point;
            }
        }
    }

    private bool IsInBounds(Point point)
    {
        return point.X >= 0 && point.X < Width && point.Y >= 0 && point.Y < Height;
    }

    private bool IsStaticGear(Tile tile)
    {
        return tile is Tile.Gear or Tile.Start || (EnableBluetooth && tile is Tile.BluetoothGear);
    }
    private bool IsAnyGear(Tile tile)
    {
        return IsStaticGear(tile) || tile is Tile.Clockwise or Tile.CounterClockwise;
    }

    private bool IsBluetoothSender(Tile tile)
    {
        return (char)tile is >= 'a' and <= 'z';
    }

    private Point FindBlueToothReceiverGear(Tile senderTile)
    {
        char receiverChar = char.ToUpper((char)senderTile);
        foreach (var point in tiles.Keys)
        {
            Tile tile = tiles[point];
            if ((char)tile == receiverChar)
            {
                return point.GetAllNeightboringPoints().Single(p => IsAnyGear(tiles[p]));
            }
        }

        throw new InvalidOperationException($"No receiver found for sender {senderTile}");
    }

    public void ChangeTile(Point point, Tile newTile)
    {
        if (tiles[point] != newTile)
        {
            tiles[point] = newTile;

            foreach (var neighbor in GetNeighboringGears(point))
            {
                ChangeTile(neighbor, ReverseRotation(newTile));
            }
        }
    }

    private Point[] GetNeighboringGears(Point point)
    {
        List<Point> result = [];

        foreach (var neighbor in point.GetAllNeightboringPoints())
        {
            if (IsInBounds(neighbor))
            {
                Tile neighborTile = tiles[neighbor];
                if (IsStaticGear(neighborTile))
                {
                    result.Add(neighbor);
                }

                if (EnableBluetooth && IsBluetoothSender(neighborTile))
                {
                    Point receiverGear = FindBlueToothReceiverGear(neighborTile);

                    if (!ApplyPrimeRule || !IsPartOfPrimeGroup(receiverGear))
                    {
                        result.Add(receiverGear);
                    }
                }
            }
        }

        return [.. result];
    }

    private bool IsPartOfPrimeGroup(Point point)
    {
        int groupSize = GetGearGroupSize(point);
        return Tools.IsPrime(groupSize);
    }

    private int GetGearGroupSize(Point point)
    {
        HashSet<Point> group = [];

        FillGroup(point);

        return group.Count;

        void FillGroup(Point p)
        {
            if(group.Contains(p)) return;

            if (IsInBounds(p) && IsAnyGear(tiles[p]))
            {
                group.Add(p);

                foreach (var neighbor in p.GetAllNeightboringPoints())
                {
                    FillGroup(neighbor);
                }
            }
        }
    }

    private Tile ReverseRotation(Tile rotation)
    {
        return rotation switch
        {
            Tile.Clockwise => Tile.CounterClockwise,
            Tile.CounterClockwise => Tile.Clockwise,
            _ => throw new ArgumentException("Unknown rotation", nameof(rotation)),
        };
    }

    internal BigInteger MakeNumberFromLights()
    {
        string binary = "";

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Point point = new Point(x, y);
                Tile tile = tiles[point];

                if (tile == Tile.Light)
                {
                    var lightState = GetLightState(point);
                    if (lightState == LightState.Low) binary += "0";
                    if (lightState == LightState.High) binary += "1";
                }
            }
        }

        return BigInteger.Parse("0" + binary, System.Globalization.NumberStyles.BinaryNumber);
    }

    private LightState GetLightState(Point point)
    {
        if (tiles[point] != Tile.Light) return LightState.Off;

        foreach (var neighbor in point.GetAllNeightboringPoints().Where(IsInBounds))
        {
            Tile neighborTile = tiles[neighbor];
            if (neighborTile == Tile.Clockwise) return LightState.High;
            if (neighborTile == Tile.CounterClockwise) return LightState.Low;
        }
        return LightState.Off;
    }

    private enum LightState
    {
        Off = 0,
        Low = 1,
        High = 2
    }

    public enum Tile
    {
        None = '\0',
        Start = 'S',
        Gear = '#',
        BluetoothGear = '3',
        Light = '*',
        Clockwise = ')',
        CounterClockwise = '('
    }
}

record struct Point(int X, int Y)
{
    public Point[] GetAllNeightboringPoints()
    {
        var me = this;
        return [.. AllDirections.Select(d => me.GetNeightboringPoint(d))];
    }

    private Point GetNeightboringPoint(Direction direction)
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

    private enum Direction
    {
        None = '\0',
        South = 'v',
        West = '<',
        North = '^',
        East = '>'
    }

    private static readonly Direction[] AllDirections = [Direction.North, Direction.East, Direction.South, Direction.West];
}


static class Tools
{
    // From Stackoverflow https://stackoverflow.com/a/21176886/101087
    public static bool IsPrime(int n)
    {
        if (n > 1)
        {
            return Enumerable.Range(1, n).Where(x => n % x == 0)
                             .SequenceEqual(new[] { 1, n });
        }

        return false;
    }
}
// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day12\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day12\Full.txt";
#endif

    Grid grid = new Grid(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + Part1(grid));
    Console.WriteLine("Part 2: " + Part2(grid));
    Console.ReadLine();
}

long Part1(Grid grid) => grid.Regions.Sum(r => r.GetPrice());

long Part2(Grid grid) => grid.Regions.Sum(r => r.GetDiscountedPrice());


class Grid
{
    private int Height;
    private int Width;
    public readonly Dictionary<Point, char> Plots = [];
    public readonly List<Region> Regions = [];


    public Grid(string[] input)
    {
        Width = input.First().Length;
        Height = input.Length;

        InitializePlots(input);
        InitializeRegions();
    }

    private void InitializePlots(string[] input)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Point point = new Point(x, y);
                Plots[point] = input[y][x];
            }
        }
    }

    private void InitializeRegions()
    {
        HashSet<Point> unassignedPlots = [..Plots.Keys];

        while (unassignedPlots.Any())
        {
            Point startPoint = unassignedPlots.First();

            var plotsInRegion = FindRegion(startPoint);

            Regions.Add(new Region(plotsInRegion));
            
            unassignedPlots.ExceptWith(plotsInRegion);
        }
    }

    private HashSet<Point> FindRegion(Point startPoint)
    {
        char plotType = Plots[startPoint];
        HashSet<Point> result = [];

        FloodFill(startPoint);

        return result;

        void FloodFill(Point point)
        {
            if (!Plots.ContainsKey(point)) return;
            if (result.Contains(point)) return;
            if (Plots[point] != plotType) return;

            result.Add(point);

            foreach(var neighbor in point.GetAllNeighboringPointsCross())
            {
                FloodFill(neighbor);
            }
        }
    }
}

class Region
{
    private HashSet<Point> Plots = [];
    private HashSet<(Point Plot, Direction Direction)> Fences = [];
    private HashSet<(Point Plot, Direction Direction)> FencePosts = [];

    public Region(HashSet<Point> plots)
    {
        Plots.UnionWith(plots);
        InitFencesAndPosts();
    }

    private void InitFencesAndPosts()
    {
        HashSet<(Point, Direction)> result = [];

        foreach(var plot in Plots)
        {
            foreach(var side in plot.GetAllNeighboringPointsWithDirectionCross())
            {
                if (!Plots.Contains(side.Neighbor))
                {
                    Fences.Add((plot, side.Direction));
                }
            }

            foreach (var side in plot.GetAllNeighboringPointsWithDirectionDiagonal())
            {
                if (!Plots.Contains(side.Neighbor))
                {
                    FencePosts.Add((plot, side.Direction));
                }
            }
        }
    }
    private long CountSides()
    {
        long count = 0;

        // We count corners. That's the same number as sides. 
        foreach (var plotFencesAndPosts in Fences.Union(FencePosts).GroupBy(f => f.Plot))
        {
            var directions = plotFencesAndPosts.Select(d => d.Direction).ToHashSet();

            // Corners with fences on both angels
            // e.g. TopLeft corner for marked R
            //        o---
            //        |RRR
            //         ^ 

            if (directions.Contains(Direction.Right)
                && directions.Contains(Direction.Top))
                count++;

            if (directions.Contains(Direction.Top)
                && directions.Contains(Direction.Left))
                count++;

            if (directions.Contains(Direction.Bottom)
                && directions.Contains(Direction.Left))
                count++;

            if (directions.Contains(Direction.Bottom)
                && directions.Contains(Direction.Right))
                count++;

            // Corners with fencepost but no fence on either angle
            // e.g. TopLeft corner for marked R
            //
            //        |RRR  
            //      --oRRR
            //      RRRRRR
            //         ^

            if (directions.Contains(Direction.TopRight)
                && !directions.Contains(Direction.Right)
                && !directions.Contains(Direction.Top))
                count++;

            if (directions.Contains(Direction.TopLeft)
                && !directions.Contains(Direction.Top)
                && !directions.Contains(Direction.Left))
                count++;

            if (directions.Contains(Direction.BottomLeft)
                && !directions.Contains(Direction.Bottom)
                && !directions.Contains(Direction.Left))
                count++;

            if (directions.Contains(Direction.BottomRight)
                && !directions.Contains(Direction.Bottom)
                && !directions.Contains(Direction.Right))
                count++;

        }

        return count;
    }

    public long GetPrice() => Plots.Count * Fences.Count;
    public long GetDiscountedPrice() => Plots.Count * CountSides();
}

record Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.Left => this with { X = X - 1 },
            Direction.Right => this with { X = X + 1 },
            Direction.Bottom => this with { Y = Y + 1 },
            Direction.Top => this with { Y = Y - 1 },

            Direction.BottomLeft => this with { X = X - 1, Y = Y + 1 },
            Direction.BottomRight => this with { X = X + 1, Y = Y + 1 },
            Direction.TopRight => this with { X = X + 1, Y = Y - 1 },
            Direction.TopLeft => this with { X = X - 1, Y = Y - 1 },

            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public Point[] GetAllNeighboringPointsCross()
    {
        return AllDirectionsCross.Select(GetNeightboringPoint).ToArray();
    }

    public (Point Neighbor, Direction Direction)[] GetAllNeighboringPointsWithDirectionCross()
    {
        return AllDirectionsCross.Select(d => (GetNeightboringPoint(d), d)).ToArray();
    }

    public (Point Neighbor, Direction Direction)[] GetAllNeighboringPointsWithDirectionDiagonal()
    {
        return AllDirectionsDiagonal.Select(d => (GetNeightboringPoint(d), d)).ToArray();
    }

    public static Direction[] AllDirectionsCross = [Direction.Left, Direction.Right, Direction.Top, Direction.Bottom];
    public static Direction[] AllDirectionsDiagonal = [Direction.TopLeft, Direction.TopRight, Direction.BottomLeft, Direction.BottomRight];
}

enum Direction
{
    Left,
    Right,
    Top,
    Bottom,
    TopRight,
    BottomRight,
    TopLeft,
    BottomLeft
}
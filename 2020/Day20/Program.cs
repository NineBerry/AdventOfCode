// #define Sample
using System.Text;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day20\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day20\Full.txt";
#endif

    var input = 
        File.ReadAllText(fileName)
        .ReplaceLineEndings("\n")
        .Split("\n\n")
        .Select(s => s.Split("\n"))
        .ToArray();
    
    Puzzle puzzle = new Puzzle(input);
    Console.WriteLine("Part 1: " + Part1(puzzle));

    string[] image = puzzle.GetActualImage();

    Console.WriteLine("Part 2: " + Part2(image));
    Console.ReadLine();
}

long Part1(Puzzle puzzle)
{
    TileVariant[] cornerPieces = [
        puzzle.FinishedPuzzle[0][0], 
        puzzle.FinishedPuzzle[0][puzzle.SideLength - 1], 
        puzzle.FinishedPuzzle[puzzle.SideLength - 1][0], 
        puzzle.FinishedPuzzle[puzzle.SideLength - 1][puzzle.SideLength - 1]];

    return cornerPieces
        .Select(v => (long)v.Tile.ID)
        .Aggregate((a, b) => a * b);
}


long Part2(string[] image)
{
    var variants = image.GetAllRotationsAndFlips();

    foreach(var variant in variants)
    {
        (HashSet<Point> safeSpots, HashSet<Point> dangerousSpots) = SeaMonsterWatch.CountSafeAndDangerousSpots(variant);

        if (dangerousSpots.Any()) return safeSpots.Count();
    }

    return 0;
}


public static class SeaMonsterWatch
{
    public static (HashSet<Point> SafeSpots, HashSet<Point> DangerousSpots) CountSafeAndDangerousSpots(string[] image)
    {
        var allSpots = GetSpots(image);
        HashSet<Point> dangerousSpots = []; 

        for(int x=0; x < image.First().Length; x++)
        {
            for (int y = 0; y < image.Length; y++)
            {
                Point point = new Point(x, y);
                var monsterSpots = GetMonsterSpots(point);

                if(monsterSpots.All(p => allSpots.Contains(p)))
                {
                    dangerousSpots.UnionWith(monsterSpots);
                }
            }
        }

        return (allSpots.Except(dangerousSpots).ToHashSet(), dangerousSpots);
    }

    private static HashSet<Point> GetSpots(string[] image)
    {
        HashSet<Point> result = [];

        for (int x = 0; x < image.First().Length; x++)
        {
            for (int y = 0; y < image.Length; y++)
            {
                if (image[y][x] == '#')
                {
                    result.Add(new Point(x, y));
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    private static HashSet<Point> GetMonsterSpots(Point point)
    {
        // What a monster looks like
        // -------------------------
        //                   # 
        // #    ##    ##    ###
        //  #  #  #  #  #  #   
        
        HashSet<Point> result = [];

        result.Add(point.Add(0, 0));
        result.Add(point.Add(1, 1));
        result.Add(point.Add(4, 1));
        result.Add(point.Add(5, 0));
        result.Add(point.Add(6, 0));
        result.Add(point.Add(7, 1));
        result.Add(point.Add(10, 1));
        result.Add(point.Add(11, 0));
        result.Add(point.Add(12, 0));
        result.Add(point.Add(13, 1));
        result.Add(point.Add(16, 1));
        result.Add(point.Add(17, 0));
        result.Add(point.Add(18, 0));
        result.Add(point.Add(18, -1));
        result.Add(point.Add(19, 0));

        return result;
    }
}


public class Puzzle
{
    public Puzzle(string[][] input)
    {
        Tiles = 
            input
            .Select(l => new Tile(l))
            .ToDictionary(t => t.ID, t => t);

        DetermineCompatibleVariants();
        
        SideLength = (int)Math.Sqrt(Tiles.Count);
        FinishedPuzzle = Enumerable.Range(0, SideLength).Select(_ => new TileVariant[SideLength]).ToArray();

        SolvePuzzle();
    }

    private void DetermineCompatibleVariants()
    {
        foreach (var tile in Tiles.Values) 
        {
            var variantsOfOtherTiles = Tiles.Values.Except([tile]).SelectMany(t => t.Variants).ToArray();

            foreach(var variant in tile.Variants)
            {
                variant.DetermineCompatibleVariants(variantsOfOtherTiles);
            }
        }
    }

    private void SolvePuzzle()
    {
        var topLeftPiece = 
            Tiles.Values
            .SelectMany(t => t.Variants)
            .Where(v => v.CompatibleLeft.Count == 0 && v.CompatibleTop.Count == 0)
            .First();

        PuzzleRow(0, topLeftPiece);
        for(int row=1; row < SideLength; row++)
        {
            var firstPieceInRow = FinishedPuzzle[row-1][0].CompatibleBottom.Single();
            PuzzleRow(row, firstPieceInRow);
        }
    }

    private void PuzzleRow(int row, TileVariant nextPiece)
    {
        FinishedPuzzle[row][0] = nextPiece;
        int col = 1;
        while (col < SideLength)
        {
            nextPiece = nextPiece.CompatibleRight.Single();
            FinishedPuzzle[row][col] = nextPiece;
            col++;
        }
    }

    public string[] GetActualImage()
    {
        List<StringBuilder> result = new();
        const int charsPerTile = 8;

        for(int row=0; row < SideLength; row++)
        {
            foreach (var _ in Enumerable.Range(0, charsPerTile))
            {
                result.Add(new StringBuilder());
            }

            for (int col = 0; col < SideLength; col++)
            {
                var source = FinishedPuzzle[row][col].Grid;

                for(int sourceRow = 0; sourceRow < charsPerTile; sourceRow++)
                {
                    var toAdd = source[sourceRow + 1].Substring(1, charsPerTile);
                    result[charsPerTile * row + sourceRow].Append(toAdd);
                }
            }
        }

        return result.Select(s => s.ToString()).ToArray();
    }

    public int SideLength;
    
    /// <summary>
    /// First dimension is Y, second dimension is X
    /// </summary>
    public readonly TileVariant[][] FinishedPuzzle;
    public readonly Dictionary<int, Tile> Tiles;
}

public class Tile
{
    public Tile(string[] input)
    {
        ID = int.Parse(input[0].Substring(5,4));
        Grid = input.Skip(1).ToArray();

        Variants = MakeVariants();
    }

    private TileVariant[] MakeVariants()
    {
        return Grid.
            GetAllRotationsAndFlips()
            .Select(g => new TileVariant(this, g))
            .ToArray();
    }

    public readonly int ID;
    public readonly string[] Grid;
    public readonly TileVariant[] Variants;
}

public class TileVariant
{
    public TileVariant(Tile tile, string[] grid)
    {
        Tile = tile;
        Grid = grid;

        TopBorder = Grid.First();
        BottomBorder = Grid.Last();
        LeftBorder = new string(Grid.Select(l => l.First()).ToArray());
        RightBorder = new string(Grid.Select(l => l.Last()).ToArray());
    }

    public readonly Tile Tile;
    public readonly string[] Grid;

    public readonly string TopBorder;
    public readonly string BottomBorder;
    public readonly string LeftBorder;
    public readonly string RightBorder;

    public HashSet<TileVariant> CompatibleTop = [];
    public HashSet<TileVariant> CompatibleBottom = [];
    public HashSet<TileVariant> CompatibleLeft = [];
    public HashSet<TileVariant> CompatibleRight = [];

    internal void DetermineCompatibleVariants(TileVariant[] variantsOfOtherTiles)
    {
        CompatibleTop = variantsOfOtherTiles.Where(v => v.BottomBorder == this.TopBorder).ToHashSet();
        CompatibleBottom = variantsOfOtherTiles.Where(v => v.TopBorder == this.BottomBorder).ToHashSet();
        CompatibleLeft = variantsOfOtherTiles.Where(v => v.RightBorder == this.LeftBorder).ToHashSet();
        CompatibleRight = variantsOfOtherTiles.Where(v => v.LeftBorder == this.RightBorder).ToHashSet();
    }
}

public record struct Point(int X, int Y)
{
    public Point Add(int x, int y)
    {
        return new Point(X + x, Y + y);
    }
}

public static class Extensions
{
    public static string[] FlipVertical(this string[] pattern)
    {
        return pattern.Select(s => new string(s.Reverse().ToArray())).ToArray();
    }

    public  static string[] RotateClockwise(this string[] pattern)
    {
        string firstRow = pattern.First();

        var rotatedMap = firstRow.Select(
                (column, columnIndex) => string.Join("", pattern.Reverse().Select(row => row[columnIndex]))
            ).ToArray();

        return rotatedMap;
    }

    public static IEnumerable<string[]> GetAllRotationsAndFlips(this string[] source)
    {
        List<string[]> result = new();

        var rotated = source;
        for (int i = 0; i <= 3; i++)
        {
            result.Add(rotated);
            var flipped = rotated.FlipVertical();
            result.Add(flipped);
            rotated = rotated.RotateClockwise();
        }

        return result.ToArray();
    }
}
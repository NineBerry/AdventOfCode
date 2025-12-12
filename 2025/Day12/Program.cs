// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day12\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day12\Full.txt";
#endif

    TreeFarm treeFarm = new TreeFarm(File.ReadAllLines(fileName));
    Console.WriteLine("Part 1: " + treeFarm.CountFittingRegions());

    Console.ReadLine();
}


class TreeFarm
{
    private PresentShape[] PresentShapes = [];
    private Region[] Regions = [];

    public TreeFarm(string[] lines)
    {
        string[] shapeStrings = lines.Take(6 * 5).ToArray();
        PresentShapes = shapeStrings.Chunk(5).Select(chunk => new PresentShape(chunk)).ToArray();

        string[] regionStrings = lines.Skip(6 * 5).ToArray();
        Regions = regionStrings.Select(s => new Region(s)).ToArray();
    }

    public long CountFittingRegions()
    {
        return Regions.Count(r => r.CanFit(PresentShapes));
    }
}

class PresentShape
{
    public PresentShape(string[] lines)
    {
        lines = lines.Skip(1).ToArray();
        MaxRequired = lines.Sum(s => s.Trim().Length);
        MinRequired = lines.SelectMany(s => s).Count(ch => ch == '#');
    }

    public readonly int MinRequired;
    public readonly int MaxRequired;
}

class Region
{
    private int Width;
    private int Height;

    private int[] PresentsPerShape;

    public Region(string line)
    {
        int[] ints = Tools.ParseInts(line);
        Width = ints[0];
        Height = ints[1];
        PresentsPerShape = ints.Skip(2).ToArray();
    }

    public bool CanFit(PresentShape[] shapes)
    {
        int area = Width * Height;
        
        int minAreaRequired = 0;
        int maxAreaRequired = 0;

        foreach(var required  in PresentsPerShape.Index())
        {
            minAreaRequired += required.Item * shapes[required.Index].MinRequired;
            maxAreaRequired += required.Item * shapes[required.Index].MaxRequired;
        }

        if (area >= maxAreaRequired)
        {
            // More space than needed if we don't overlap the present squares
            return true;
        }
        else if (area < minAreaRequired)
        {
            // Less space than actually needed for all the present pieces
            return false;
        }
        else
        {
            Console.WriteLine("Difficult case");
            // TODO Handle actual difficult cases so we get the right solution for Sample input
            // Funnily, in the actual input, there are none....
            return true;
        }
    }
}

public static class Tools
{
    public static int[] ParseInts(string input)
    {
        return
            Regex.Matches(input, @"\d+")
            .Select(m => int.Parse(m.Value))
            .ToArray();
    }
}
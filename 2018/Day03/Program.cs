// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day03\Full.txt";
#endif

    Claim[] claims = File
        .ReadAllLines(fileName)
        .Select(s => new Claim(s))
        .ToArray();

    Fabric fabric = new Fabric();

    foreach (Claim claim in claims)
    {
        fabric.AddClaim(claim); 
    }

    Console.WriteLine("Part 1: " + Part1(fabric));
    Console.WriteLine("Part 2: " + Part2(fabric, claims));
    Console.ReadLine();
}

long Part1(Fabric fabric)
{
    return fabric.Where(pair => pair.Value.Count >= 2).Count();
}

int Part2(Fabric fabric, Claim[] claims)
{
    var allClaimIds = 
        claims
        .Select(c => c.ID)
        .ToHashSet();

    var impossibleClaimIds =
        fabric
        .Where(pair => pair.Value.Count >= 2)
        .SelectMany(pair => pair.Value)
        .ToHashSet();
    
    var possibleClaimIds = allClaimIds.Except(impossibleClaimIds);
    return possibleClaimIds.Single();
}


public record Point(int X, int Y);

public class Fabric: Dictionary<Point, HashSet<int>>
{
    public void AddClaim(Claim claim)
    {
        for(int x = claim.MinX; x <= claim.MaxX; x++)
        {
            for (int y = claim.MinY; y <= claim.MaxY; y++)
            {
                AddPoint(new Point(x, y), claim.ID);
            }
        }
    }

    public void AddPoint(Point point, int claimID)
    {
        if(!TryGetValue(point, out var ids))
        {
            ids = [];
            Add(point, ids);
        }

        ids.Add(claimID);
    }
}

public class Claim
{
    public Claim(string input)
    {
        int[] values = Regex.Matches(input, "\\d+").Select(m => int.Parse(m.Value)).ToArray();

        ID = values[0];
        MinX = values[1];
        MinY = values[2];
        MaxX = MinX + values[3] -1;
        MaxY = MinY + values[4] -1;
    }

    public int ID;
    public int MinX;
    public int MaxX;
    public int MinY;
    public int MaxY;
}
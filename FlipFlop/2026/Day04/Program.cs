// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day04\Sample.txt";
    int part1Cut = 8;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day04\Full.txt";
    int part1Cut = 400;
#endif

    Flower flower = new Flower(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + Part1(flower, part1Cut));
    Console.WriteLine("Part 2: " + Part2(flower));
    Console.WriteLine("Part 3: " + Part3(flower));

    Console.ReadLine();
}

long Part1(Flower flower, int cut)
{
    return flower.CountLeavesAbove(cut);
}

long Part2(Flower flower)
{
    return flower.CountSwapsClimbing();
}

long Part3(Flower flower)
{
    long workers = 0;

    while(flower.HasLeaves())
    {
        workers++;
        flower.HarvestTop();
    }

    return workers;
}

class Flower
{
    private string[] representation;

    public Flower(string[] representation)
    {
        var representationHeight = representation.Length;
        this.representation =
            representation
            .Take(representationHeight - 1) // Cut root
            .Skip(3) // Cut top
            .Reverse() // From bottom to top
            .ToArray();
    }

    public long CountLeavesAbove(int cut)
    {
        return representation.Skip(cut).Count(HasLeave);
    }

    public long CountSwapsClimbing()
    {
        var leaves = representation.Where(HasLeave);

        long swaps = 0;

        string previous = leaves.First();
        foreach (var leave in leaves)
        {
            if (leave != previous)
            {
                swaps++;
                previous = leave;
            }
        }

        return swaps;
    }

    public void HarvestTop()
    {
        // First remove all lines without leaves
        representation = representation.Where(HasLeave).ToArray();

        string previous = representation.First();

        for (int i = 0; i < representation.Length; i++)
        {
            if (representation[i] != previous)
            {
                previous = representation[i];
                representation[i - 1] = "  |";
            }
        }

        representation[representation.Length - 1] = "  |";
    }

    public bool HasLeaves()
    {
        return representation.Any(HasLeave);
    }

    private static bool HasLeave(string line)
    {
        return line.Contains('o');
    }
}
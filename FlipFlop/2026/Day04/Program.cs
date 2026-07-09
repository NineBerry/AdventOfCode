// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day04\Sample.txt";
    int part1Cut = 8;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day04\Full.txt";
    // string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day04\Full2.txt";
    int part1Cut = 400;
#endif

    Flower flower = new Flower(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + Part1(flower, part1Cut));
    Console.WriteLine("Part 2: " + Part2(flower));
    // Console.WriteLine("Part 3: " + Part3(flower));
    Console.WriteLine("Part 3: " + Part3Optimized(flower));

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

long Part3Optimized(Flower flower)
{
    return flower.HarvestAll();
}


class Flower
{
    enum LeafType
    {
        None,
        Left,
        Right
    }

    private LeafType[] leaves;

    public Flower(string[] representation)
    {
        var representationHeight = representation.Length;
        var strippedAndReversed =
            representation
            .Take(representationHeight - 1) // Cut root
            .Skip(3) // Cut top
            .Reverse() // From bottom to top
            .ToArray();

        leaves = strippedAndReversed.Select(Translate).ToArray();

        LeafType Translate(string line)
        {
            return line switch
            {
                "  |" => LeafType.None,
                "o-|" => LeafType.Left,
                "  |-o" => LeafType.Right,
                _ => throw new InvalidOperationException($"Invalid line: {line}")
            };
        }
    }

    public long CountLeavesAbove(int cut)
    {
        return leaves.Skip(cut).Count(HasLeave);
    }

    public long CountSwapsClimbing()
    {
        var onlyLeaves = leaves.Where(HasLeave);

        long swaps = 0;

        var previous = onlyLeaves.First();
        foreach (var leave in onlyLeaves)
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
        leaves = leaves.Where(HasLeave).ToArray();

        LeafType previous = leaves.First();

        for (int i = 0; i < leaves.Length; i++)
        {
            if (leaves[i] != previous)
            {
                previous = leaves[i];
                leaves[i - 1] = LeafType.None;
            }
        }

        leaves[leaves.Length - 1] = LeafType.None;
    }

    public bool HasLeaves()
    {
        return leaves.Any(HasLeave);
    }

    private static bool HasLeave(LeafType leaf)
    {
        return leaf != LeafType.None;
    }

    internal long HarvestAll()
    {
        List<(LeafType LeafType, int Count)> rleLeaves = ConvertToRunLength(leaves);
        rleLeaves = MergeRunLength(rleLeaves);

        int workers = 0;

        while(rleLeaves.Any())
        {
            int batchWorkers = rleLeaves.Min(l => l.Count);

            rleLeaves = rleLeaves.Select(l => (l.LeafType, l.Count - batchWorkers)).ToList();
            rleLeaves = MergeRunLength(rleLeaves);

            workers += batchWorkers;
        }

        return workers;
    }

    private List<(LeafType LeafType, int Count)> MergeRunLength(List<(LeafType LeafType, int Count)> rleLeaves)
    {
        List<(LeafType LeafType, int Count)> result = new();

        LeafType previous = LeafType.None;

        foreach (var leave in rleLeaves)
        {
            if(leave.Count == 0) continue;

            if (leave.LeafType != previous)
            {
                result.Add(leave);
                previous = leave.LeafType;
            }
            else
            {
                var last = result.Last();
                result[result.Count - 1] = (last.LeafType, last.Count + leave.Count);
            }
        }

        return result;
    }

    private List<(LeafType LeafType, int Count)> ConvertToRunLength(LeafType[] leaves)
    {
        return leaves.Where(HasLeave).Select(l => (l, 1)).ToList();
    }
}
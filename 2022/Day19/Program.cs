// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day19\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day19\Full.txt";
#endif

    BluePrint[] bluePrints = File.ReadAllLines(fileName).Select(s => new BluePrint(s)).ToArray();

    Console.WriteLine("Part 1: " + Part1(bluePrints));
    Console.WriteLine("Part 2: " + Part2(bluePrints));
    Console.ReadLine();
}

long Part1(BluePrint[] bluePrints)
{
    return bluePrints.Select(b => b.BluePrintId * GetMaxGeodes(b, 24)).Sum();
}

long Part2(BluePrint[] bluePrints)
{
    long[] results = bluePrints.Take(3).Select(b => GetMaxGeodes(b, 32)).ToArray();
    return results.Aggregate((a,b) => a * b);
}


// Potential improvements for speed:
// * Use a different data structure for cache
// * Find more brances to prune earlier.
// * Don't recurse for every minute, but for every robot created.
// (Decide in advance which robot to create next, then skip those minutes)

long GetMaxGeodes(BluePrint bluePrint, int minutes)
{
    Dictionary<(int OreRobots, int ClayRobots, int ObsidianRobots, int GeodeRobots, int Ore, int Clay, int Obsidian, int MinutesLeft), long> cache = [];

    int maxOre = ((int[])[bluePrint.OreRobotOreCosts, bluePrint.ClayRobotOreCosts, bluePrint.ObsidianRobotOreCosts, bluePrint.GeodeRobotOreCosts]).Max();
    int maxClay = bluePrint.ObsidianRobotClayCosts;
    int maxObsidian = bluePrint.GeodeRobotObsidianCosts;

    long GetMaxGeodes(int oreRobots, int clayRobots, int obsidianRobots, int geodeRobots, int ore, int clay, int obsidian, int minutesLeft)
    {
        if (minutesLeft == 1) return geodeRobots;

        if (oreRobots > maxOre) return 0;
        if (clayRobots > maxClay) return 0;
        if (obsidianRobots > maxObsidian) return 0;

        if (cache.TryGetValue((oreRobots, clayRobots, obsidianRobots, geodeRobots, ore, clay, obsidian, minutesLeft), out var cached))
        {
            return cached;
        }

        int newOre = ore + oreRobots;
        int newClay = clay + clayRobots;
        int newObsidian = obsidian + obsidianRobots;
        int newGeode = geodeRobots;

        long maxResult = newGeode;

        // Create Geode robot if possible
        if (bluePrint.GeodeRobotOreCosts <= ore && bluePrint.GeodeRobotObsidianCosts <= obsidian)
        {
            long result = newGeode + GetMaxGeodes(oreRobots, clayRobots, obsidianRobots, geodeRobots + 1, newOre - bluePrint.GeodeRobotOreCosts, newClay, newObsidian - bluePrint.GeodeRobotObsidianCosts, minutesLeft - 1);
            maxResult = Math.Max(maxResult, result);
        }
        
        // Create Obsidian robot if possible
        if (bluePrint.ObsidianRobotOreCosts <= ore && bluePrint.ObsidianRobotClayCosts <= clay)
        {
            long result = newGeode + GetMaxGeodes(oreRobots, clayRobots, obsidianRobots + 1, geodeRobots, newOre - bluePrint.ObsidianRobotOreCosts, newClay - bluePrint.ObsidianRobotClayCosts, newObsidian, minutesLeft - 1);
            maxResult = Math.Max(maxResult, result);
        }

        // Create Clay robot if possible
        if (bluePrint.ClayRobotOreCosts <= ore)
        {
            long result = newGeode + GetMaxGeodes(oreRobots, clayRobots + 1, obsidianRobots, geodeRobots, newOre - bluePrint.ClayRobotOreCosts, newClay, newObsidian, minutesLeft - 1);
            maxResult = Math.Max(maxResult, result);
        }

        // Create Ore robot if possible
        if (bluePrint.OreRobotOreCosts <= ore)
        {
            long result = newGeode + GetMaxGeodes(oreRobots + 1, clayRobots, obsidianRobots, geodeRobots, newOre - bluePrint.OreRobotOreCosts, newClay, newObsidian, minutesLeft - 1);
            maxResult = Math.Max(maxResult, result);
        }

        // Step 1 minute without creating robot
        {
            long result = newGeode + GetMaxGeodes(oreRobots, clayRobots, obsidianRobots, geodeRobots, newOre, newClay, newObsidian, minutesLeft - 1);
            maxResult = Math.Max(maxResult, result);
        }

        cache.Add((oreRobots, clayRobots, obsidianRobots, geodeRobots, ore, clay, obsidian, minutesLeft), maxResult);

        return maxResult;
    }


    long res = GetMaxGeodes(1, 0, 0, 0, 0, 0, 0, minutes);
    Console.WriteLine($"BP: {bluePrint.BluePrintId} => {res}");

    return res;
}

struct BluePrint
{
    public BluePrint(string input)
    {
        int[] ints = Regex.Matches(input, "\\d+").Select(m => int.Parse(m.Value)).ToArray();

        BluePrintId = ints[0];
        OreRobotOreCosts = ints[1];
        ClayRobotOreCosts = ints[2];
        ObsidianRobotOreCosts = ints[3];
        ObsidianRobotClayCosts = ints[4];
        GeodeRobotOreCosts = ints[5];
        GeodeRobotObsidianCosts = ints[6];
    }

    public readonly int BluePrintId;

    public readonly int OreRobotOreCosts;
    public readonly int ClayRobotOreCosts;
    public readonly int ObsidianRobotOreCosts;
    public readonly int ObsidianRobotClayCosts;
    public readonly int GeodeRobotOreCosts;
    public readonly int GeodeRobotObsidianCosts;
}

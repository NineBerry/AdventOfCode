// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day02\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day02\Full.txt";
#endif
    List<int[]> reports = [.. File.ReadAllLines(fileName).Select(ParseReport)];
    
    Console.WriteLine("Part 1: " + Part1(reports));
    Console.WriteLine("Part 2: " + Part2(reports));
    Console.ReadLine();
}

long Part1(IEnumerable<int[]> reports)
{
    return reports.Count(IsSafe);
}
long Part2(IEnumerable<int[]> reports)
{
    return reports.Count(IsSafeWithDampener);
}


int[] ParseReport(string line)
{
    return [.. Regex.Matches(line, @"\d+").Select(m => int.Parse(m.Value))];
}

bool IsSafe(IEnumerable<int> report)
{
    return (CheckGaps(report, [-1, -2, -3]) || CheckGaps(report, [+1, +2, +3]));
}

bool CheckGaps(IEnumerable<int> report, HashSet<int> allowedGaps)
{
    foreach(var pair in report.Zip(report.Skip(1)))
    {
        int diff = pair.Second - pair.First;
        if(!allowedGaps.Contains(diff)) return false;
    }

    return true;
}

bool IsSafeWithDampener(int[] report)
{
    bool isSafe = IsSafe(report);

    // TODO: Improve performance by being smarter in IsSafe and not actually
    // trying all the variants explicitely.

    if (!isSafe)
    {
        foreach (var index in Enumerable.Range(0, report.Length))
        {
            List<int> modified = report.ToList();
            modified.RemoveAt(index);
            isSafe = IsSafe(modified);

            if (isSafe) break;
        }
    }

    return isSafe;
}


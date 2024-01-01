// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day03\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

int[] ParseLine(string line)
{
    return Regex.Matches(line, "[0-9]+").Select(m => int.Parse(m.Value)).ToArray();
}

bool IsValidTriangle(int[] sides)
{
    return (sides[0] + sides[1] > sides[2] ) && (sides[0] + sides[2] > sides[1]) && (sides[2] + sides[1] > sides[0]);
}

long CountPossibleTriangles(IEnumerable<int[]> triangles)
{
    return triangles.Count(IsValidTriangle);
}

long Part1(string[] lines)
{
    var triangles = lines.Select(ParseLine);
    return CountPossibleTriangles(triangles);
}

long Part2(string[] lines)
{
    List<int[]> triangles = new();

    for (int i = 0; i < lines.Length; i += 3)
    {
        var t1 = ParseLine(lines[i]);
        var t2 = ParseLine(lines[i + 1]);
        var t3 = ParseLine(lines[i + 2]);

        triangles.Add([t1[0], t2[0], t3[0]]);
        triangles.Add([t1[1], t2[1], t3[1]]);
        triangles.Add([t1[2], t2[2], t3[2]]);
    }

    return CountPossibleTriangles(triangles);
}


// #define Sample

using System.Text;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day18\Sample.txt";
    int wantedRowsPart1 = 10;
    int wantedRowsPart2 = 10;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day18\Full.txt";
    int wantedRowsPart1 = 40;
    int wantedRowsPart2 = 400_000;
#endif

    string startLine = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Solve(startLine, wantedRowsPart1));
    Console.WriteLine("Part 2: " + Solve(startLine, wantedRowsPart2));
    Console.ReadLine();
}

int Solve(string startLine, int wantedRows)
{
    int safeCount = 0;

    string line = startLine;

    foreach(var _ in Enumerable.Range(1, wantedRows))
    {
        safeCount += line.Count(ch => ch == '.');
        line = GenerateNextLine(line);
    }

    return safeCount;
}

string GenerateNextLine(string line)
{
    StringBuilder sb = new StringBuilder();

    for (int i = 0; i < line.Length; i++)
    {
        bool leftIsTrap = IsTrap(i - 1);
        bool centerIsTrap = IsTrap(i);
        bool rightIsTrap = IsTrap(i + 1);

        bool isTrap =
            (leftIsTrap && centerIsTrap && !rightIsTrap)
            || (rightIsTrap && centerIsTrap && !leftIsTrap)
            || (leftIsTrap && !centerIsTrap && !rightIsTrap)
            || (!leftIsTrap && !centerIsTrap && rightIsTrap);

        sb.Append(isTrap ? '^' : '.');
    }

    return sb.ToString();

    bool IsTrap(int pos)
    {
        if (pos < 0 || pos >= line.Length) return false;
        return line[pos] == '^';
    }
}
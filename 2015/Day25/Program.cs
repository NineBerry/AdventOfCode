// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day25\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day25\Full.txt";
#endif

    int[] values = Regex.Matches(File.ReadAllText(fileName), "\\d+").Select(m => int.Parse(m.Value)).ToArray();
    int row = values[0];
    int column = values[1];

    Console.WriteLine("Part 1: " + Part1(row, column));
    Console.ReadLine();
}

long Part1(int row, int column)
{
    Point point = new Point(1, 1);
    long value = 20151125;

    while (point.X != column || point.Y != row)
    {
        point = point.GetNext();
        value = GetNextValue(value);
    }

    return value;
}

long GetNextValue(long value)
{
    value = value * 252533;
    value = value % 33554393;
    return value;
}

record Point(int X, int Y)
{
    public Point GetNext()
    {
        if(Y == 1)
        {
            return new Point(1, X + 1);
        }
        else
        {
            return new Point(X + 1, Y - 1);
        }
    }
}
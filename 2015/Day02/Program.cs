// #define Sample

using System.Text.RegularExpressions;
using Box = (int Length, int Width, int Height);

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day02\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day02\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    var boxes = input.Select(parseLine).ToArray();

    Console.WriteLine("Part 1: " + Part1(boxes));
    Console.WriteLine("Part 2: " + Part2(boxes));
    Console.ReadLine();
}

long Part1(Box[] boxes)
{
    return boxes.Sum(calculatePaper);
}

long Part2(Box[] boxes)
{
    return boxes.Sum(calculateRibbon);
}

Box parseLine(string line)
{
    int[] numbers = Regex.Matches(line, "[0-9]+").Select(m => int.Parse(m.Value)).ToArray();
    return (numbers[0], numbers[1], numbers[2]);
}

int calculatePaper(Box box)
{
    int surface = 2 * box.Length * box.Width + 2 * box.Width * box.Height + 2 * box.Height * box.Length;
    int extra = new int[] { box.Length * box.Width, box.Length * box.Height, box.Height * box.Width }.Min();

    return surface + extra;
}

int calculateRibbon(Box box)
{
    int volume = box.Width * box.Height *  box.Length;
    int bow = new int[] { 2 * (box.Length + box.Width), 2 * (box.Length + box.Height), 2 * (box.Height + box.Width) }.Min();

    return volume + bow;
}

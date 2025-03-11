// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day17\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day17\Full.txt";
#endif

    string input = File.ReadAllText(fileName);
    var numbers = input.ExtractNumbers();

    var targetXMin = numbers[0];
    var targetXMax = numbers[1];
    var targetYMin = numbers[2];
    var targetYMax = numbers[3];

    (int maxYOfValidThrows, int countValidThrows) = CalculateValidThrows(targetXMin, targetXMax, targetYMin, targetYMax);

    Console.WriteLine("Part 1: " + maxYOfValidThrows);
    Console.WriteLine("Part 2: " + countValidThrows);
    Console.ReadLine();
}

(int maxYOfValidThrows, int countValidThrows) CalculateValidThrows(int targetXMin, int targetXMax, int targetYMin, int targetYMax)
{
    int maxYOfValidThrows = 0;
    int countValidThrows = 0;

    int xMaxSpan = targetXMax;
    int yMaxSpan = Math.Abs(targetYMin);

    foreach (var x in Enumerable.Range(1, xMaxSpan))
    {
        foreach(var y in Enumerable.Range(-yMaxSpan, 2 * yMaxSpan))
        {
            (bool valid, int maxY) = Simulate(x, y, targetXMin, targetXMax, targetYMin, targetYMax);

            if (valid)
            {
                countValidThrows++; 
                maxYOfValidThrows = Math.Max(maxYOfValidThrows, maxY);
            }
        }
    }

    return (maxYOfValidThrows, countValidThrows);
}

(bool valid, int maxY) Simulate(int velocityX, int velocityY, int targetXMin, int targetXMax, int targetYMin, int targetYMax)
{
    int maxY = int.MinValue;

    int currentX = 0;
    int currentY = 0;

    while (true)
    {
        maxY = Math.Max(maxY, currentY);

        currentX += velocityX;
        currentY += velocityY;

        if (velocityX > 0) velocityX--;
        velocityY--;

        // Check for hit
        if (
            currentX >= targetXMin
            && currentX <= targetXMax
            && currentY >= targetYMin
            && currentY <= targetYMax)
        {
            return (true, maxY);
        }

        // Check for going beyond
        if (
            currentX > targetXMax 
            || currentY < targetYMin)
        {
            return (false, int.MinValue);
        }
    }
}

public static class Tools
{
    public static int[] ExtractNumbers(this string input)
    {
        return Regex.Matches(input, @"-?\d+").Select(m => int.Parse(m.Value)).ToArray();
    }
}
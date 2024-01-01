// #define Sample
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day14\Sample.txt";
    int duration = 1000;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day14\Full.txt";
    int duration = 2503;
#endif

    string[] input = File.ReadAllLines(fileName);

    Reindeer[] reindeers = input.Select(line => new Reindeer(line)).ToArray();

    Console.WriteLine("Part 1: " + Part1(reindeers, duration));
    Console.WriteLine("Part 2: " + Part2(reindeers, duration));
    Console.ReadLine();
}

long Part1(Reindeer[] reindeers, int duration)
{
    var kilometers = reindeers.Select(r => r.KilometersAfterSeconds(duration));
    return kilometers.Max();
}

long Part2(Reindeer[] reindeers, int duration)
{
    Reindeer.RunScores(reindeers, duration);
    return reindeers.Select(reindeer => reindeer.Score).Max();
}

class Reindeer
{
    public Reindeer(string input)
    {
        Name = input.Split(' ', 2)[0];
        int[] numbers = Regex.Matches(input, "[0-9]+").Select(m => int.Parse(m.Value)).ToArray();
        Speed = numbers[0];
        SpeedingDuration = numbers[1];
        RestingDuration = numbers[2];
    }

    public string Name;
    public int Speed;
    public int SpeedingDuration;
    public int RestingDuration;
    public int Score;

    public int KilometersAfterSeconds(int seconds)
    {
        int fullCycles = seconds / (SpeedingDuration + RestingDuration);
        int restSeconds = seconds % (SpeedingDuration + RestingDuration);
        int restSpeedingSeconds = Math.Min(restSeconds, SpeedingDuration);
        int distance = (fullCycles * SpeedingDuration * Speed) + (restSpeedingSeconds * Speed);

        return distance;
    }

    public static void RunScores(Reindeer[] reindeers, int seconds)
    {
        foreach (var reindeer in reindeers) reindeer.Score = 0; 
        
        for(int i=1; i <= seconds; i++)
        {
            var tempResults = reindeers.Select(r => (Reindeer: r, Kilometers: r.KilometersAfterSeconds(i)));
            var best = tempResults.Select(pair => pair.Kilometers).Max();
            foreach (var reindeer in tempResults.Where(pair => pair.Kilometers == best).Select(pair => pair.Reindeer).ToArray())
            {
                reindeer.Score++;
            }
        }
    }
}
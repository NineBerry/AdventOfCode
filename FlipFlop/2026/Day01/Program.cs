// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\FlipFlop\2026\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\FlipFlop\2026\Day01\Full.txt";
#endif

    int[] temperatures = File.ReadAllLines(fileName).Select(int.Parse).ToArray();
    Console.WriteLine("Part 1: " + Part1(temperatures));
    Console.WriteLine("Part 2: " + Part2(temperatures));
    Console.WriteLine("Part 3: " + Part3(temperatures));

    Console.ReadLine();
}

long Part1(int[] temperatures)
{
    return
        temperatures
        .Where(t => t < 60)
        .Select(t=> timeToMakePerfectTemperature(t, 60))
        .Sum();
}


long Part2(int[] temperatures)
{
    return
        temperatures
        .Sum(t => timeToMakePerfectTemperature(t, 60));

}

long Part3(int[] temperatures)
{
    int colleageCount = temperatures.Length / 2;
    
    int[] cupTemperatures = temperatures.Take(colleageCount).ToArray();
    int[] preferredTemperatures = temperatures.Skip(colleageCount).ToArray();

    return Enumerable.Range(0, colleageCount)
        .Sum(i => timeToMakePerfectTemperature(cupTemperatures[i], preferredTemperatures[i]));
}

long timeToMakePerfectTemperature(int currentTemperature, int perfectTemperature)
{
    if(currentTemperature > perfectTemperature)
    {
        return (currentTemperature - perfectTemperature) * 5;
    }
    else if(currentTemperature < perfectTemperature)
    {
        return perfectTemperature - currentTemperature;
    }

    return 0;
}
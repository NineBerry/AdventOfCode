// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day03\Full.txt";
#endif

    var lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(lines));
    Console.WriteLine("Part 2: " + Part2(lines));
    Console.ReadLine();
}

long Part1(string[] lines)
{
    long gammeRate = 0;
    long epsilonRate = 0;

    for(int i = 0; i< lines[0].Length; i++)
    {
        gammeRate <<= 1;
        epsilonRate <<= 1;
        
        long zeroCount = lines.Count(s => s[i] == '0');
        long oneCount = lines.Length - zeroCount;

        if (oneCount >= zeroCount) gammeRate++;
        if (zeroCount >= oneCount) epsilonRate++;
    }


    return gammeRate * epsilonRate;
}

long Part2(string[] lines)
{
    long oxygenRating = 0;
    long co2Rating = 0;

    string[] filtered = lines;

    for (int i = 0; i < lines[0].Length; i++)
    {
        long zeroCount = filtered.Count(s => s[i] == '0');
        long oneCount = filtered.Count() - zeroCount;

        char filterCriteria = oneCount >= zeroCount ? '1' : '0';
        filtered = filtered.Where(l => l[i] == filterCriteria).ToArray();

        if(filtered.Count() == 1)
        {
            oxygenRating = Convert.ToInt64(filtered.Single(), 2);
            break;
        }
    }

    filtered = lines;

    for (int i = 0; i < lines[0].Length; i++)
    {
        long zeroCount = filtered.Count(s => s[i] == '0');
        long oneCount = filtered.Count() - zeroCount;

        char filterCriteria = zeroCount <= oneCount  ? '0' : '1';
        filtered = filtered.Where(l => l[i] == filterCriteria).ToArray()    ;

        if (filtered.Count() == 1)
        {
            co2Rating = Convert.ToInt64(filtered.Single(), 2);
            break;
        }
    }

    return oxygenRating * co2Rating;
}
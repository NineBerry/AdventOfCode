// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day10\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day10\Full.txt";
#endif

    long[] values = File.ReadAllLines(fileName).Select(long.Parse).ToArray();

    long deviceJolts = values.Max() + 3;
    long[] sortedJolts = [0, .. values.Order(), deviceJolts];

    Console.WriteLine("Part 1: " + Part1(sortedJolts));
    Console.WriteLine("Part 2: " + Part2(sortedJolts));
    Console.ReadLine();
}

long Part1(long[] sortedJolts)
{
    var differences = sortedJolts
        .Zip(sortedJolts.Skip(1))
        .Select(pair => pair.Second - pair.First)
        .ToArray();

    int count1s = differences.Count(l => l == 1);
    int count3s = differences.Count(l => l == 3);

    return count1s * count3s;
}

long Part2(long[] sortedJolts)
{
    var differences = sortedJolts
        .Zip(sortedJolts.Skip(1))
        .Select(pair => pair.Second - pair.First)
        .ToArray();

    var factors = GetDifferenceOneGroupLengths(differences).Select(GetFactorByGroupLength);

    return factors.Aggregate((a,b) => a * b);
}

long[] GetDifferenceOneGroupLengths(long[] differences)
{
    List<long> result = [];

    int count = 0;

    foreach (var difference in differences)
    {
        if(difference == 1) 
        {  
            count++; 
        }
        else if(difference == 3)
        {
            if(count != 0) result.Add(count);
            count = 0;
        }
        else
        {
            throw new ApplicationException("Unexpected difference");
        }
    }

    return result.ToArray();
}

long GetFactorByGroupLength(long groupLength)
{
    return groupLength switch
    {
        1 => 1,
        2 => 2,
        3 => 4,
        4 => 7,
        _ => throw new ApplicationException("Unexpected group length")
    };
}



/// Notes for Part 2:
/*

We noticed that all the samples and the full input have only differences 
1 and 3. 2 can be ignored so far.

--------------------------------------------------------------------------
Sample Big
11113111133111311331111313311113

19208 = 2·4·7·7·7·7

--------------------------------------------------------------------------
Sample Small
131113113133

8 = 2·2·2 = 4 * 2 

----------------------------------------------------------------------

313    -> Factor 1
3113   -> Factor 2 
31113  -> Factor 4
311113 -> Factor 7
*/
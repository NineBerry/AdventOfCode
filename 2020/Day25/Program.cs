// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day25\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day25\Full.txt";
#endif

    long[] publicKeys = File.ReadAllLines(fileName).Select(long.Parse).ToArray();

    Console.WriteLine("Part 1: " + Part1(publicKeys));
    Console.ReadLine();
}

long Part1(long[] publicKeys)
{
    int loopSize = 0;
    long value = 1;
    long subjectValue = 7;
    long publicKeyToUse;

    while (true)
    {
        loopSize++;
        value = (value * subjectValue) % 20201227;

        if(value == publicKeys[0])
        {
            publicKeyToUse = publicKeys[1];
            break;
        }

        if (value == publicKeys[1])
        {
            publicKeyToUse = publicKeys[0];
            break;
        }
    }

    value = 1;
    subjectValue = publicKeyToUse;
    foreach(long _ in Enumerable.Range(1, loopSize))
    {
        value = (value * subjectValue) % 20201227;
    }


    return value;
}

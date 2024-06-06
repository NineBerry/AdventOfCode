// #define Sample

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2019\Day16\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2019\Day16\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2019\Day16\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2019\Day16\Full.txt";
#endif

    int[] inputPart1 = File.ReadAllText(fileNamePart1).Select(char.GetNumericValue).Select(d => (int)d).ToArray();
    Console.WriteLine("Part 1: " + Part1(inputPart1));

    int[] inputPart2 = File.ReadAllText(fileNamePart2).Select(char.GetNumericValue).Select(d => (int)d).ToArray();
    Console.WriteLine("Part 2: " + Part2(inputPart2));
    Console.ReadLine();
}


string Part1(int[] input)
{
    foreach(var _ in Enumerable.Range(1, 100))
    {
        input = FFT.Transform(input);
    }

    return string.Join("", input.Take(8));
}

string Part2(int[] input)
{
    // Idea is: digits are only influenced by digits at the 
    // same position or higher from the previous sequence
    // So we can ignore all the stuff up to the position
    // we are interested in

    int offset = int.Parse(string.Join("", input.Take(7)));
    int count = (input.Length * 10_000) - offset;

    // Make sequence only from the position we are interested in
    int[] expandedInput = new int[count];
    foreach(var i in Enumerable.Range(0, count))
    {
        expandedInput[i] = input[(offset + i) % input.Length];
    }

    foreach (var _ in Enumerable.Range(1, 100))
    {
        // We can use TransformFixed1 because we are looking at the 
        // latter part of the actual sequence
        expandedInput = FFT.TransformFixed1(expandedInput);
    }

    return string.Join("", expandedInput.Take(8));
}


public static class FFT
{
    internal static int[] Transform(int[] input)
    {
        int[] result = new int[input.Length];

        foreach (var i in Enumerable.Range(0, input.Length))
        {
            result[i] = Math.Abs(input.Select((v, index) => v * GetFactor(i + 1, index)).Sum()) % 10;
        }

        return result;
    }

    private static int GetFactor(int groupLength, int position)
    {
        position++;

        int groupNumber = position / groupLength;
        int groupTyp = groupNumber % 4;

        return groupTyp switch
        {
            0 => 0,
            1 => 1,
            2 => 0,
            3 => -1,
            _ => throw new ArgumentException(),
        };
    }

    // This is used when we are only interested in the latter part
    // of the sequence. Here, we can assume we always have the factor +1
    // and just use a sum to get the nect sequence
    internal static int[] TransformFixed1(int[] input)
    {
        int[] result = new int[input.Length];

        int sum = 0;

        for (int i = result.Length - 1; i >= 0; i--)
        {
            sum += input[i];
            result[i] = sum % 10;
        }

        return result;
    }
}
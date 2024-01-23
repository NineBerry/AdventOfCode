// #define Sample
using System.Text.RegularExpressions;
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day10\Sample.txt";
    int arrayLengthPart1 = 5;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day10\Full.txt";
    int arrayLengthPart1 = 256;
#endif

    string inputText = File.ReadAllText(fileName);

    int[] lengths = 
        Regex.Matches(inputText, "\\d+")
        .Select(m => int.Parse(m.Value))
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(lengths, arrayLengthPart1));
    Console.WriteLine("Part 2: " + Part2(inputText));
    Console.ReadLine();
}

int Part1(int[] lengths, int arrayLength)
{
    byte[] array = new byte[arrayLength];
    for(int i = 0;i < arrayLength; i++)
    {
        array[i] = (byte)i;
    }
    int currentPosition = 0;
    int skipSize = 0;

    foreach(int length in lengths) 
    {
        ReverseArray(array, currentPosition, length);

        currentPosition = currentPosition + length + skipSize;
        skipSize++;
    }

    return array[0] * array[1];
}

string Part2(string input)
{
    const int arrayLength = 256;
    
    byte[] lengths = input.Select(ch => (byte)ch).ToArray();
    lengths = [..lengths, 17, 31, 73, 47, 23];

    byte[] array = new byte[arrayLength];
    for (int i = 0; i < arrayLength; i++)
    {
        array[i] = (byte)i;
    }
    int currentPosition = 0;
    int skipSize = 0;

    foreach (var _ in Enumerable.Range(0, 64))
    {
        foreach (int length in lengths)
        {
            ReverseArray(array, currentPosition, length);

            currentPosition = currentPosition + length + skipSize;
            skipSize++;
        }
    }

    byte[] dense = new byte[16];

    for(int i=0; i < 16; i++)
    {
        dense[i] = array.Skip(i * 16).Take(16).Aggregate((a, b) => (byte)(a ^ b));
    }

    return Convert.ToHexString(dense);
}

void ReverseArray(byte[] array, int currentPosition, int length)
{
    for(int i = 0; i < length / 2; i++)
    {
        int lowerIndex = currentPosition + i;
        int upperIndex = currentPosition + length - 1 - i;

        byte temp = array[ReduceToArray(lowerIndex, array.Length)];
        array[ReduceToArray(lowerIndex, array.Length)] = array[ReduceToArray(upperIndex, array.Length)];
        array[ReduceToArray(upperIndex, array.Length)] = temp;
    }
}

int ReduceToArray(int inValue, int arrayLength)
{
    return inValue % arrayLength;
}
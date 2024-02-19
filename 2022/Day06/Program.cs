// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day06\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day06\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + FindMarker(input, 4));
    Console.WriteLine("Part 2: " + FindMarker(input, 14));
    Console.ReadLine();
}

long FindMarker(string input, int markerLength)
{
    for(int i=0; i < input.Length; i++)
    {
        string sub = input.Substring(i, markerLength);
        if (sub.Distinct().Count() == markerLength) return i + markerLength;
    }
    
    return 0;
}
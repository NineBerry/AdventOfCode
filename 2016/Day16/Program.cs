// #define Sample

using System.Text;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day16\Sample.txt";
    int diskLengthPart1 = 20;
    int diskLengthPart2 = 20;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day16\Full.txt";
    int diskLengthPart1 = 272;
    int diskLengthPart2 = 35_651_584;
#endif

    string initialState = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Solve(initialState, diskLengthPart1));
    Console.WriteLine("Part 2: " + Solve(initialState, diskLengthPart2));
    Console.ReadLine();
}

string Solve(string initialState, int diskLength)
{
    string fillString = GenerateString(initialState, diskLength);
    string checkSum = GenerateCheckSum(fillString);

    return checkSum;
}

string GenerateString(string text, int diskLength)
{
    StringBuilder sb = new StringBuilder(diskLength * 2); 
    sb.Append(text);

    while (sb.Length < diskLength)
    {
        char[] b = sb.ToString().Reverse().Select(ch => ch == '1' ? '0' : '1').ToArray();
        sb.Append('0');
        sb.Append(b);
    }

    return sb.ToString().Substring(0, diskLength);
}


string GenerateCheckSum(string text)
{
    while(text.Length % 2 == 0)
    {
        text = GenerateCheckSumStep(text);
    }

    return text;
}

string GenerateCheckSumStep(string from)
{
    StringBuilder sb = new StringBuilder(from.Length);

    for(int i=0; i < from.Length; i += 2)
    {
        char next = from[i] == from[i + 1] ? '1' : '0';
        sb.Append(next);
    }

    return sb.ToString();
}

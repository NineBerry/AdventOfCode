// #define Sample
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day09\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day09\Full.txt";
#endif

    string input = File.ReadAllText(fileName);
    
    Console.WriteLine("Part 1: " + Part1(input));
    // Console.WriteLine("Part 2: " + Part2(input));
    Console.WriteLine("Part 2: " + Part2Optimized(input));
    Console.ReadLine();
}

long Part1(string input)
{
    return Decompress(input, recursive: false).Length;
}
long Part2(string input)
{
    string result = Decompress(input, recursive: true);
    return result.Length;
}
BigInteger Part2Optimized(string input)
{
    return CountDecompressV2Size(input);
}

string Decompress(string input, bool recursive)
{
    StringBuilder sb = new StringBuilder();

    bool anyFound = false;
    bool inside = false;
    int i = 0;
    string token = "";

    while(i < input.Length)
    {
        char ch = input[i];

        if (inside)
        {
            if (ch == ')')
            {
                inside = false;
                HandleToken();
                anyFound = true;
                token = "";
            }
            else
            {
                token += ch;
            }
        }
        else
        {
            if(ch == '(')
            {
                inside = true;
                token = "";
            }
            else
            {
                sb.Append(ch);
            }
        }
        i++;
    }


    if (recursive && anyFound) return Decompress(sb.ToString(), recursive);

    return sb.ToString();  


    void HandleToken()
    {
        int[] numbers = Regex.Matches(token, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
        int length = numbers[0];
        int repeats = numbers[1];

        string text = input.Substring(i + 1, length);
        for(int j = 0; j < repeats; j++)
        {
            sb.Append(text);
        }

        i += length;
    }
}

BigInteger CountDecompressV2Size(string input)
{
    BigInteger result = 0;
    Markers markers = [];

    bool inside = false;
    int i = 0;
    string token = "";

    while (i < input.Length)
    {
        markers.DecreaseLengths();
        char ch = input[i];

        if (inside)
        {
            if (ch == ')')
            {
                inside = false;
                markers.Add(HandleToken());
                token = "";
            }
            else
            {
                token += ch;
            }
        }
        else
        {
            if (ch == '(')
            {
                inside = true;
                token = "";
            }
            else
            {
                result += markers.CombinedRepeats();
            }
        }
        i++;
    }


    return result;


    Marker HandleToken()
    {
        int[] numbers = Regex.Matches(token, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
        int length = numbers[0] + 1;
        int repeats = numbers[1];

        return new Marker { Length = length, Repeats = repeats };
    }
}

public record Marker
{
    public int Length;
    public int Repeats;
}

public class Markers: List<Marker>
{
    public void DecreaseLengths()
    {
        foreach(var marker in this)
        {
            marker.Length--;
        }

        RemoveAll(m => m.Length <= 0);
    }

    public BigInteger CombinedRepeats()
    {
        if(Count == 0) return BigInteger.One;
        
        return this.Aggregate(BigInteger.One, (a, b) => a * b.Repeats);
    }  
}
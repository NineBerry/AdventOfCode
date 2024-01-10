#define Sample
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
    Console.WriteLine("Part 2: " + Part2(input));
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


// #define Sample
using System.Text;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day05\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day05\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string input)
{
    return CollapseFully(input).Length;
}

long Part2(string input)
{
    int min = int.MaxValue;

    for (char ch = 'a'; ch <= 'z'; ch++)
    {
        string modified = input.Replace(ch.ToString(), "", StringComparison.OrdinalIgnoreCase);
        string collpased = CollapseFully(modified);

        min = Math.Min(min, collpased.Length);
    }

    return min;
}

string CollapseFully(string input)
{
    var temp = "";
    while (temp != input)
    {
        temp = input;
        input = Collapse(input);
    }
    return input;
}


string Collapse(string input)
{
    StringBuilder sb = new StringBuilder();

    for(int i=0; i < input.Length; i++)
    {
        char ch1 = input[i];
        char ch2 = i < input.Length - 1 ? input[i + 1] : '\0';

        if(IsPolarized(ch1, ch2))
        {
            i++;
        }
        else
        {
            sb.Append(ch1);
        }
    }

    return sb.ToString();
}

bool IsPolarized(char ch1, char ch2)
{
    return ch1 switch
    {
        >= 'a' and <= 'z' when ch2 == char.ToUpper(ch1) => true,
        >= 'A' and <= 'Z' when ch2 == char.ToLower(ch1) => true,
        _ => false,
    };
}
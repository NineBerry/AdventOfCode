using System.Text.RegularExpressions;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day04\Full.txt";

    int[] values = Regex.Matches(File.ReadAllText(fileName), "\\d+").Select(m => int.Parse(m.Value)).ToArray();

    Console.WriteLine("Part 1: " + Part1(values[0], values[1]));
    Console.WriteLine("Part 2: " + Part2(values[0], values[1]));
    Console.ReadLine();
}

int Part1(int from, int to)
{
    return 
        Enumerable.Range(from, to - from + 1)
        .Count(i => ValidPasswordPart1(i.ToString()));
}

int Part2(int from, int to)
{
    return
        Enumerable.Range(from, to - from + 1)
        .Count(i => ValidPasswordPart2(i.ToString()));
}

bool ValidPasswordPart1(string password)
{
    return         
        password.Zip(password.Substring(1)).All(pair => pair.Second >= pair.First)
        && password.Zip(password.Substring(1)).Any(pair => pair.Second == pair.First);
}

bool ValidPasswordPart2(string password)
{
    bool valid = 
        password.Zip(password.Substring(1)).All(pair => pair.Second >= pair.First)
        && password.Zip(password.Substring(1)).Any(pair => pair.Second == pair.First);

    if (valid)
    {
        List<int> groups = [];

        char previous = '\0';
        int count = 0;
        foreach (char c in password)
        {
            if(c == previous)
            {
                count++;
            }
            else
            {
                groups.Add(count);
                previous = c;
                count = 1;
            }
        }
        groups.Add(count);
        valid = groups.Contains(2);
    }

    return valid;
}
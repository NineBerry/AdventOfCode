// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day02\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day02\Full.txt";
#endif

    PasswordWithRules[] passwords = File.ReadAllLines(fileName).Select(s => new PasswordWithRules(s)).ToArray();

    Console.WriteLine("Part 1: " + Part1(passwords));
    Console.WriteLine("Part 2: " + Part2(passwords));
    Console.ReadLine();
}

long Part1(PasswordWithRules[] passwords)
{
    return passwords.Count(p => p.IsValidV1());
}

long Part2(PasswordWithRules[] passwords)
{
    return passwords.Count(p => p.IsValidV2());
}

record PasswordWithRules
{
    public PasswordWithRules(string line)
    {
        var groups = Regex.Match(line, @"(\d+)-(\d+) (.): (.*)").Groups;
        
        Min = int.Parse(groups[1].Value);
        Max = int.Parse(groups[2].Value);
        RuleLetter = groups[3].Value[0];
        Password = groups[4].Value;
    }

    public int Min;
    public int Max;
    public char RuleLetter;
    
    public string Password;

    public bool IsValidV1()
    {
        int count = Password.Count(ch => ch == RuleLetter);
        return count >= Min && count <= Max;
    }
    public bool IsValidV2()
    {
        return (Password[Min - 1] == RuleLetter) ^ (Password[Max - 1] == RuleLetter);
    }
}
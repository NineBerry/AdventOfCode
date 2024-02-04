// #define Sample

using System.Security.Cryptography;
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day04\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day04\Full.txt";
#endif

    var passports = File.ReadAllText(fileName).ReplaceLineEndings("\n").Split("\n\n").Select(s => new Passport(s)).ToArray();

    Console.WriteLine("Part 1: " + Part1(passports));
    Console.WriteLine("Part 2: " + Part2(passports));
    Console.ReadLine();
}

long Part1(Passport[] passports)
{
    return passports.Count(p => p.IsValid());
}

long Part2(Passport[] passports)
{
    return passports.Count(p => p.IsValid() && p.IsEvenMoreValid());
}

public class Passport
{
    public Passport(string input)
    {
        var data = Regex.Matches(input, "[a-z]{3}:[#a-z0-9]+").Select(m => m.Value).Select(s => s.Split(":"));

        foreach(var property in data)
        {
            Data.Add(property[0], property[1]);
        }
    }

    public bool IsValid()
    {
        string[] required = ["byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"];
        
        return required.All(Data.ContainsKey);
    }

    public bool IsEvenMoreValid()
    {
        return 
            CheckBirthYear() && 
            CheckIssueYear() && 
            CheckExpirationYear() && 
            CheckHeight() && 
            CheckHairColor() && 
            CheckEyeColor() && 
            CheckPassportID();
    }

    private bool CheckPassportID()
    {
        return CheckAgainstRegularExpression(Data["pid"], "^[0-9]{9}$");
    }

    private bool CheckEyeColor()
    {
        return CheckAgainstRegularExpression(Data["ecl"], "^(amb|blu|brn|gry|grn|hzl|oth)$");
    }

    private bool CheckHairColor()
    {
        return CheckAgainstRegularExpression(Data["hcl"], "^#[a-f0-9]{6}$");
    }

    private bool CheckHeight()
    {
       string height = Data["hgt"];

        if (height.EndsWith("cm")) return CheckInteger(height.Substring(0, height.Length-2), 150, 193);
        if (height.EndsWith("in")) return CheckInteger(height.Substring(0, height.Length - 2), 59, 76);

        return false;        
    }

    private bool CheckExpirationYear()
    {
        return CheckInteger(Data["eyr"], 2020, 2030);
    }

    private bool CheckIssueYear()
    {
        return CheckInteger(Data["iyr"], 2010, 2020);
    }

    private bool CheckBirthYear()
    {
        return CheckInteger(Data["byr"], 1920, 2002);
    }

    private static bool CheckAgainstRegularExpression(string value, string regEx)
    {
        return Regex.IsMatch(value, regEx);
    }
    private static bool CheckInteger(string value, int min, int max)
    {
        return int.TryParse(value, out int number) && number >= min && number <= max;
    }

    private Dictionary<string, string> Data = [];
}
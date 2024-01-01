// #define Sample
using System.Text;
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day04\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day04\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    var rooms = input.Select(ParseLine);

    Console.WriteLine("Part 1: " + Part1(rooms));
    Console.WriteLine("Part 2: " + Part2(rooms));
    Console.ReadLine();
}

(string Name, int SectorID, string Checksum) ParseLine(string line)
{
    Match match = Regex.Match(line, "(.*)-([0-9]+)\\[(.*)\\]");
    return (match.Groups[1].Value, int.Parse(match.Groups[2].Value), match.Groups[3].Value);
}

long Part1(IEnumerable<(string Name, int SectorID, string Checksum)> rooms)
{
    return rooms.Where(r => CalculateChecksum(r.Name) == r.Checksum).Sum(r => r.SectorID);
}

string CalculateChecksum(string name)
{
    string checksum = string.Join("", 
        name
        .Where(ch => ch != '-')
        .GroupBy(ch => ch)
        .OrderByDescending(group => group.Count())
        .ThenBy(group => group.Key)
        .Select(group => group.Key)
        .Take(5));

    // Console.WriteLine(checksum);
    return checksum;
}

long Part2(IEnumerable<(string Name, int SectorID, string Checksum)> rooms)
{
    var room =  rooms
        .Where(r => CalculateChecksum(r.Name) == r.Checksum)
        .Select(r => (Name: DecryptName(r.Name, r.SectorID), SectorID: r.SectorID))
        .Where(r => r.Name == "northpole object storage")
        .Single();

    return room.SectorID;
}

string DecryptName(string name, int shift)
{
    shift = (shift % 26);

    string decrypted = string.Join("", name.Select(ch => ch == '-' ? ' ' : Rot(ch, shift)));

    return decrypted;
}

char Rot(char ch, int shift)
{
    return (char)('a' + (ch - 'a' + shift) % 26);
}
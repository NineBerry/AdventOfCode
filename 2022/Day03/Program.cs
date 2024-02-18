#define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day03\Full.txt";
#endif

    string[] rucksacks = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(rucksacks));
    Console.WriteLine("Part 2: " + Part2(rucksacks));
    Console.ReadLine();
}

long Part1(string[] rucksacks)
{
    return rucksacks.Sum(GetPriorityForRucksack);
}

long Part2(string[] rucksacks)
{
    long sum = 0;

    for (int i = 0; i < rucksacks.Length; i+=3)
    {
        sum += GetPriorityForTeam(rucksacks[i], rucksacks[i + 1], rucksacks[i + 2]);
    }

    return sum;
}


int GetPriorityForTeam(string rucksack1, string rucksack2, string rucksack3)
{
    char commonChar = rucksack1.Intersect(rucksack2).Intersect(rucksack3).Single();
    return GetPriorityForChar(commonChar);
}


int GetPriorityForRucksack(string line)
{
    int compartmentSize = line.Length / 2;
    string first = line.Substring(0, compartmentSize);
    string second = line.Substring(compartmentSize);

    char commonChar = first.Intersect(second).Single();
    return GetPriorityForChar(commonChar);
}

int GetPriorityForChar(char commonChar)
{
    return commonChar switch
    {
        >= 'a' and <= 'z' => (commonChar - 'a') + 1,
        >= 'A' and <= 'Z' => (commonChar - 'A') + 1 + 26,
        _ => throw new ApplicationException("invalid char")

    };
}
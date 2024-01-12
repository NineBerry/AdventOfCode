// #define Sample

{

#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2018\Day02\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2018\Day02\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2018\Day02\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2018\Day02\Full.txt";
#endif

    string[] ids = File.ReadAllLines(fileNamePart1);
    Console.WriteLine("Part 1: " + Part1(ids));

    ids = File.ReadAllLines(fileNamePart2);
    Console.WriteLine("Part 2: " + Part2(ids));
    Console.ReadLine();
}

long Part1(string[] ids)
{
    int count1 = ids.Count(HasTwiceLetter);
    int count2 = ids.Count(HasThriceLetter);

    return count1 * count2;
}

string Part2(string[] ids)
{
    for(int i = 0; i < ids.Length - 1; i++)
    {
        for (int j = i + 1; j < ids.Length - 1; j++)
        {
            if (CountDifferences(ids[i] , ids[j]) == 1)
            {
                return GetCommonCharacters(ids[i], ids[j]);
            }
        }
    }

    return "";
}


int CountDifferences(string s1, string s2)
{
    return 
        s1
        .Zip(s2)
        .Count(pair => pair.First != pair.Second);
}

string GetCommonCharacters(string s1, string s2)
{
    return string.Join("", 
        s1
        .Zip(s2)
        .Where(pair => pair.First == pair.Second)
        .Select(pair => pair.First));
}

bool HasTwiceLetter(string id)
{
    return id.GroupBy(ch => ch).Where(group => group.Count() == 2).Any();
}

bool HasThriceLetter(string id)
{
    return id.GroupBy(ch => ch).Where(group => group.Count() == 3).Any();
}

//#define Sample2
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day05\Sample.txt";
#elif  Sample2
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day05\Sample2.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day05\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string[] input)
{
    return input.Count(IsNice1);
}

long Part2(string[] input)
{
    return input.Count(IsNice2);
}


bool IsNice1(string word)
{
    HashSet<char> vowels = ['a', 'e', 'i', 'o', 'u',]; ;
    int vowelsSeen = 0;
    bool doubleSeen = false;
    HashSet<string> forbidden = ["ab", "cd", "pq", "xy"]; ;
    bool forbiddenSeen = false;

    for(int i=0; i < word.Length - 1; i++)
    {
        char ch = word[i];
        char ch2 = word[i+1];

        HandleSingleChar(ch);
        HandleDoubleChar(ch, ch2);
    }
    HandleSingleChar(word.Last());

    void HandleSingleChar(char ch)
    {
        if(vowels.Contains(ch)) vowelsSeen++;
    }

    void HandleDoubleChar(char ch1, char ch2)
    {
        doubleSeen |= (ch1 == ch2);
        forbiddenSeen |= forbidden.Contains("" + ch1 + "" + ch2);
    }

    return (vowelsSeen >= 3) && doubleSeen && !forbiddenSeen;
}

bool IsNice2(string word)
{
    bool letterTripleSeen = false;

    for (int i = 0; i < word.Length - 2; i++)
    {
        if (word[i] == word[i + 2])
        {
            letterTripleSeen = true;
            break;
        }
    }

    bool letterPairPairSeen = false;
    if (letterTripleSeen) 
    {
        Dictionary<string, int> pairSeenAt = new();

        for (int i = 0; i < word.Length - 1; i++)
        {
            string s = "" + word[i] + "" + word[i + 1];

            if (pairSeenAt.TryGetValue(s, out int seenAt))
            {
                if((i - seenAt) > 1)
                {
                    letterPairPairSeen = true;
                    break;
                }
            }
            else
            {
                pairSeenAt[s] = i;
            }
        }
    }

    return letterTripleSeen && letterPairPairSeen;
}
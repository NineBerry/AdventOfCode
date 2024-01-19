// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day09\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day09\Full.txt";
#endif

    (int groupScore, int garbageCount) = ParseGroups(File.ReadAllText(fileName));

    Console.WriteLine("Part 1: " + groupScore);
    Console.WriteLine("Part 2: " + garbageCount);
    Console.ReadLine();
}

(int GroupScore, int GarbageCount) ParseGroups(string input)
{
    int score = 0;
    int garbageCount = 0;

    bool inGarbage = false;
    bool afterExclamation = false;
    int inGroup = 0;

    foreach (char ch in input)
    {
        bool countGarbage = inGarbage;

        if (afterExclamation)
        {
            afterExclamation = false;
            continue;
        }

        switch (ch)
        {
            case '<':
                if (!inGarbage)
                {
                    inGarbage = true;
                    countGarbage = false;
                }
                break;
            case '>':
                if (inGarbage)
                {
                    inGarbage = false;
                    countGarbage = false;
                }
                break;
            case '!':
                if (inGarbage)
                {
                    afterExclamation = true;
                    countGarbage = false;
                }
                break;
            case '{':
                if (!inGarbage) inGroup++;
                break;
            case '}':
                if (!inGarbage)
                {
                    score += inGroup;
                    inGroup--;
                }
                break;
        }

        if (countGarbage)
        {
            garbageCount++;
        }
    }

    return (score, garbageCount);
}



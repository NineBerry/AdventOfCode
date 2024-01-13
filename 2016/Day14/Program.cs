// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day14\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day14\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}


long Part1(string secretKey)
{
    return Solve(secretKey, 1);
}

long Part2(string secretKey)
{
    return Solve(secretKey, 2016 + 1);
}

long Solve(string secretKey, int hashCount)
{
    List<int> passwords = new();

    Queue<(int Index, char? FirstTripleCharacter, HashSet<char> FiverCharacters)> queue = [];
    queue.EnsureCapacity(2000);
    int addIndex = 0;

    foreach (int i in Enumerable.Range(1, 1500))
    {
        GrowQueue();
    }

    while (passwords.Count < 64)
    {
        var current = queue.Dequeue();

        if (current.FirstTripleCharacter.HasValue)
        {
            if (queue.Take(1000).Any(e => e.FiverCharacters.Contains(current.FirstTripleCharacter.Value)))
            {
                passwords.Add(current.Index);
            }
        }

        GrowQueue();
    }

    return passwords[63];

    void GrowQueue()
    {
        string md5 = CreateMD5(secretKey + addIndex, hashCount);
        (var firstTripleCharacter, var fiverCharacters) = GetStringStats(md5);
        queue.Enqueue((addIndex, firstTripleCharacter, fiverCharacters));
        addIndex++;
    }
}


(char? FirstTripleCharacter, HashSet<char> FiverCharacters) GetStringStats(string s)
{
    char? firstTripleCharacter = null;
    HashSet<char> fiverCharacters = [];

    char previous = '\0';
    int charCounter = 0;

    foreach (char ch in s + "|") 
    {
        if(ch == previous)
        {
            charCounter++;
        }
        else
        {
            charCounter = 1;
            previous = ch;
        }

        Check();
    }

    Check();

    return (FirstTripleCharacter: firstTripleCharacter, FiverCharacters: fiverCharacters);

    void Check()
    {
        if (charCounter == 3 && !firstTripleCharacter.HasValue)
        {
            firstTripleCharacter = previous;
        }
        
        if (charCounter == 5)
        {
            fiverCharacters.Add(previous);
        }
    }
}

string CreateMD5(string input, int count)
{
    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
    {
        foreach (int i in Enumerable.Range(1, count))
        {

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            input = Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
        return input;
    }
}
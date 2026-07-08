// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day03\Full.txt";
#endif

    string[] passwords = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(passwords));
    Console.WriteLine("Part 2: " + Part2(passwords));
    Console.WriteLine("Part 3: " + Part3(passwords));

    Console.ReadLine();
}

string Part1(string[] passwords)
{
    var passwordScores = passwords.Select(p => (p, ScorePassword(p, 1))).ToDictionary(x => x.p, x => x.Item2);
    long maxScore = passwordScores.Values.Max();
    return passwordScores.First(p => p.Value == maxScore).Key;
}

string Part2(string[] passwords)
{
    var passwordScores = passwords.Select(p => (p, ScorePassword(p, 2))).ToDictionary(x => x.p, x => x.Item2);
    long maxScore = passwordScores.Values.Max();
    return passwordScores.First(p => p.Value == maxScore).Key;
}

long Part3(string[] passwords)
{
    long best = 0;

    for (var ch = 'a'; ch <= 'z'; ch++)
    {
        best = Math.Max(best, TryCharacter(ch));
        best = Math.Max(best, TryCharacter(char.ToUpper(ch)));
    }

    for (var ch = '0'; ch <= '9'; ch++)
    {
        best = Math.Max(best, TryCharacter(ch));
    }

    return best;

    long TryCharacter(char character)
    {
        return passwords.Sum(password => ScorePassword(password + character, 2));
    }
}

long ScorePassword(string password, int level)
{
    int score = 0;  

    if(HasLowerCase(password))
    {
        score += 1;
    }

    if (HasUpperCase(password))
    {
        score += 1;
    }

    if (HasDigit(password))
    {
        score += 1;
    }

    if(level >= 2)
    {
        if (HasOnlySeven(password))
        {
            score += 7;
        }

        if(HasSequence(password, out int sequenceLength))
        {
            score += (sequenceLength * sequenceLength);
        }

        if (HasRGB(password))
        {
            score *= 3;
        }

    }

    return score * password.Length;
}

bool HasLowerCase(string value)
{
    return value.Any(ch => ch >= 'a' && ch <= 'z');
}

bool HasUpperCase(string value)
{
    return value.Any(ch => ch >= 'A' && ch <= 'Z');
}

bool HasDigit(string value)
{
    return value.Any(ch => ch >= '0' && ch <= '9');
}

bool HasOnlySeven(string value)
{
    return value.Contains('7') && !value.Any(ch => ch >= '0' && ch <= '9' && ch != '7');
}


bool HasSequence(string value, out int longestSequenceLength)
{
    longestSequenceLength = 0;

    int currentSequenceLength = 1;
    char lastChar = '\0';

    foreach(var ch in value)
    {
        if(ch == lastChar)
        {
            currentSequenceLength++;
        }
        else
        {
            lastChar = ch;

            if (currentSequenceLength >= longestSequenceLength) longestSequenceLength = currentSequenceLength;
            currentSequenceLength = 1;
        }
    }

    if (currentSequenceLength >= longestSequenceLength) longestSequenceLength = currentSequenceLength;

    return longestSequenceLength >= 3;
}

bool HasRGB(string value)
{
    return value.Contains("red") || value.Contains("green") || value.Contains("blue");
}

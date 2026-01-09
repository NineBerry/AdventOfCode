// #define Sample


{
#if Sample
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day01\Full.txt";
#endif

    string[] bananas = File.ReadAllLines(fileName);
    Console.WriteLine("Part 1: " + Part1(bananas));
    Console.WriteLine("Part 2: " + Part2(bananas));
    Console.WriteLine("Part 3: " + Part3(bananas));

    Console.ReadLine();
}

long Part1(string[] bananas)
{
    return bananas.Sum(Score);
}

long Part2(string[] bananas)
{
    return bananas.Sum(ScoreIfEven);
}
long Part3(string[] bananas)
{
    return bananas.Sum(ScoreIfNotDeprecated);
}


long Score(string banana)
{
    return banana.Count(ch => ch == 'b' || ch == 'n');
}

long ScoreIfEven(string banana)
{
    long score = Score(banana);
    return long.IsEvenInteger(score) ? score : 0;
}

long ScoreIfNotDeprecated(string banana)
{
    if(banana.Contains('e')) return 0;
    return Score(banana);
}

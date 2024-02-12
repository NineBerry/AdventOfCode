// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day04\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day04\Full.txt";
#endif

    string input = File.ReadAllText(fileName);
    BingoGame game = new BingoGame(input);
    var wins = game.PlayGame();

    Console.WriteLine("Part 1: " + Part1(wins));
    Console.WriteLine("Part 2: " + Part2(wins));

    Console.ReadLine();
}

long Part1(List<(int DrawnNumber, int SumUnmarkedNumbers)> wins)
{
    return wins.First().DrawnNumber * wins.First().SumUnmarkedNumbers;
}

long Part2(List<(int DrawnNumber, int SumUnmarkedNumbers)> wins)
{
    return wins.Last().DrawnNumber * wins.Last().SumUnmarkedNumbers;
}

public class BingoGame
{
    int[] DrawnNumbers;
    BingoCard[] BingoCards;

    public BingoGame(string input)
    {
        var parts = input.ReplaceLineEndings("\n").Split("\n\n");
        DrawnNumbers = parts.First().Split(',').Select(int.Parse).ToArray();
        BingoCards = parts.Skip(1).Select(s => new BingoCard(s)).ToArray();
    }

    public List<(int DrawnNumber, int SumUnmarkedNumbers)> PlayGame()
    {
        List<(int DrawnNumber, int SumUnmarkedNumbers)> result = [];

        foreach (var number in DrawnNumbers)
        {
            foreach(var card in BingoCards) 
            {
                if (card.Won) continue;

                if (card.CallNumber(number))
                {
                    result.Add((number, card.SumUnmarkedNumbers()));
                }
            }
        }

        return result;
    }
}

public class BingoCard
{
    int[][] Values;
    HashSet<int> DrawnNumbers = [];

    public BingoCard(string input)
    {
        Values = 
            input
            .Split("\n")
            .Select(s => 
                s.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
            .ToArray();
    }

    public bool CallNumber(int number)
    {
        DrawnNumbers.Add(number);
        return IsBingo();
    }

    public int SumUnmarkedNumbers()
    {
        return Values.SelectMany(v => v).Where(v => !DrawnNumbers.Contains(v)).Sum();
    }

    private bool IsBingo()
    {
        for(int i=0; i < Values.Length; i++)
        {
            if(Values[i].All(v => DrawnNumbers.Contains(v)) ||
                Values.All(v => DrawnNumbers.Contains(v[i])))
            {
                Won = true;
                return true;
            }
        }

        return false;
    }

    public bool Won = false;
}
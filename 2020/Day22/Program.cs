// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day22\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day22\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    int[] player1 = GetPlayerCards(1, input);
    int[] player2 = GetPlayerCards(2, input);

    GameState gameState = new GameState(player1, player2);

    Console.WriteLine("Part 1: " + Part1(gameState));
    Console.WriteLine("Part 2: " + Part2(gameState));
    Console.ReadLine();
}

int[] GetPlayerCards(int player, string[] lines)
{
    string playerStartLine = $"Player {player}:";
    return 
        lines
        .SkipWhile(s => s!=playerStartLine)
        .Skip(1)
        .TakeWhile(s => s != "")
        .Select(int.Parse)
        .ToArray();   
}

long Part1(GameState gameState)
{
    gameState = PlayCombat(gameState);
    return CalculateScore(gameState);
};

long Part2(GameState gameState)
{
    (gameState, _) = PlayRecursiveCombat(gameState);
    return CalculateScore(gameState);
};

long CalculateScore(GameState gameState)
{
    int[] values = [.. gameState.Player1, .. gameState.Player2];
    return values.Reverse().Select((value, index) => value * (index + 1)).Sum();
}


GameState PlayCombat(GameState originalGameState)
{
    GameState gameState = originalGameState.Clone();

    while (gameState.Player1.Any() && gameState.Player2.Any())
    {
        int player1Draw = gameState.Player1.Dequeue();
        int player2Draw = gameState.Player2.Dequeue();

        var toAdd = player1Draw > player2Draw ? gameState.Player1 : gameState.Player2;
        toAdd.Enqueue(Math.Max(player1Draw, player2Draw));
        toAdd.Enqueue(Math.Min(player1Draw, player2Draw));
    }

    return gameState;
};




(GameState GameState, int Player) PlayRecursiveCombat(GameState originalGameState)
{
    var originalGameStateAsString = originalGameState.ToString();
    if (GameState.GlobalSeen.TryGetValue(originalGameStateAsString, out var cachedResult))
    {
       return cachedResult;
    }
    
    HashSet<string> seenGameStates = [];

    GameState gameState = originalGameState.Clone();

    while (gameState.Player1.Any() && gameState.Player2.Any())
    {
        var gameStateAsString = gameState.ToString();
        if (seenGameStates.Contains(gameStateAsString))
        {
            GameState.GlobalSeen.Add(originalGameStateAsString, (gameState, 1));
            return (gameState, 1);
        }
        seenGameStates.Add(gameStateAsString);

        int player1Draw = gameState.Player1.Dequeue();
        int player2Draw = gameState.Player2.Dequeue();

        int drawWinner;

        if (player1Draw <= gameState.Player1.Count && player2Draw <= gameState.Player2.Count)
        {
            // recurse
            GameState subGameState = new GameState(
                gameState.Player1.Take(player1Draw).ToArray(),
                gameState.Player2.Take(player2Draw).ToArray());

            (_, drawWinner) = PlayRecursiveCombat(subGameState);
        }
        else
        {
            drawWinner = player1Draw > player2Draw ? 1 : 2;
        }

        var toAdd = (drawWinner == 1) ? gameState.Player1 : gameState.Player2;
        
        toAdd.Enqueue((drawWinner == 1) ? player1Draw : player2Draw);
        toAdd.Enqueue((drawWinner == 1) ? player2Draw : player1Draw);
    }

    int gameWinner = gameState.Player1.Any() ? 1 : 2;
    GameState.GlobalSeen.Add(originalGameStateAsString, (gameState, gameWinner));
    return (gameState, gameWinner);
};



public class GameState
{
    public GameState(int[] player1, int[] player2)
    {
        Player1 = new Queue<int>(player1);
        Player2 = new Queue<int>(player2);
    }

    public GameState Clone()
    {
        return new GameState(Player1.ToArray(), Player2.ToArray());
    }

    public Queue<int> Player1;
    public Queue<int> Player2;

    public static Dictionary<string, (GameState, int)> GlobalSeen = [];

    public override string ToString()
    {
        return string.Join(",", Player1) + "|" + string.Join(",", Player2);
    }
}
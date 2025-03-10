// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day21\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day21\Full.txt";
#endif

    string input = File.ReadAllText(fileName);
    var numbers = input.ExtractNumbers();
    var player1Start = numbers[1];
    var player2Start = numbers[3];

    Console.WriteLine("Part 1: " + Part1(player1Start, player2Start));
    Console.WriteLine("Part 2: " + Part2(player1Start, player2Start));
    Console.ReadLine();
}


long Part1(int player1Start, int player2Start)
{
    return DeterministicGame.Play(player1Start, player2Start);
}
long Part2(int player1Start, int player2Start)
{
    return DiracGame.Play(player1Start, player2Start);
}


static class DeterministicGame
{
    public static long Play(int playerStart1, int playerStart2)
    {
        int rolls = 0;
        bool player1Next = true;

        Player player1 = new Player { Position = playerStart1, Score = 0 };
        Player player2 = new Player { Position = playerStart2, Score = 0 };

        while(player1.Score < 1000 && player2.Score < 1000)
        {
            Player currentPlayer = player1Next ? player1 : player2;
            player1Next = !player1Next;

            Roll(currentPlayer, ref rolls);
        }


        return rolls * Math.Min(player1.Score, player2.Score);
    }

    private static void Roll(Player currentPlayer, ref int rolls)
    {
        int steps = ++rolls + ++rolls + ++rolls;
        currentPlayer.Move(steps);
    }

    class Player
    {
        public int Position;
        public int Score;

        internal void Move(int steps)
        {
            Position = (Position + steps) % 10;
            if (Position == 0) Position = 10;
            Score += Position;
        }
    }
}


static class DiracGame
{
    public static long Play(int playerStart1, int playerStart2)
    {
        (int Count, int Steps)[] dimensions = 
        [
            (1, 3),
            (3, 4),
            (6, 5),
            (7, 6),
            (6, 7),
            (3, 8),
            (1, 9)
        ];

        long player1Won = 0;
        long player2Won = 0;

        Player player1 = new Player { Position = playerStart1, Score = 0 };
        Player player2 = new Player { Position = playerStart2, Score = 0 };


        PlayGame(true, player1, player2, 1);

        return Math.Max(player1Won, player2Won);


        void PlayGame(bool player1Next, Player player1, Player player2, long universes)
        {
            if(player1.Score >= 21)
            {
                player1Won += universes;
                return;
            }
            
            if (player2.Score >= 21)
            {
                player2Won += universes;
                return;
            }

            foreach(var dimension in dimensions)
            {
                if (player1Next)
                {
                    PlayGame(false, player1.Move(dimension.Steps), player2, universes * dimension.Count);
                }
                else
                {
                    PlayGame(true, player1, player2.Move(dimension.Steps), universes * dimension.Count);
                }
            }
        }
    }

    class Player
    {
        public int Position;
        public int Score;

        internal Player Move(int steps)
        {
            int newPosition = (Position + steps) % 10;
            if (newPosition == 0) newPosition = 10;
            int newScore = Score + newPosition;

            return new Player { Position = newPosition, Score = newScore };
        }
    }
}

public static class Tools
{
    public static int[] ExtractNumbers(this string input)
    {
        return Regex.Matches(input, @"-?\d+").Select(m => int.Parse(m.Value)).ToArray();
    }
}
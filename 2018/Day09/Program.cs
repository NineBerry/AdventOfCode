// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day09\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day09\Full.txt";
#endif

    int[] values = 
        Regex.Matches(File.ReadAllText(fileName), "\\d+").
        Select(m => int.Parse(m.Value))
        .ToArray();

    int countPlayers = values[0];
    int highestMarble = values[1];

    Console.WriteLine("Part 1: " + Solve(countPlayers, highestMarble));
    Console.WriteLine("Part 2: " + Solve(countPlayers, highestMarble * 100));
    Console.ReadLine();
}

long Solve(int countPlayers, int highestMarble)
{
    long[] playerScore = new long[countPlayers + 1];
    
    LinkedList<int> marblesCircle = [];
    var currentMarbleInCircle = marblesCircle.AddFirst(0);

    int currentPlayer = 1;
    int currentMarbleToPlace = 1;

    while(currentMarbleToPlace <= highestMarble)
    {
        if(currentMarbleToPlace % 23 == 0)
        {
            playerScore[currentPlayer] += currentMarbleToPlace;
            var toRemove = GetNextCounterClockWiseCount(marblesCircle, currentMarbleInCircle, 7);
            currentMarbleInCircle = GetNextClockWise(marblesCircle, toRemove);

            playerScore[currentPlayer] += toRemove.Value;
            marblesCircle.Remove(toRemove);
        }
        else
        {
            var insertAfter = GetNextClockWise(marblesCircle, currentMarbleInCircle);
            currentMarbleInCircle = marblesCircle.AddAfter(insertAfter, currentMarbleToPlace);
        }

        currentMarbleToPlace++;
        currentPlayer++;
        if (currentPlayer > countPlayers) currentPlayer = 1;
    }

    return playerScore.Max();
}

LinkedListNode<int> GetNextClockWise(LinkedList<int> elves, LinkedListNode<int> current)
{
    return current.Next ?? elves.First!;
}

LinkedListNode<int> GetNextCounterClockWiseCount(LinkedList<int> elves, LinkedListNode<int> current, int count)
{
    foreach(var _ in Enumerable.Range(1, count))
    {
        current = GetNextCounterClockWise(elves, current);
    }

    return current; 
}

LinkedListNode<int> GetNextCounterClockWise(LinkedList<int> elves, LinkedListNode<int> current)
{
    return current.Previous?? elves.Last!;
}
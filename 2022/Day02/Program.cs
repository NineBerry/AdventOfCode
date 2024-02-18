// #define Sample

using System.Runtime.CompilerServices;
using Round = (char First, char Second);

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day02\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day02\Full.txt";
#endif

    Round[] rounds = 
        File.ReadAllLines(fileName)
        .Select(s => new Round(s[0], s[2]))
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(rounds));
    Console.WriteLine("Part 2: " + Part2(rounds));
    Console.ReadLine();
}

long Part1(Round[] rounds)
{
    return rounds.Sum(PlayRoundPart1);
}

long Part2(Round[] rounds)
{
    return rounds.Sum(PlayRoundPart2);
}

int PlayRoundPart1(Round round)
{
    return PlayRound(round.Second.ToHand(), round.First.ToHand());
}

int PlayRoundPart2(Round round)
{
    Hand elf = round.First.ToHand();
    RoundResult expectedResult = round.Second.ToRoundResult();
    Hand mine = elf.GetHandForResult(expectedResult);

    return PlayRound(mine, elf);
}

int PlayRound(Hand mine, Hand elf)
{
    return mine.GetScoreForHand() + mine.GetScoreForGame(elf);
}

public enum Hand
{
    Rock,
    Paper,
    Scissor
}

public enum RoundResult
{
    Win,
    Draw,
    Loss
}

public static class Extensions
{
    public static int GetScoreForHand(this Hand hand)
    {
        return hand switch
        {
            Hand.Rock => 1,
            Hand.Paper => 2,
            Hand.Scissor => 3,
            _ => throw new ArgumentException("Unknown Hand")
        };
    }

    public static Hand ToHand(this char ch)
    {
        return ch switch
        {
            'A' or 'X' => Hand.Rock,
            'B' or 'Y' => Hand.Paper,
            'C' or 'Z' => Hand.Scissor,
            _ => throw new ArgumentException("Unknown Hand")
        };
    }

    public static RoundResult ToRoundResult(this char ch)
    {
        return ch switch
        {
            'X' => RoundResult.Loss,
            'Y' => RoundResult.Draw,
            'Z' => RoundResult.Win,
            _ => throw new ArgumentException("Unknown Round Result")
        };
    }

    public static int GetScoreForGame(this Hand mine, Hand other)
    {
        return mine.GetRoundResult(other) switch
        {
            RoundResult.Loss => 0,
            RoundResult.Draw => 3,
            RoundResult.Win => 6,
            _ => throw new ArgumentException("Unknown Result")
        };
    }

    public static RoundResult GetRoundResult(this Hand mine, Hand other)
    {
        if(mine == other) return RoundResult.Draw;

        return (mine, other) switch
        {
            (Hand.Rock, Hand.Paper) => RoundResult.Loss,
            (Hand.Rock, Hand.Scissor) => RoundResult.Win,
            (Hand.Paper, Hand.Scissor) => RoundResult.Loss,
            (Hand.Paper, Hand.Rock) => RoundResult.Win,
            (Hand.Scissor, Hand.Rock) => RoundResult.Loss,
            (Hand.Scissor, Hand.Paper) => RoundResult.Win,
            _ => throw new ArgumentException("Unknown hand combination")
        };
    }

    public static Hand GetHandForResult(this Hand elf, RoundResult expectedResult)
    {
        if (expectedResult == RoundResult.Draw) return elf;

        return (elf, expectedResult) switch
        {
            (Hand.Scissor, RoundResult.Win) => Hand.Rock,
            (Hand.Scissor, RoundResult.Loss) => Hand.Paper,
            (Hand.Rock, RoundResult.Win) => Hand.Paper,
            (Hand.Rock, RoundResult.Loss) => Hand.Scissor,
            (Hand.Paper, RoundResult.Win) => Hand.Scissor,
            (Hand.Paper, RoundResult.Loss) => Hand.Rock,
            _ => throw new ApplicationException("Unknown combination")
        };
    }
}
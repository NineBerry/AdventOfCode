// #define Sample

using System.Numerics;
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day22\Sample.txt";
    int deckSize = 10;
    int cardToFind = 5;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day22\Full.txt";
    int deckSize = 10_007;
    int cardToFind = 2019;
#endif

    string[] lines = File.ReadAllLines(fileName);
    Deck deck = new Deck(deckSize);

    Console.WriteLine("Part 1: " + Part1(deck, lines, cardToFind));
    // Console.WriteLine("Part 2: " + Part2(ruleset));
    Console.ReadLine();
}


int Part1(Deck deck, string[] commands, int cardToFind)
{
    foreach (string cmd in commands)
    {
        deck.Shuffle(cmd);
    }

    return deck.GetCardPosition(cardToFind);
}

public class Deck
{
    private BigInteger[] deck;

    public Deck(int size)
    {
        deck = Enumerable.Range(0, size).ToArray();
    }

    public int GetCardPosition(int cardToFind)
    {
        return Array.IndexOf(deck, cardToFind);
    }

    public void Shuffle(string cmd)
    {
        if(cmd.StartsWith("deal into"))
        {
            ShuffleReverse();
        }
        else if(cmd.StartsWith("deal with"))
        {
            int increment = int.Parse(Regex.Match(cmd, "\\d+").Value);
            ShuffleIncrement(increment);
        }
        else if(cmd.StartsWith("cut"))
        {
            int cut = int.Parse(Regex.Match(cmd, "-?\\d+").Value);
            ShuffleCut(cut);
        }
        else
        {
            throw new ArgumentException("Unknown command");
        }
    }

    private void ShuffleCut(int cut)
    {
        if (cut > 0) ShuffleCutTop(cut);
        if (cut < 0) ShuffleCutBottom(-1 * cut);
    }

    private void ShuffleCutBottom(int cut)
    {
        deck = [..deck[^cut..^0], .. deck[0..^cut]];
    }

    private void ShuffleCutTop(int cut)
    {
        deck = [..deck[cut..^0], ..deck[0..cut]];
    }

    private void ShuffleIncrement(int increment)
    {
        int[] deckSource = [..deck];
        deck = new int[deckSource.Length];

        int pos = 0;

        foreach (int put in deckSource)
        {
            deck[pos] = put;
            pos += increment;
            if(pos >= deck.Length) pos = pos - deck.Length;
        }
    }

    private void ShuffleReverse()
    {
        Array.Reverse(deck);
    }

    public override string ToString()
    {
        return string.Join(",", deck);
    }
}
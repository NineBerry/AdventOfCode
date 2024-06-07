#define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day22\Sample.txt";
    int deckSize = 10;
    int cardToFind = 5;
    long deckSizePart2 = 10_007;
    long positionToFindPart2 = 7614;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day22\Full.txt";
    int deckSize = 10_007;
    int cardToFind = 2019;
    long deckSizePart2 = 119315717514047;
    long positionToFindPart2 = 2020;
#endif

    string[] lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(deckSize, lines, cardToFind));
    Console.WriteLine("Part 2: " + Part2(deckSizePart2, lines, positionToFindPart2));
    Console.ReadLine();
}

int Part1(int decksize, string[] commands, int cardToFind)
{
    Deck deck = new Deck(decksize);

    foreach (string cmd in commands)
    {
        deck.Shuffle(cmd);
    }

    return deck.GetCardPosition(cardToFind);
}

// Idea for Part 2:
// Use a virtual deck only tracking the one position we want to read at the end,
// applying the commands in reverse order 
long Part2(long decksize, string[] commands, long positionToFind)
{
    VirtualDeck deck = new VirtualDeck(decksize);

    foreach (string cmd in commands.Reverse())
    {
        positionToFind = deck.UnshufflePosition(cmd, positionToFind);
    }

    return positionToFind;
}

public class Deck
{
    private int[] deck;

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
        if (cmd.StartsWith("deal into"))
        {
            ShuffleReverse();
        }
        else if (cmd.StartsWith("deal with"))
        {
            int increment = int.Parse(Regex.Match(cmd, "\\d+").Value);
            ShuffleIncrement(increment);
        }
        else if (cmd.StartsWith("cut"))
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
}


public class VirtualDeck
{
    private long deckSize;

    public VirtualDeck(long deckSize)
    {
        this.deckSize = deckSize;
    }

    public long UnshufflePosition(string cmd, long position)
    {
        if (cmd.StartsWith("deal into"))
        {
            return UnshuffleReverse(position);
        }
        else if (cmd.StartsWith("deal with"))
        {
            int increment = int.Parse(Regex.Match(cmd, "\\d+").Value);
            return UnshuffleIncrement(increment, position);
        }
        else if (cmd.StartsWith("cut"))
        {
            int cut = int.Parse(Regex.Match(cmd, "-?\\d+").Value);
            return UnshuffleCut(cut, position);
        }
        else
        {
            throw new ArgumentException("Unknown command");
        }
    }

    private long UnshuffleCut(int cut, long position)
    {
        if (cut > 0) return UnshuffleCutTop(cut, position);
        if (cut < 0) return UnshuffleCutBottom(-1 * cut, position);
        throw new ArgumentException();
    }

    private long UnshuffleCutBottom(int cut, long position)
    {
        return position; // TODO
    }

    private long UnshuffleCutTop(int cut, long position)
    {
        return position; // TODO
    }

    private long UnshuffleIncrement(int increment, long position)
    {
        // Start with 0, then subtract increment position times, always adding size if below 0

        return position; // TODO
    }

    private long UnshuffleReverse(long position)
    {
        return position; // TODO
    }
}


// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day05\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day05\Full.txt";
#endif

    Seat[] seats = File.ReadAllLines(fileName).Select(l => new Seat(l)).ToArray();

    Console.WriteLine("Part 1: " + Part1(seats));
    Console.WriteLine("Part 2: " + Part2(seats));
    Console.ReadLine();
}

long Part1(Seat[] seats)
{
    return seats.Max(s => s.SeatID);
}

long Part2(Seat[] seats)
{
    int frontRow = seats.Select(s => s.Row).Min();
    int backRow = seats.Select(s => s.Row).Max();

    int[] possibleSeatIDs = 
        seats
        .Where(s => s.Row != frontRow && s.Row != backRow)
        .Select(s => s.SeatID)
        .Order()
        .ToArray();

    foreach(var pair in possibleSeatIDs.Zip(possibleSeatIDs.Skip(1)))
    {
        if(pair.Second != pair.First + 1)
        {
            return pair.First + 1;
        }
    }

    return 0;
}

public class Seat
{
    public Seat(string line)
    {
        string rowEncoded = line.Substring(0, 7);
        string columnEncoded = line.Substring(7, 3);

        Row = Decode(rowEncoded, "FB");
        Column = Decode(columnEncoded, "LR");

        SeatID = Row * 8 + Column;
    }

    private int Decode(string encoded, string digits)
    {
        string s = encoded.Replace(digits[0], '0').Replace(digits[1], '1');
        return Convert.ToInt32(s, 2);
    }

    public readonly int Row;
    public readonly int Column;
    public readonly int SeatID;
}
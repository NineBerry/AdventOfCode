// #define Sample


{
#if Sample
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day03\Full.txt";
#endif

    Bush[] bushes = File.ReadAllLines(fileName).Select(Bush.Parse).ToArray();
    Console.WriteLine("Part 1: " + Part1(bushes));
    Console.WriteLine("Part 2: " + Part2(bushes));
    Console.WriteLine("Part 3: " + Part3(bushes));

    Console.ReadLine();
}


string Part1(Bush[] bushes)
{
    var mostCommon = bushes.GroupBy(c => c.Combined).OrderByDescending(g => g.Count()).First();
    return mostCommon.First().ToString();
}

long Part2(Bush[] bushes)
{
    return bushes.Count(b => b.Label == "Green");
}
long Part3(Bush[] bushes)
{
    return bushes.Sum(b => b.Price);
}


class Bush
{
    public readonly byte Red;
    public readonly byte Green;
    public readonly byte Blue;
    public int Combined => Red + 100 * Green + 100 * 100 * Blue;
    public readonly string Label;
    public readonly long Price;

    public Bush(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Label = MakeLabel(red, green, blue);
        Price = MakePrice(Label);
    }

    private string MakeLabel(byte red, byte green, byte blue)
    {
        if(red == green || red == blue || green == blue)
        {
            return "Special";
        }

        byte max = Math.Max(red, Math.Max(green, blue));

        if (max == red) return "Red";
        if (max == green) return "Green";
        if (max == blue) return "Blue";

        throw new Exception("Label cannot be determined");
    }

    private long MakePrice(string label)
    {
        return label switch
        {
            "Red" => 5,
            "Green" => 2,
            "Blue" => 4,
            _ => 10
        };
    }

    public static Bush Parse(string input)
    {
        byte[] values = input.Split(',').Select(byte.Parse).ToArray();
        return new Bush(values[0], values[1], values[2]);
    }

    public override string ToString()
    {
        return $"{Red},{Green},{Blue}";
    }

}
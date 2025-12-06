// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day06\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day06\Full.txt";
#endif

    var lines = File.ReadAllLines(fileName);
    (var numbers, var operators) = ParseSheet(lines);

    Console.WriteLine("Part 1: " + Part1(numbers, operators));
    Console.WriteLine("Part 2: " + Part2(numbers, operators));
    Console.ReadLine();
}

long Part1(string[][] numbers, string[] operators)
{
    long result = 0;

    foreach ((int index, string op) in operators.Index())
    {
        var operands =
            numbers[index]
            .Select(s => s.Trim())
            .Select(long.Parse);
        
        result += Tools.Calculate(op, operands);
    }
    
    return result;
}

long Part2(string[][] numbers, string[] operators)
{
    long result = 0;

    foreach ((int index, string op) in operators.Index())
    {
        var operands = numbers[index];
        result += CalculateCephalopody(op, operands);
    }

    return result;
}

long CalculateCephalopody(string op, string[] operands)
{
    long[] transformedOperands = GetCephalopodyNumbers(operands);
    return Tools.Calculate(op, transformedOperands);
}

long[] GetCephalopodyNumbers(string[] operands)
{
    // We expect as many numbers as the digits of the longest input element
    int transformedCount = operands.Max(o => o.Length);
    
    int[][] operandsAsDigits = operands.Select(Tools.DigitValuesOrZero).ToArray();

    List<long> tranformed = [];

    // Go over each digit position
    foreach (var index in Enumerable.Range(0, transformedCount))
    {
        // and add the digit at that position from every row
        long transformedSingle = 0;

        foreach(var source in operandsAsDigits)
        {
            var digit = source[index];
            if (digit == 0) continue;

            transformedSingle *= 10;
            transformedSingle += source[index];
        }

        tranformed.Add(transformedSingle);
    }

    return [.. tranformed];
}

(string[][] operands, string[] operators) ParseSheet(string[] lines)
{

    List<string> operators = new();
    List<string[]> operands = new();

    // Add space everywhere at end to have end signal for automaton
    string[] paddedLines = lines.Select(s => s + " ").ToArray();

    // All but last line contain operands
    string[] operandLines =
        paddedLines
        .Take(lines.Length - 1)
        .ToArray();

    int operandLineCount = operandLines.Length;
    
    string[] collector = [.. Enumerable.Repeat("", operandLineCount)];

    for (int index=0; index < paddedLines.First().Length; index++)
    {
        if(paddedLines.All(s => s[index] == ' '))
        {
            // Gap in all rows reached, remember and reset
            operands.Add(collector);
            collector = [.. Enumerable.Repeat("", operandLineCount)];
        }
        else
        {
            // still in group, update collector
            for(int rowIndex = 0; rowIndex < operandLineCount; rowIndex++)
            {
                collector[rowIndex] += operandLines[rowIndex][index];
            }
        }

        // Check for operator in last line
        char possibleOperator = paddedLines.Last()[index];
        if(possibleOperator != ' ')
        {
            operators.Add(possibleOperator.ToString());
        }
    }

    return (operands.ToArray(), operators.ToArray());
}

static class Tools
{
    internal static long Calculate(string op, IEnumerable<long> operands)
    {
        if (op == "+") return operands.Sum();
        if (op == "*") return operands.Aggregate(1L, (a,b) => a * b);

        throw new ArgumentException("Unknown Operand");
    }

    public static int[] DigitValuesOrZero(this string str)
    {
        return
            str
            .Select(Tools.DigitValueOrZero)
            .ToArray();
    }

    public static int DigitValueOrZero(this char ch)
    {
        if(int.TryParse(ch.ToString(), out var digit))
        {
            return digit; 
        }

        return 0;
    }
}
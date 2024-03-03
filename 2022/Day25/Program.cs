// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day25\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day25\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Solve(lines));
    Console.ReadLine();
}

string Solve(string[] lines)
{
    long sum = lines.Select(ParseSNAFU).Sum();
    return ToSNAFU(sum);
}

long ParseSNAFU(string input)
{
    long result = 0;
    long factor = 1;

    foreach(var ch in input.Reverse())
    {
        int digit = ch switch
        {
            '0' => 0,
            '1' => 1,
            '2' => 2,
            '-' => -1,
            '=' => -2,
            _ => throw new ArgumentException("Invalid character")
        };

        result += (digit * factor);
        factor *= 5;
    }

    return result;
}

string ToSNAFU(long value)
{
    string inBase5 = new string(DecimalToArbitrarySystem(value, 5).Reverse().ToArray());
    string result = "";
    int carry = 0;

    int i = 0;
    while(i < inBase5.Length || carry != 0)
    {
        int digitValue = (i < inBase5.Length) ? (int)char.GetNumericValue(inBase5[i]) : 0;
        digitValue += carry;
        carry = 0;
        i++;

        carry = digitValue / 5;
        digitValue %= 5;

        if(digitValue is >= 0 and <= 2)
        {
            result += digitValue.ToString();
        }
        else if(digitValue is 3)
        {
            result += '=';
            carry++;
        }
        else if (digitValue is 4)
        {
            result += '-';
            carry++;
        }
    }
    
    return new string(result.Reverse().ToArray());

}

// Taken from https://stackoverflow.com/a/10981113/101087
string DecimalToArbitrarySystem(long decimalNumber, int radix)
{
    const int BitsInLong = 64;
    const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    if (radix < 2 || radix > Digits.Length)
        throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

    if (decimalNumber == 0)
        return "0";

    int index = BitsInLong - 1;
    long currentNumber = Math.Abs(decimalNumber);
    char[] charArray = new char[BitsInLong];

    while (currentNumber != 0)
    {
        int remainder = (int)(currentNumber % radix);
        charArray[index--] = Digits[remainder];
        currentNumber = currentNumber / radix;
    }

    string result = new String(charArray, index + 1, BitsInLong - index - 1);
    if (decimalNumber < 0)
    {
        result = "-" + result;
    }

    return result;
}
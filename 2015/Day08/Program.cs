// #define Sample
using System.Text;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day08\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day08\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string[] input)
{
    int literalSum = 0;
    int valueSum = 0;

    foreach(var literal in input)
    {
        var value = GetLiteralValue(literal);
        literalSum += literal.Length;
        valueSum += value.Length;
    }

    return literalSum - valueSum;
}

long Part2(string[] input)
{
    int literalSum = 0;
    int valueSum = 0;

    foreach (var value in input)
    {
        var literal = EncodeValue(value);

        literalSum += literal.Length;
        valueSum += value.Length;
    }

    return literalSum - valueSum;
}

string GetLiteralValue(string literal)
{
    string inner = literal.Substring(1, literal.Length - 2);

    StringBuilder sb = new StringBuilder();

    string currentToken = "";

    foreach(char ch in inner)
    {
        if (ch == '\\')
        {
            if(currentToken == "\\")
            {
                sb.Append(ch);
                currentToken = "";
            }
            else
            {
                sb.Append(currentToken);
                currentToken = "\\";
            }
        }
        else if (ch == '"')
        {
            if (currentToken == "\\")
            {
                sb.Append('"');
                currentToken = "";
            }
            else
            {
                sb.Append(currentToken);
                sb.Append(ch);
            }
        }
        else if (ch == 'x')
        {
            if (currentToken == "\\")
            {
                currentToken = "\\x";
            }
            else
            {
                sb.Append(currentToken);
                sb.Append(ch);
            }
        }
        else if (currentToken.Length == 2)
        {
            currentToken += ch;
        }
        else if (currentToken.Length == 3)
        {
            currentToken += ch;

            string hex = currentToken.Substring(2, 2);
            byte converted = Convert.FromHexString(hex)[0];
            sb.Append((char)converted);

            currentToken = "";
        }
        else
        {
            sb.Append(ch);
        }

    }

    return sb.ToString();
}


string EncodeValue(string value)
{
    StringBuilder sb = new StringBuilder();

    foreach (char ch in value)
    {
        if (ch == '"')
        {
            sb.Append("\\\"");
        }
        else if(ch == '\\')
        {
            sb.Append("\\\\");
        }
        else
        {
            sb.Append(ch);
        }
    }

    return '"' + sb.ToString() + '"';
}
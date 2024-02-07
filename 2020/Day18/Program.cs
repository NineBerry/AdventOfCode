// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day18\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day18\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string[] formulas)
{
    return formulas.Sum(Calculate);
}
long Part2(string[] formulas)
{
    return formulas.Sum(CalculateAdvancedMath);
}

long Calculate(string formulaString)
{
    Token[] tokens = Scan(formulaString);
    return Execute(tokens);
}

long CalculateAdvancedMath(string formulaString)
{
    Token[] tokens = Scan(formulaString);
    tokens = ApplyAdvancedMathPrecedence(tokens);
    return Execute(tokens);
}

Token[] Scan(string formulaString)
{
    List<Token> tokens = new();

    bool inNumber = false;
    string current = "";

    foreach(var ch in formulaString + " ")
    {
        if(ch is >= '0' and <= '9')
        {
            if (inNumber)
            {
                current += ch;
            }
            else
            {
                inNumber = true;
                current = "" + ch;
            }
        }
        else
        {
            if (inNumber)
            {
                inNumber = false;
                tokens.Add(new Token { TokenType = TokenType.Number, Value = current });
                current = "";
            }
        }

        if (ch is '+' or '*') tokens.Add(new Token { TokenType = TokenType.Operator, Value = "" + ch });
        if (ch is '(') tokens.Add(new Token { TokenType = TokenType.OpenGroup });
        if (ch is ')') tokens.Add(new Token { TokenType = TokenType.CloseGroup });
    }

    return tokens.ToArray();    
}

Token[] ApplyAdvancedMathPrecedence(Token[] originalTokens)
{
    List<Token> tokens = new(originalTokens);

    int index = 0;
    while (index < tokens.Count) 
    {
        var token = tokens[index];
        if (token.TokenType == TokenType.Operator && token.Value == "+")
        {
            // find end of right operand and add ) there
            int rightPosition = FindIndexBehindOperand(tokens, index, +1);
            tokens.Insert(rightPosition + 1, new Token {TokenType = TokenType.CloseGroup});

            // Add ( before left operand
            int leftPosition = FindIndexBehindOperand(tokens, index, -1);
            tokens.Insert(leftPosition, new Token { TokenType = TokenType.OpenGroup});

            index += 2;
        }
        else
        {
            index++;
        }
    }

    return tokens.ToArray();
}

int FindIndexBehindOperand(List<Token> tokens, int originalIndex, int direction)
{
    int index = originalIndex + direction;

    if (tokens[index].TokenType == TokenType.Number) return index;

    int groupCounter = 0;

    do
    {
        if(tokens[index].TokenType == TokenType.OpenGroup) groupCounter++;
        if (tokens[index].TokenType == TokenType.CloseGroup) groupCounter--;

        if(groupCounter == 0) return index;
        index += direction;
    } while (true);
}


long Execute(Token[] tokens)
{
    long currentValue = 0;
    char lastOperator = '+';

    Stack<(long Value, char LastOperator)> stack = [];

    foreach (var token in tokens)
    {
        switch (token.TokenType)
        {
            case TokenType.Operator:
                lastOperator = token.Value[0];
                break;

            case TokenType.Number:
                currentValue = ApplyOperator(currentValue, token.ValueAsInt, lastOperator);
                break;

            case TokenType.OpenGroup:
                stack.Push((currentValue, lastOperator));
                lastOperator = '+';
                currentValue = 0;
                break;

            case TokenType.CloseGroup:
                long groupValue = currentValue;
                (currentValue, lastOperator) = stack.Pop();
                currentValue = ApplyOperator(currentValue, groupValue, lastOperator);
                break;
        }
    }

    return currentValue;
}

long ApplyOperator(long value, long secondValue, char op)
{
    return op switch
    {
        '+' => value + secondValue,
        '*' => value * secondValue,
        _ => throw new ApplicationException("Unknown Operator")
    };
}

enum TokenType
{
    Number,
    Operator,
    OpenGroup,
    CloseGroup,
}

record struct Token
{
    public TokenType TokenType;
    public string Value;
    public int ValueAsInt => int.Parse(Value);
}
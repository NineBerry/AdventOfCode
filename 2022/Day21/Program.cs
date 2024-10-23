// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day21\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day21\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);
    MonkeyBusiness business = new(lines);

    Console.WriteLine("Part 1: " + Part1(business));
    Console.WriteLine("Part 2: " + Part2(business));
    Console.ReadLine();
}

long Part1(MonkeyBusiness monkeyBusiness)
{
    return monkeyBusiness.GetMonkey(MonkeyBusiness.Root).GetValue(monkeyBusiness);
}

long Part2(MonkeyBusiness monkeyBusiness)
{
    Monkey rootMonkey = monkeyBusiness.GetMonkey(MonkeyBusiness.Root);
    rootMonkey.Operator = '=';

    return rootMonkey.GetHumanValue(monkeyBusiness, 0);
}

public class MonkeyBusiness
{
    Dictionary<string, Monkey> monkeys = [];

    public MonkeyBusiness(string[] input)
    {
        monkeys = input.Select(s => new Monkey(s)).ToDictionary(m => m.Name, m => m);
    }

    public Monkey GetMonkey(string monkey)
    {
        return monkeys[monkey];
    }

    public static string Root = "root";
    public static string Human = "humn";
}

public class Monkey
{
    public string Name { get; set; }

    public Monkey(string line)
    {
        Name = line.Substring(0, 4);

        if (line.Length == 17)
        {
            Operator = line[11];
            LeftMonkey = line.Substring(6, 4);
            RightMonkey = line.Substring(13, 4);
        }
        else
        {
            Value = int.Parse(line.Substring(6));
        }
    }

    public long GetValue(MonkeyBusiness business)
    {
        if (!Value.HasValue)
        {
            long left = business.GetMonkey(LeftMonkey).GetValue(business);
            long right = business.GetMonkey(RightMonkey).GetValue(business);
            Value = Calculate(left, right, Operator);
        }
        return Value!.Value;
    }

    private long Calculate(long left, long right, char op)
    {
        return op switch
        {
            '+' => left + right,
            '-' => left - right,
            '*' => left * right,
            '/' => left / right,
            _ => throw new NotImplementedException(),
        };
    }

    private long CalculateRequiredLeft(long required, long right, char op)
    {
        long result= op switch
        {
            '+' => required - right,
            '-' => required + right,
            '*' => required / right,
            '/' => required * right,
            '=' => right,
            _ => throw new NotImplementedException(),
        };

        return result;
    }

    private long CalculateRequiredRight(long required, long left, char op)
    {
        long result = op switch
        {
            '+' => required - left,
            '-' => left - required,
            '*' => required / left,
            '/' => left / required,
            '=' => left,
            _ => throw new NotImplementedException(),
        };

        return result;
    }

    public long GetHumanValue(MonkeyBusiness business, long requiredValue)
    {
        if (Name == MonkeyBusiness.Human) return requiredValue;

        Monkey leftMonkey = business.GetMonkey(LeftMonkey);
        Monkey rightMonkey = business.GetMonkey(RightMonkey);

        if (leftMonkey.ContainsHuman(business))
        {
            long nextRequiredValue = CalculateRequiredLeft(requiredValue, rightMonkey.GetValue(business), Operator);
            return leftMonkey.GetHumanValue(business, nextRequiredValue);
        }
        else if (rightMonkey.ContainsHuman(business))
        {
            long nextRequiredValue = CalculateRequiredRight(requiredValue, leftMonkey.GetValue(business), Operator);
            return rightMonkey.GetHumanValue(business, nextRequiredValue);
        }
        else throw new InvalidDataException();
    }

    public bool ContainsHuman(MonkeyBusiness business)
    {
        if (Name == MonkeyBusiness.Human) return true;
        if (Operator == ' ') return false;

        return business.GetMonkey(LeftMonkey).ContainsHuman(business)
            || business.GetMonkey(RightMonkey).ContainsHuman(business);
    }


    private long? Value = null;
    public readonly string LeftMonkey = "";
    public readonly string RightMonkey = "";
    public char Operator = ' ';
}
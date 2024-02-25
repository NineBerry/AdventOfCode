#define Sample

using System.Numerics;
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day11\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day11\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string input)
{
    MonkeyBusiness monkeyBusiness = new(input, reduceWorry: true);

    foreach(var _ in Enumerable.Range(0, 20))
    {
        monkeyBusiness.RunRound();
    }

    return monkeyBusiness.GetMonkeyBusinessLevel();
}

long Part2(string input)
{
    MonkeyBusiness monkeyBusiness = new(input, reduceWorry: false);
    
    foreach (var step in Enumerable.Range(0, 10_000))
    {
        monkeyBusiness.RunRound();
    }

    return monkeyBusiness.GetMonkeyBusinessLevel();
}

public class MonkeyBusiness
{
    public MonkeyBusiness(string input, bool reduceWorry)
    {
        Monkeys = 
            input
            .ReplaceLineEndings("\n")
            .Split("\n\n")
            .Select(l => new Monkey(l, reduceWorry))
            .ToDictionary(m => m.ID, m => m);
    }

    public bool ReduceWorry;
    public Dictionary<int, Monkey> Monkeys;

    public void RunRound()
    {
        foreach(var monkey in Monkeys.Values.OrderBy(m => m.ID)) 
        {
            monkey.InspectItems(this);
        }
    }

    public long  GetMonkeyBusinessLevel()
    {
        var mostActiveMonkeyInspections = 
            Monkeys.Values
            .Select(m => (long)m.ItemsInspected)
            .OrderDescending()
            .Take(2)
            .ToArray();
        return mostActiveMonkeyInspections.Aggregate((a, b) => a * b);
    }
}

public class Monkey
{
    public Monkey(string inputText, bool reduceWorry)
    {
        string[] input = inputText.Split("\n").ToArray();

        ID = GetIntegers(input[0]).Single();
        Items = GetIntegers(input[1]).Select(i => (BigInteger)i).ToList();
        Operation = new Operation(input[2].Split("= ")[1]);
        TestDivisor = GetIntegers(input[3]).Single();
        GiveToMonkeyIfTrue = GetIntegers(input[4]).Single();
        GiveToMonkeyIfFalse = GetIntegers(input[5]).Single();

        ReduceWorry = reduceWorry;
    }

    private int[] GetIntegers(string line)
    {
        return Regex.Matches(line, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
    }

    public void InspectItems(MonkeyBusiness monkeyBusiness)
    {
        foreach(var item in Items)
        {
            InspectItem(item, monkeyBusiness);
        }
        Items.Clear();
    }

    private void InspectItem(BigInteger item, MonkeyBusiness monkeyBusiness)
    {
        ItemsInspected++;

        if (ReduceWorry)
        {
            BigInteger modifiedItem = Operation.PerformOperation(item, item);
            modifiedItem /= 3;
            int nextMonkey = (modifiedItem % TestDivisor == 0) ? GiveToMonkeyIfTrue : GiveToMonkeyIfFalse;
            monkeyBusiness.Monkeys[nextMonkey].Items.Add(modifiedItem);
        }
        else
        {
            // Not working. Idea: Instead of using a fixed number, use remainder classes for all
            // used remainders. The perform the operation on each remainder class.

            BigInteger reducedItem = item % TestDivisor;
            BigInteger modifiedItem = Operation.PerformOperation(reducedItem, item);
            int nextMonkey = (modifiedItem % TestDivisor == 0) ? GiveToMonkeyIfTrue : GiveToMonkeyIfFalse;
            monkeyBusiness.Monkeys[nextMonkey].Items.Add(modifiedItem);
        }
    }

    public int ID;
    public List<BigInteger> Items = [];
    public Operation Operation;
    public int TestDivisor;
    public int GiveToMonkeyIfTrue;
    public int GiveToMonkeyIfFalse;
    public int ItemsInspected = 0;
    public bool ReduceWorry;
}


public class Operation
{
    public Operation(string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);    
        OperandLeft = parts[0];
        OperandRight = parts[2];
        Operator = parts[1][0];
    }

    public char Operator;
    public string OperandLeft;
    public string OperandRight;

    public BigInteger PerformOperation(BigInteger leftOldValue, BigInteger rightOldValue)
    {
        BigInteger left = GetValue(OperandLeft, leftOldValue);
        BigInteger right = GetValue(OperandRight, rightOldValue);

        return Operator switch
        {
            '*' => left * right,
            '+' => left + right,
            _ => throw new ApplicationException("Unknown Operator")
        };
    }

    private BigInteger GetValue(string description, BigInteger oldValue)
    {
        if(description == "old") return oldValue;
        return BigInteger.Parse(description);
    }
}
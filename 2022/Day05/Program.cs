// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day05\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day05\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    Instruction[] instructions = lines.SkipWhile(s => s != "").Skip(1).Select(s => new Instruction(s)).ToArray();
    string[] boxStacksDefinition = lines.TakeWhile(s => s != "").ToArray();

    Console.WriteLine("Part 1: " + Part1(boxStacksDefinition, instructions));
    Console.WriteLine("Part 2: " + Part2(boxStacksDefinition, instructions));
    Console.ReadLine();
}

string Part1(string[] boxStacksDefinition, Instruction[] instructions)
{
    BoxStacks boxStacks = new(boxStacksDefinition);
    
    foreach (var instruction in instructions)
    {
        boxStacks.ApplyPart1(instruction);
    }

    return boxStacks.GetTopString();
}

string Part2(string[] boxStacksDefinition, Instruction[] instructions)
{
    BoxStacks boxStacks = new(boxStacksDefinition);

    foreach (var instruction in instructions)
    {
        boxStacks.ApplyPart2(instruction);
    }

    return boxStacks.GetTopString();
}


public record Instruction
{
    public int Count;
    public int From;
    public int To;

    public Instruction(string line)
    {
        int[] values = Regex.Matches(line, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
        Count = values[0];
        From = values[1];
        To = values[2];
    }
}

public class BoxStacks
{
    public BoxStacks(string[] input)
    {
        stacks = Enumerable.Range(0, 10).Select(n => new Stack<char>()).ToArray();

        var matches = Regex.Matches(input.Last(), "[0-9]").ToArray();

        foreach (var match in matches)
        {
            int number = int.Parse(match.Value);
            int stringPos = match.Index;
            for(int line = input.Length - 2; line >= 0; line--)
            {
                char ch = input[line][stringPos];
                if(ch != ' ') stacks[number].Push(ch);
            }
        }
    }

    public string GetTopString()
    {
        return string.Join("", stacks.Select(stack => stack.Count == 0 ? "" : "" + stack.Peek()));
    }

    public void ApplyPart1(Instruction instruction)
    {
        var from = stacks[instruction.From];
        var to = stacks[instruction.To];

        foreach (var _ in Enumerable.Range(1, instruction.Count)) 
        { 
            var ch = from.Pop();    
            to.Push(ch);
        }
    }

    public void ApplyPart2(Instruction instruction)
    {
        var from = stacks[instruction.From];
        var to = stacks[instruction.To];

        Stack<char> temp = [];

        foreach (var _ in Enumerable.Range(1, instruction.Count))
        {
            var ch = from.Pop();
            temp.Push(ch);
        }

        while(temp.TryPop(out char ch))
        {
            to.Push(ch);
        }
    }

    private Stack<char>[] stacks = [];
}
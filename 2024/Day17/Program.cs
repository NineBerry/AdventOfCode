﻿// #define Sample

using System.Text.RegularExpressions;
using static Computer;

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2024\Day17\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2024\Day17\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2024\Day17\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2024\Day17\Full.txt";
#endif

    Console.WriteLine("Part 1: " + Part1(fileNamePart1));
    Console.WriteLine("Part 2: " + Part2(fileNamePart2));
    Console.ReadLine();
}

string Part1(string fileName)
{
    var parsed = ParseFile(fileName); 
    var output = RunComputer(parsed.Program, parsed.RegisterA);
    return string.Join(',', output);
}

long Part2(string fileName)
{
    long[] program = ParseFile(fileName).Program;

    var cycles = FindCycles(program);
    return LockPick(program, program.Length - 1, cycles, cycles.Last());
}

long[] FindCycles(long[] program)
{
    long test = 1;
    long currentIncrement = 1;
    long currentCycleCounter = 0;
    int previousLength = 1;
    List<long> cycles = [1];

    while (true)
    {
        var output = RunComputer(program, test);
        currentCycleCounter++;

        if (output.Length > previousLength)
        {
            // Console.WriteLine($"Found cycle {currentCycleCounter} at {test}");
            cycles.Add(currentIncrement);
            previousLength = output.Length;
            currentIncrement *= currentCycleCounter;
            currentCycleCounter = 0;
        }

        if (output.Length == program.Length)
        {
            break;
        }

        test += currentIncrement;
    }

    cycles.Add(test);
    
    return [.. cycles];
}

long LockPick(long[] program, int positionToMatch, long[] cycles, long test)
{
    long currentIncrement = cycles[positionToMatch];

    while(true)
    {
        var output = RunComputer(program, test);
        
        if(Tools.EqualEnds(program, output, positionToMatch))
        {
            if (positionToMatch == 0) return test;

            long result = LockPick(program, positionToMatch - 1, cycles, test);
            if (result != 0) return result;
        }

        test += currentIncrement;                    
    }
}

long[] RunComputer(long[] program, long registerA)
{
    Computer computer = new Computer(program);
    computer.RegisterA = registerA;
    List<long> output = [];

    computer.Output += (Computer computer, long value) =>
    {
        output.Add(value);
    };

    computer.Run();

    return output.ToArray();
}

(long[] Program, long RegisterA) ParseFile(string fileName)
{
    string inputText = File.ReadAllText(fileName);
    long[] inputValues = Tools.ParseIntegers(inputText);
    long[] program = inputValues.Skip(3).ToArray();

    return (program, inputValues[0]);
}

public static class Tools
{
    public static long[] ParseIntegers(string input)
    {
        return
            Regex.Matches(input, @"-?\d+")
            .Select(m => long.Parse(m.Value))
            .ToArray();
    }

    public static bool EqualEnds(long[] first, long[] second, int offset)
    {
        return first.Skip(offset)
            .Zip(second.Skip(offset))
            .All(p => p.First == p.Second);
    }
}

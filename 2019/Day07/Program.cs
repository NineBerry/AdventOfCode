// #define Sample

using System.Collections.Concurrent;

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2019\Day07\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2019\Day07\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2019\Day07\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2019\Day07\Full.txt";
#endif

    int[] programPart1 = ReadProgram(fileNamePart1);
    Console.WriteLine("Part 1: " + Part1(programPart1));

    int[] programPart2 = ReadProgram(fileNamePart2);
    Console.WriteLine("Part 2: " + Part2(programPart2));
    Console.ReadLine();
}

static int[] ReadProgram(string fileNamePart2)
{
    return File.ReadAllText(fileNamePart2).Split(',').Select(int.Parse).ToArray();
}

int Part1(int[] program)
{
    var combinations = Combinations.CreateCombinations(Enumerable.Range(0, 5).ToArray());
    var values = combinations.Select(comb => TestCombination(program, comb)).ToArray();
    return values.Max();
}

int TestCombination(int[] program, int[] combination)
{
    int output = 0;

    foreach (int i in combination)
    {
        output = RunComputer(program, i, output);
    }

    return output;
}

int RunComputer(int[] program, int phaseSetting, int input)
{
    int output = 0;
    bool firstCall = true;

    Computer computer = new Computer(program);
    computer.Input += (c) =>
    {
        if (firstCall)
        {
            firstCall = false;
            return phaseSetting;
        }
        return input;
    };
    computer.Output += (c, value) => output = value;

    computer.Run();
    return output;
}

int Part2(int[] program)
{
    var combinations = Combinations.CreateCombinations(Enumerable.Range(5, 5).ToArray());
    var values = combinations.Select(comb => TestCombinationWithFeedbackLoop(program, comb)).ToArray();
    return values.Max();
}

int TestCombinationWithFeedbackLoop(int[] program, int[] combination)
{
    int output = 0;

    Computer computerA = new Computer(program);
    Computer computerB = new Computer(program);
    Computer computerC = new Computer(program);
    Computer computerD = new Computer(program);
    Computer computerE = new Computer(program);

    BlockingCollection<int> queueA = [];
    BlockingCollection<int> queueB = [];
    BlockingCollection<int> queueC = [];
    BlockingCollection<int> queueD = [];
    BlockingCollection<int> queueE = [];

    computerA.Input += (c) => queueA.Take();
    computerA.Output += (c, value) => queueB.Add(value);

    computerB.Input += (c) => queueB.Take();
    computerB.Output += (c, value) => queueC.Add(value);

    computerC.Input += (c) => queueC.Take();
    computerC.Output += (c, value) => queueD.Add(value);

    computerD.Input += (c) => queueD.Take();
    computerD.Output += (c, value) => queueE.Add(value);

    computerE.Input += (c) => queueE.Take();
    computerE.Output += (c, value) =>
    {
        output = value;
        queueA.Add(value);
    };

    queueA.Add(combination[0]);
    queueB.Add(combination[1]);
    queueC.Add(combination[2]);
    queueD.Add(combination[3]);
    queueE.Add(combination[4]);

    queueA.Add(0);

    var taskA = Task.Factory.StartNew(() => computerA.Run(), TaskCreationOptions.LongRunning);
    var taskB = Task.Factory.StartNew(() => computerB.Run(), TaskCreationOptions.LongRunning);
    var taskC = Task.Factory.StartNew(() => computerC.Run(), TaskCreationOptions.LongRunning);
    var taskD = Task.Factory.StartNew(() => computerD.Run(), TaskCreationOptions.LongRunning);
    var taskE = Task.Factory.StartNew(() => computerE.Run(), TaskCreationOptions.LongRunning);

    Task.WaitAll(taskA, taskB, taskC, taskD, taskE);

    return output;
}



public static class Combinations
{
    public static IEnumerable<int[]> CreateCombinations(int[] numbers)
    {
        foreach (var combination in CreateCombinationsRecursive(numbers, []))
        {
            yield return combination;
        }
    }

    private static IEnumerable<int[]> CreateCombinationsRecursive(int[] numbers, int[] soFar)
    {
        if (soFar.Length == numbers.Length)
        {
            yield return [.. soFar];
        }
        else
        {
            foreach (var i in numbers)
            {
                if (soFar.Contains(i)) continue;

                foreach (var combination in CreateCombinationsRecursive(numbers, [.. soFar, i]))
                {
                    yield return combination;
                }
            }
        }
    }
}
using System.Collections.Concurrent;
using System.Text;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day21\Full.txt";
    string visualizationFileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day21\Visualization.txt";

    long[] program = ReadProgram(fileName);
    Grid grid = new Grid(program, visualizationFileName);
    Console.WriteLine("Part 1: " + grid.RunHullSurvey());
    Console.WriteLine("Part 2: " + grid.RunExtendedHullSurvey());

    Console.ReadLine();
}

static long[] ReadProgram(string fileNamePart2)
{
    return File.ReadAllText(fileNamePart2).Split(',').Select(long.Parse).ToArray();
}

public class Grid
{
    private long[] Program;
    private string VisualizationFileName;

    public Grid(long[] program, string visualizationFileName)
    {
        VisualizationFileName = visualizationFileName;
        Program = program;
    }

    public long RunHullSurvey()
    {
        string[] code = [
            "NOT A T",
            "OR T J",
            "NOT B T",
            "OR T J",
            "NOT C T",
            "OR T J",
            "AND D J",
            "WALK",
            ];

        return RunCode(code);
    }

    public long RunExtendedHullSurvey()
    {
        string[] code = [
            "NOT A T",
            "OR T J",
            "NOT B T",
            "OR T J",
            "NOT C T",
            "OR T J",
            "AND D J",
            "NOT J T",
            "OR E T",
            "OR H T",
            "AND T J",
            "RUN",
            ];

        return RunCode(code);
    }

    public long RunCode(string[] code)
    {
        (var reader, var writer) = RunProgram(Program);

        foreach (var line in code)
        {
            SendLineToRobot(writer, line);
        }

        var text = ReadTextFromRobot(reader, out var suffix);
        SaveStringToFile(text, VisualizationFileName);
        return suffix;
    }

    private static string ReadTextFromRobot(BlockingCollection<long> reader, out long suffix)
    {
        suffix = 0;

        StringBuilder sb = new StringBuilder();
        foreach (var item in reader.GetConsumingEnumerable())
        {
            if (item > 255)
            {
                suffix = item;
                continue;
            }

            char ch = (char)item;

            if (ch == '\n')
            {
                sb.AppendLine();
            }
            else
            {
                sb.Append(ch);
            }
        }

        return sb.ToString();
    }
    private void SaveStringToFile(string text, string visualizationFileName)
    {
        File.WriteAllText(visualizationFileName, text);
    }

    private static
        (BlockingCollection<long> Reader,
        BlockingCollection<long> Writer)
        RunProgram(long[] program)
    {
        Computer computer;
        BlockingCollection<long> reader = [];
        BlockingCollection<long> writer = [];
        computer = new Computer(program);
        computer.Input += (c) => writer.Take();
        computer.Output += (c, value) => reader.Add(value);
        Task.Factory.StartNew(() =>
        {
            computer.Run();
            reader.CompleteAdding();
        }, TaskCreationOptions.LongRunning);

        return (reader, writer);
    }

    private void SendLineToRobot(BlockingCollection<long> writer, string line)
    {
        foreach (var ch in line)
        {
            writer.Add(ch);
        }
        writer.Add(10);
    }
}
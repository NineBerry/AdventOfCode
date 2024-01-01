// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day06\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day06\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    Command[] commands = input.Select(ParseLine).ToArray();

    Console.WriteLine("Part 1: " + Part1(commands));
    Console.WriteLine("Part 2: " + Part2(commands));
    Console.ReadLine();
}

long Part1(Command[] input)
{
    int[,] grid = new int[1000, 1000];

    foreach (var command in input)
    {
        ApplyCommandToGrid(grid, command, ApplyCommandToCellPart1);
    }
    return grid.Cast<int>().Sum();
}

long Part2(Command[] input)
{
    int[,] grid = new int[1000, 1000];

    foreach (var command in input)
    {
        ApplyCommandToGrid(grid, command, ApplyCommandToCellPart2);
    }
    return grid.Cast<int>().Sum();
}

void ApplyCommandToGrid(int[,] grid, Command command, Func<int, CommandType, int> CommandHandler)
{
    for(int x=command.MinX; x<= command.MaxX; x++)
    {
        for (int y = command.MinY; y <= command.MaxY; y++)
        {
            grid[y, x] = CommandHandler(grid[y, x], command.CommandType);
        }
    }
}

int ApplyCommandToCellPart1(int current, CommandType commandType)
{
    return commandType switch
    {
        CommandType.On => 1,
        CommandType.Off => 0,
        CommandType.Toggle => current == 0 ? 1 : 0,
        _ => throw new ApplicationException("Invalid command type")
    };
}

int ApplyCommandToCellPart2(int current, CommandType commandType)
{
    return commandType switch
    {
        CommandType.On => current + 1,
        CommandType.Off => Math.Max(current - 1, 0),
        CommandType.Toggle => current + 2,
        _ => throw new ApplicationException("Invalid command type")
    };
}

Command ParseLine(string line)
{
    CommandType commandType = line[6] switch
    {
        'f' => CommandType.Off,
        'n' => CommandType.On,
        ' ' => CommandType.Toggle,
        _ => throw new ArgumentException("Invalid input")
    };

    int[] numbers = Regex.Matches(line, "[0-9]+").Select(m => int.Parse(m.Value)).ToArray();
    return new Command {CommandType = commandType, MinX = numbers[0], MinY = numbers[1], MaxX = numbers[2], MaxY = numbers[3]};
}

enum CommandType
{
    On, Off, Toggle
}

record struct Command
{
    public CommandType CommandType;
    public int MinX;
    public int MinY;
    public int MaxX;
    public int MaxY;
}

// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day08\Sample.txt";
    int width = 7;
    int height = 3;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day08\Full.txt";
    int width = 50;
    int height = 6;
#endif

    Grid grid = new Grid(width, height);
    Command[] commands = File.ReadAllLines(fileName).Select(l => new Command(l)).ToArray();

    Console.WriteLine("Part 1: " + Part1(grid, commands));
    Console.WriteLine("Part 2: \n" + Part2(grid));
    Console.ReadLine();
}

long Part1(Grid grid, Command[] commands)
{
    grid.ApplyCommands(commands);
    return grid.LightCount;
}

string Part2(Grid grid)
{
    return grid.ToString();
}

class Grid
{
    public Grid(int width, int height)
    {
        Height = height;
        Width = width;
    }

    private HashSet<Point> Lights = new();

    public readonly int Height;
    public readonly int Width;

    public int LightCount => Lights.Count;

    public void ApplyCommands(Command[] commands)
    {
        foreach (var command in commands)
        {
            ApplyCommand(command);
        }
    }

    private void ApplyCommand(Command command)
    {
        switch (command.Type)
        {
            case CommandType.Rect:
                ApplyCommandRect(command.Param1, command.Param2);
                break;
            case CommandType.RotateRow:
                ApplyCommandRotateRow(command.Param1, command.Param2);
                break;
            case CommandType.RotateColumn:
                ApplyCommandRotateColumn(command.Param1, command.Param2);
                break;
        }
    }

    private void ApplyCommandRotateColumn(int column, int rotateBy)
    {
        var pointsInColumn = Lights.Where(p => p.X == column).ToArray();
        Lights = Lights.Except(pointsInColumn).ToHashSet();

        foreach (var point in pointsInColumn)
        {
            Lights.Add(point with { Y = RotateCoordinate(point.Y, rotateBy, Height) });
        }
    }

    private void ApplyCommandRotateRow(int row, int rotateBy)
    {
        var pointsInRow = Lights.Where(p => p.Y == row).ToArray();
        Lights = Lights.Except(pointsInRow).ToHashSet();

        foreach (var point in pointsInRow)
        {
            Lights.Add(point with { X = RotateCoordinate(point.X, rotateBy, Width)} );
        }
    }

    private void ApplyCommandRect(int width, int height)
    {
        for(int x= 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var point = new Point(x, y);
                if(IsInGrid(point)) Lights.Add(point);
            }
        }
    }

    private int RotateCoordinate(int coordinate, int rotateBy, int spaceLength)
    {
        return (coordinate + rotateBy) % spaceLength;
    }

    public bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }


    private char[] emptyMap = null;
    public override string ToString()
    {
        if (emptyMap == null)
        {
            string line = new string(Enumerable.Repeat(' ', Width).ToArray());
            emptyMap = string.Join('\n', Enumerable.Range(0, Height).Select(s => line)).ToCharArray();
        }

        char[] map = (char[])emptyMap.Clone();

        foreach (var light in Lights)
        {
            // +1 for line break
            map[light.Y * (Width + 1) + light.X] = '█';
        }

        return new string(map);
    }
}

record struct Point(int X, int Y);

public enum CommandType 
{
    Rect,
    RotateRow,
    RotateColumn
}

public record Command
{
    public Command(string input)
    {
        Type = input switch
        {
            string s when s.StartsWith("rect") => CommandType.Rect,
            string s when s.StartsWith("rotate row") => CommandType.RotateRow,
            string s when s.StartsWith("rotate column") => CommandType.RotateColumn,

            _ => throw new ApplicationException("Invalid Command")
        };

        int[] values = Regex.Matches(input, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
        Param1 = values[0];
        Param2 = values[1];
    }

    public CommandType Type;
    public int Param1;
    public int Param2;
}
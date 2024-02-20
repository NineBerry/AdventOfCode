// #define Sample

using Instruction = (Direction Direction, int Distance);

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2022\Day09\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2022\Day09\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2022\Day09\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2022\Day09\Full.txt";
#endif

    Console.WriteLine("Part 1: " + Solve(fileNamePart1, 2));
    Console.WriteLine("Part 2: " + Solve(fileNamePart2, 10));
    Console.ReadLine();
}

Instruction[] ParseFile(string fileName)
{
    return
        File.ReadAllLines(fileName)
        .Select(s => ((Direction)s[0], int.Parse(s.Substring(2))))
        .ToArray();
}

long Solve(string fileName, int knotCount)
{
    var instructions = ParseFile(fileName);
    Grid grid = new Grid(knotCount);
    grid.PerformInstructions(instructions);

    return grid.TailVisitedCount;
}

public class Grid
{
    public Grid(int knotCount)
    {
        Knots = Enumerable.Range(0, knotCount).Select(i => new Point(0, 0)).ToArray();
        TailVisited.Add(Knots.Last());
    }

    public void PerformInstructions(Instruction[] instructions)
    {
        foreach (Instruction instruction in instructions)
        {
            PerformInstruction(instruction);
        }
    }

    private void PerformInstruction(Instruction instruction)
    {
        foreach (var _ in Enumerable.Range(1, instruction.Distance))
        {
            Knots[0] = Knots[0].GetNeightboringPoint(instruction.Direction);
            DragTail();
            TailVisited.Add(Knots.Last());
        }
    }

    private void DragTail()
    {
        for (int i = 1; i < Knots.Length; i++)
        {
            DragKnot(i - 1, i);
        }
    }

    private void DragKnot(int previousIndex, int nextIndex)
    {
        Point previousKnot = Knots[previousIndex];
        Point nextKnot = Knots[nextIndex];

        if (previousKnot.OutOfTouch(nextKnot))
        {
            Point dragDirection = new Point(Math.Sign(previousKnot.X - nextKnot.X), Math.Sign(previousKnot.Y - nextKnot.Y));
            Knots[nextIndex] = nextKnot.Add(dragDirection);
        }
    }

    private Point[] Knots;

    private HashSet<Point> TailVisited = [];

    public int TailVisitedCount => TailVisited.Count;
}

public record Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.West => this with { X = this.X - 1 },
            Direction.East => this with { X = this.X + 1 },
            Direction.South => this with { X = this.X, Y = this.Y + 1 },
            Direction.North => this with { X = this.X, Y = this.Y - 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public Point Add(Point vector)
    {
        return new Point(X + vector.X, Y + vector.Y);
    }

    public bool OutOfTouch(Point other)
    {
        return Math.Max(Math.Abs(other.X - X), Math.Abs(other.Y - Y)) > 1;
    }

    public static Direction[] AllDirections = [Direction.West, Direction.East, Direction.North, Direction.South];
}

public enum Direction
{
    West = 'L',
    East = 'R',
    North = 'U',
    South = 'D',
}
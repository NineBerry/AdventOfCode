// #define Sample
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day22\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day22\Full.txt";
#endif

    Node[] nodes = File.ReadAllLines(fileName).Skip(2).Select(line => new Node(line)).ToArray();

    Console.WriteLine("Part 1: " + Part1(nodes));
    Console.WriteLine("Part 2: " + Part2(nodes));
    Console.ReadLine();
}

int Part1(Node[] nodes)
{
    int count = 0;

    for(int i=0; i< nodes.Length; i++)
    {
        for(int j=0; j < nodes.Length; j++)
        {
            if (i == j) continue;

            var nodeA = nodes[i];
            var nodeB = nodes[j];

            if(nodeA.Used == 0) continue;
            
            if(nodeA.Used <= nodeB.Avail) count++;
        }
    }

    return count;
}

int Part2(Node[] nodes)
{
    Dictionary<Point, Node > initialNodesByPoint = nodes.ToDictionary(n => n.Point, n => n);

    int width = initialNodesByPoint.Keys.Max(p => p.X) + 1;
    int height = initialNodesByPoint.Keys.Max(p => p.Y) + 1;
    Grid grid = new Grid(width, height);

    Point initialDataNodePoint = new Point(width - 1, 0);
    Point targetNodePoint = new Point(0, 0);
    int secretDataSize = initialNodesByPoint[initialDataNodePoint].Used;

    HashSet<Point> globalHadData = [];
    PriorityQueue<(int Steps, Point DataNodePoint, Dictionary<Point, Node> Situation), int> todos = new();

    todos.Enqueue((0, initialDataNodePoint, initialNodesByPoint), 0);

    while(todos.TryDequeue(out var current, out var _))
    {
        if (current.DataNodePoint == targetNodePoint)
        {
                return current.Steps;
        }

        if (globalHadData.Contains(current.DataNodePoint)) continue;
        globalHadData.Add(current.DataNodePoint);

        foreach (var direction in Grid.AllDirections)
        {
            Point nextPoint = current.DataNodePoint.GetNeightboringPoint(direction);

            if (!grid.IsInGrid(nextPoint)) continue;

            Node nextNode = current.Situation[nextPoint];

            if (secretDataSize <= nextNode.Avail)
            {
                // Move actual data
                var changedSituation = new Dictionary<Point, Node>(current.Situation);
                changedSituation[nextPoint] = changedSituation[nextPoint] with { Used = changedSituation[nextPoint].Used + secretDataSize };
                changedSituation[current.DataNodePoint] = changedSituation[current.DataNodePoint] with { Used = changedSituation[current.DataNodePoint].Used - secretDataSize };

                todos.Enqueue((current.Steps + 1, nextPoint, changedSituation), current.Steps + 1);
            }
            else
            {
                // We are trying to clean nodes
                int dataToMoveSize = current.Situation[nextPoint].Used;
                var holes = current.Situation.Values.Where(n => n.Avail >= dataToMoveSize).Select(n => n.Point).Except([current.DataNodePoint]);

                foreach (var hole in holes)
                {
                    var shortestPath = GetShortestPath(hole, nextPoint, current.DataNodePoint, current.Situation, grid);

                    if (shortestPath is not [])
                    {
                        var changedSituation = new Dictionary<Point, Node>(current.Situation);

                        foreach (var pair in shortestPath.Reverse().Zip(shortestPath.Reverse().Skip(1)))
                        {
                            int moveSize = current.Situation[pair.First].Used;
                            changedSituation[pair.Second] = changedSituation[pair.Second] with { Used = changedSituation[pair.Second].Used + moveSize };
                            changedSituation[pair.First] = changedSituation[pair.First] with { Used = changedSituation[pair.First].Used - moveSize };
                        }

                        changedSituation[nextPoint] = changedSituation[nextPoint] with { Used = changedSituation[nextPoint].Used + secretDataSize };
                        changedSituation[current.DataNodePoint] = changedSituation[current.DataNodePoint] with { Used = changedSituation[current.DataNodePoint].Used - secretDataSize };

                        todos.Enqueue((current.Steps + shortestPath.Length, nextPoint, changedSituation), current.Steps + shortestPath.Length);
                    }
                }
            }

        }
    }

    return 0;
}

Point[] GetShortestPath(Point from, Point to, Point except, Dictionary<Point, Node> situation, Grid grid)
{
    HashSet<Point> visited = [];
    
    Queue<Point[]> todos = new();
    todos.Enqueue([from]);

    while (todos.TryDequeue(out var current))
    {
        if (current.Last() == to)
        {
            return current;
        }
        
        foreach (var direction in Grid.AllDirections)
        {
            Point nextPoint = current.Last().GetNeightboringPoint(direction);
            
            if(visited.Contains(nextPoint)) continue;
            if (!grid.IsInGrid(nextPoint)) continue;
            if (except == nextPoint) continue;
            if (current.Contains(nextPoint)) continue;
            if (situation[nextPoint].Size < situation[current.Last()].Used) continue;

            todos.Enqueue([.. current, nextPoint]);
            visited.Add(nextPoint);
        }
    }

    return [];
}

public record Point
{
    public Point(int x, int y)
    {
        X = x; 
        Y = y;
    }

    public int X;
    public int Y;

    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y + 1 },
            Direction.West => this with { X = this.X - 1 },
            Direction.North => this with { Y = this.Y - 1 },
            Direction.East => this with { X = this.X + 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}

public record struct Node
{
    public Node(string line)
    {
        int[] values = Regex.Matches(line, "\\d+").Select(m => int.Parse(m.Value)).ToArray();   
        Point = new Point(values[0], values[1]);
        Size = values[2];
        Used = values[3];
    }

    public Point Point;
    public int Size;
    public int Used;
    public int Avail => Size - Used;
}

public class Grid
{
    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public int Width;
    public int Height;

    public bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }
    
    public static Direction[] AllDirections = [Direction.East, Direction.North, Direction.South, Direction.West];

}

public enum Direction
{
    South,
    West,
    North,
    East
}

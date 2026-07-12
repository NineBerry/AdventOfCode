// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day07\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day07\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    Point.Direction[] movements = lines.First().Select(c => (Point.Direction)c).ToArray();
    List<Point> sushi = new List<Point>(lines.Skip(2).Select(Point.Parse));


    Console.WriteLine("Part 1: " + Part1(movements, sushi));
    Console.WriteLine("Part 2: " + Part2(movements, sushi));
    Console.WriteLine("Part 3: " + Part3(movements, sushi));

    Console.ReadLine();
}   
long Part1(Point.Direction[] movements, List<Point> sushi)
{
    movements = movements.Take(movements.Length / 2).ToArray();

    SnakesGame game = new SnakesGame(movements,  sushi);
    game.Play(canGrowBody: false, canEatBody: false);

    return game.EatenSushi.Count;
}

long Part2(Point.Direction[] movements, List<Point> sushi)
{
    SnakesGame game = new SnakesGame(movements, sushi);
    game.Play(canGrowBody: true, canEatBody: false    );

    return game.SnakeBody.Count;
}

long Part3(Point.Direction[] movements, List<Point> sushi)
{
    SnakesGame game = new SnakesGame(movements, sushi);
    game.Play(canGrowBody: true, canEatBody: true);

    return game.SnakeBody.Count * game.EatenSelfCount;
}

class SnakesGame
{
    private Point.Direction[] Movements { get; }
    private Stack<Point> SushiStack { get; }
    public List<Point> EatenSushi { get; } = [];
    public LinkedList<Point> SnakeBody { get; } = [];
    public int EatenSelfCount { get; private set; } = 0;

    public SnakesGame(Point.Direction[] movements, IEnumerable<Point> sushi)
    {
        Movements = movements;
        SushiStack = new Stack<Point>(sushi.Reverse());
    }

    public void Play(bool canGrowBody, bool canEatBody)
    {
        Point moveTo = new Point();
        SnakeBody.AddFirst(moveTo);

        foreach (var movement in Movements)
        {
            moveTo = moveTo.GetNeightboringPoint(movement);

            if (WillHitSelf(moveTo))
            {
                // Handle collision
                if (canEatBody)
                {
                    EatBody(moveTo);
                }
                else
                {
                    break;
                }
            }

            bool eaten = EatSushi(moveTo);
            MoveSnake(moveTo, eaten && canGrowBody);
        }
    }

    private void MoveSnake(Point moveTo, bool grow)
    {
        SnakeBody.AddFirst(moveTo);
        if (!grow)
        {
            SnakeBody.RemoveLast();
        }
    }

    private bool WillHitSelf(Point moveTo)
    {
        return SnakeBody.Contains(moveTo) && moveTo != SnakeBody.Last!.Value;
    }

    private void EatBody(Point fromToEnd)
    {
        bool found = false;
        
        do
        {
            found = SnakeBody.Last!.Value == fromToEnd;
            SnakeBody.RemoveLast();
        } while (!found && SnakeBody.Any());
        
        EatenSelfCount++;
    }

    private bool EatSushi(Point moveTo)
    {
        if (SushiStack.Count > 0 && SushiStack.Peek().Equals(moveTo))
        {
            EatenSushi.Add(SushiStack.Pop());
            return true;
        }
        return false;
    }
}

// Attention, we use coordinate system where Y counts up to the north for this puzzle.
record struct Point(int X, int Y)
{
    public static Point Parse(string input)
    {
        var parts = input.Split(',');
        return new Point(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y - 1 },
            Direction.West => this with { X = this.X - 1 },
            Direction.North => this with { Y = this.Y + 1 },
            Direction.East => this with { X = this.X + 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public enum Direction
    {
        None = '\0',
        South = 'v',
        West = '<',
        North = '^',
        East = '>'
    }
}
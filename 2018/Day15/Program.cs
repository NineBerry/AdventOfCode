// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day15\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day15\Full.txt";
#endif

    Console.WriteLine("Part 1: " + Part1(fileName));
    Console.WriteLine("Part 2: " + Part2(fileName));
    Console.ReadLine();
}

long Part1(string fileName)
{
    Game game = new Game(fileName, 3, false);
    return game.RunGame();
}

long Part2(string fileName)
{
    int attackPower = 3;
    try
    {
        while (true)
        {
            Console.Write("\rTesting attack power " + attackPower);
            Game game = new Game(fileName, attackPower, true);
            try
            {
                return game.RunGame();
            }
            catch (UnitDiedException)
            {
                attackPower++;
            }
        }
    }
    finally
    {
        Console.WriteLine("\r");
    }
}


public class Game
{
    public HashSet<Point> Walls = [];
    public List<Unit> Units = [];
    private int Width;
    private int Height;

    public Game(string fileName, int elfAttackPower, bool elvesMustNotDie)
    {
        string[] lines = File.ReadAllLines(fileName);

        Width = lines[0].Length;
        Height = lines.Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                
                Point point = new Point(x, y);

                switch (tile)
                {
                    case '#': Walls.Add(point);
                        break;
                    case 'G':
                        Units.Add(new Unit(UnitType.Goblin, point, this, 3, false));
                        break;
                    case 'E':
                        Units.Add(new Unit(UnitType.Elf, point, this, elfAttackPower, elvesMustNotDie));
                        break;
                }
            }
        }
    }

    public bool Round()
    {
        foreach (var unit in Units)
        {
            unit.StartRound();
        }

        var sorted = Units.OrderBy(u => u.RoundStartPoint).ToArray();

        foreach (var unit in sorted)
        {
            if (Units.Count(u => u.Alive && u.UnitType == UnitType.Elf) == 0) return false;
            if (Units.Count(u => u.Alive && u.UnitType == UnitType.Goblin) == 0) return false;

            if (!unit.Alive) continue;

            unit.Move();
            unit.Attack();
        }

        return true;
    }

    public long RunGame()
    {
        int rounds = 0;

        // Console.WriteLine(this);

        while (Round())
        {
            rounds++;
            // Console.WriteLine(rounds);
            // Console.WriteLine(this);
        }

        // Console.WriteLine(rounds);
        // Console.WriteLine(this);

        return rounds * getTotalHitPointsLeft();
    }

    public int getTotalHitPointsLeft()
    {
        return Units.Where(u => u.Alive).Sum(u => u.HitPoints);
    }

    public override string ToString()
    {
        List<string> lines = [];

        foreach (var y in Enumerable.Range(0, Height))
        {
            char[] line = Enumerable.Repeat('.', Width).ToArray();
            string additional = " ";

            foreach (var wall in Walls)
            {
                if (wall.Y == y)
                {
                    line[wall.X] = '#';
                }
            }

            foreach (var unit in Units.Where(u => u.Alive).OrderBy(u => u.CurrentPosition))
            {
                Point p = unit.CurrentPosition;
                if (p.Y == y)
                {
                    line[p.X] = unit.TypeAsString();

                    additional += $" {unit.TypeAsString()}({unit.HitPoints}) ";
                }
            }

            lines.Add(new string(line) + additional);
        }

        return string.Join("\n", lines);
    }
}

public class Unit
{
    private Game Game;

    public Unit(UnitType unitType, Point startPosition, Game game, int attackPower, bool unitMustNotDie)
    {
        UnitType = unitType;
        AbsoluteStartPoint = startPosition;
        RoundStartPoint = startPosition;
        CurrentPosition = startPosition;
        Game = game;
        AttackPower = attackPower;
        UnitMustNotDie = unitMustNotDie;
    }

    public void StartRound()
    {
        RoundStartPoint = CurrentPosition;
    }

    private IEnumerable<Unit> EnemysInRange()
    {
        var neighboringPoints = this.CurrentPosition.GetAllNeightboringPoints();
        return Game.Units.Where(u =>  (u.UnitType != this.UnitType) && u.Alive && neighboringPoints.Any(p => p == u.CurrentPosition));
    }

    public void Move()
    {
        if (!EnemysInRange().Any())
        {
            Point? moveTarget = GetMoveTarget();
            if(moveTarget.HasValue)
            {
                CurrentPosition = moveTarget.Value;
            }
        }
    }

    private Point? GetMoveTarget()
    {
        HashSet<Point> occupied = [..Game.Walls, ..Game.Units.Where(u => u.Alive).Select(u => u.CurrentPosition)];
        var targets = Game.Units
            .Where(u => u.Alive && (u.UnitType != this.UnitType))
            .SelectMany(u => u.CurrentPosition.GetAllNeightboringPoints())
            .Except(occupied)
            .ToHashSet();

        var possibleStarts = this.CurrentPosition.GetAllNeightboringPoints().Except(occupied).Order();

        int? shortestPathLengthFound = null;
        Point? shortestPathTarget = null;
        Point? shortestPathStart = null;


        foreach(var possibleStart in possibleStarts)
        {
            var foundShortest = GetShortestPath(possibleStart, occupied, targets);

            if (foundShortest.HasValue) {

                bool useFound = false;

                if (shortestPathLengthFound.HasValue)
                {
                    if(foundShortest.Value.Length < shortestPathLengthFound.Value)
                    {
                        useFound = true;
                    }
                    else if(foundShortest.Value.Length == shortestPathLengthFound.Value)
                    {
                        if(foundShortest.Value.Target.CompareTo(shortestPathTarget!.Value) < 0)
                        {
                            useFound = true;
                        }
                    }
                }
                else
                {
                    useFound = true;
                }
                
                if (useFound)
                {
                    shortestPathLengthFound = foundShortest.Value.Length;
                    shortestPathStart = possibleStart;
                    shortestPathTarget = foundShortest.Value.Target;
                }
            }
        }

        return shortestPathStart;
    }

    private (int Length, Point Target)? GetShortestPath(Point possibleStart, HashSet<Point> occupied, HashSet<Point> targets)
    {
        int? shortestPathLength = null;
        List<Point> foundTargets = [];

        HashSet<Point> visited = [];
        
        PriorityQueue<Point, int> todos = new();

        todos.Enqueue(possibleStart, 1);

        while (todos.TryDequeue(out var current, out int pathLength))
        {
            if (shortestPathLength.HasValue && pathLength > shortestPathLength.Value) break;
            
            if (visited.Contains(current)) continue;
            visited.Add(current);

            if (targets.Contains(current))
            {
                shortestPathLength = pathLength;
                foundTargets.Add(current);
            }


            foreach (var next in current.GetAllNeightboringPoints().Except(occupied).Except(visited))
            {
                todos.Enqueue(next, pathLength + 1);
            }
        }

        if (shortestPathLength.HasValue)
        {
            return (shortestPathLength.Value, foundTargets.Order().First());
        }
        
        return  null;
    }

    public void Attack()
    {
        var enemiesInRange = EnemysInRange();
        if (enemiesInRange.Any())
        {
            var attackTarget = enemiesInRange.OrderBy(u => u.HitPoints).ThenBy(u => u.CurrentPosition).FirstOrDefault();
            if(attackTarget != null)
            {
                Attack(attackTarget);
            }
        }
    }


    public void Attack(Unit otherUnit)
    {
        otherUnit.Attack(this.AttackPower);
    }


    public void Attack(int attackPower)
    {
        HitPoints = Math.Max(0, HitPoints - attackPower);

        if (UnitMustNotDie && HitPoints == 0) throw new UnitDiedException();
    }

    public char TypeAsString()
    {
        return UnitType == UnitType.Elf ? 'E' : 'G';
    }

    public UnitType UnitType;
    public Point AbsoluteStartPoint;
    public Point RoundStartPoint;
    public Point CurrentPosition;
    public int HitPoints = 200;
    public int AttackPower;
    public bool UnitMustNotDie;
    public bool Alive => HitPoints > 0;
}

public record struct Point(int X, int Y): IComparable<Point>
{
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

    public Point[] GetAllNeightboringPoints()
    {
        var me = this;
        return AllDirections.Select(me.GetNeightboringPoint).ToArray();
    }

    public int CompareTo(Point other)
    {
        if(Y == other.Y) return X.CompareTo(other.X);
        return Y.CompareTo(other.Y);
    }

    private static Direction[] AllDirections = [Direction.North, Direction.East, Direction.South, Direction.West];

}

public enum Direction
{
    None,
    West,
    North,
    South,
    East,
}

public enum UnitType
{
    Elf,
    Goblin
}

public class UnitDiedException: Exception
{

}
// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day23\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day23\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string[] input)
{
    CaveSystem cave = new CaveSystem(input);
    return cave.GetEnergyToOrder();
}


long Part2(string[] input)
{
    List<string> parts = input.ToList();
    parts.Insert(3, "  #D#C#B#A#");
    parts.Insert(4, "  #D#B#A#C#");

    CaveSystem cave = new CaveSystem(parts.ToArray());
    return cave.GetEnergyToOrder();
}

class CaveSystem
{
    public readonly int CaveSystemHeight;
    public readonly int CaveSystemWidth;
    public readonly string CaveTemplate;
    private readonly string InitialState;
    private readonly Point[] PossibleHallwayPositions;

    public const int HallwayPositionY = 1;
    public const int RoomAPositionX = 3;
    public const int RoomBPositionX = 5;
    public const int RoomCPositionX = 7;
    public const int RoomDPositionX = 9;

    public CaveSystem(string[] input)
    {
        CaveSystemHeight = input.Length;
        CaveSystemWidth = input[0].Length;
        InitialState = string.Join(Environment.NewLine, input);
        CaveTemplate = MakeCaveTemplate(InitialState);
        PossibleHallwayPositions = MakePossibleHallwayPositions();
    }

    private string MakeCaveTemplate(string initialState)
    {
        return initialState.Replace("A", ".").Replace("B", ".").Replace("C", ".").Replace("D", ".");
    }

    private Point[] MakePossibleHallwayPositions()
    {
        List<Point> result = [];

        foreach (var x in Enumerable.Range(1, CaveSystemWidth - 2))
        {
            if (x == RoomAPositionX || x == RoomBPositionX || x == RoomCPositionX || x == RoomDPositionX) continue;
            result.Add(new Point(x, HallwayPositionY));
        }

        return result.ToArray();
    }

    public long GetEnergyToOrder()
    {
        HashSet<string> seen = [];
        PriorityQueue<string, long> queue = new PriorityQueue<string, long>();
        queue.Enqueue(InitialState, 0);

        while (queue.TryDequeue(out var currentStateString, out long energy))
        {
            if (seen.Contains(currentStateString)) continue;
            seen.Add(currentStateString);

            CaveSystemState currentState = new CaveSystemState(currentStateString, this);

            if (currentState.IsFinalState())
            {
                return energy;
            }

            // Move amphipods from rooms to hallway or welcoming room
            foreach(var roomState in currentState.Rooms.Values)
            {
                if (roomState.NextToMoveOut != null)
                {
                    Amphipod amphipod = roomState.NextToMoveOut;

                    var targetRoom = currentState.Rooms[amphipod.Type];

                    if (targetRoom.Welcoming && targetRoom.WelcomingPoint.HasValue)
                    {
                        Move(amphipod, targetRoom.WelcomingPoint.Value);
                    }

                    foreach(var target in PossibleHallwayPositions)
                    {
                        Move(amphipod, target);
                    }
                }
            }

            // Move amphipods from hallway to welcoming rooms
            foreach (var amphipod in currentState.AmphipodsInHallway)
            {
                var targetRoom = currentState.Rooms[amphipod.Type];

                if (targetRoom.Welcoming && targetRoom.WelcomingPoint.HasValue)
                {
                    Move(amphipod, targetRoom.WelcomingPoint.Value);
                }
            }

            void Move(Amphipod amphipodToMove, Point moveTarget)
            {
                Point[] steps = GetSteps(amphipodToMove.Position, moveTarget);

                // Check there are no obstacles in the way
                if (steps.Any(currentState.AmphipodByPosition.ContainsKey)) return;

                string newState = currentState.MakeStringAfterMove(amphipodToMove, moveTarget, this);
                if (seen.Contains(newState)) return;
                queue.Enqueue(newState, energy + amphipodToMove.GetMoveEnergy(steps.Length));
            }
        }

        return 0;
    }

    private Point[] GetSteps(Point from, Point to)
    {
        List<Point> result = [];

        if(from.Y != HallwayPositionY && to.Y != HallwayPositionY)
        {
            // Move from room to room
            AddPointsVertically(result, from.Y, HallwayPositionY, from.X);
            AddPointsHorizontally(result, from.X, to.X, HallwayPositionY);
            AddPointsVertically(result, HallwayPositionY, to.Y, to.X);
        }
        else if(from.Y == HallwayPositionY)
        {
            // Move from hallway to room
            AddPointsHorizontally(result, from.X, to.X, from.Y);
            AddPointsVertically(result, from.Y, to.Y, to.X);
        }
        else
        {
            // Move from room to hallway
            AddPointsVertically(result, from.Y, to.Y, from.X);
            AddPointsHorizontally(result, from.X, to.X, to.Y);
        }

        return result.ToArray();
    }

    private void AddPointsVertically(List<Point> list, int fromY, int toY, int x)
    {
        if(fromY > toY)
        {
            for(int y = fromY - 1; y >= toY; y--)
            {
                list.Add(new Point(x, y));
            }
        }
        else
        {
            for (int y = fromY + 1; y <= toY; y++)
            {
                list.Add(new Point(x, y));
            }
        }
    }

    private void AddPointsHorizontally(List<Point> list, int fromX, int toX, int y)
    {
        if (fromX > toX)
        {
            for (int x = fromX - 1; x >= toX; x--)
            {
                list.Add(new Point(x, y));
            }
        }
        else
        {
            for (int x = fromX + 1; x <= toX; x++)
            {
                list.Add(new Point(x, y));
            }
        }
    }
}


class CaveSystemState
{
    public List<Amphipod> Amphipods = [];
    public List<Amphipod> AmphipodsInHallway = [];
    public Dictionary<Point, Amphipod> AmphipodByPosition = [];
    public Dictionary<char, RoomState> Rooms = [];

    public CaveSystemState(string input, CaveSystem cave)
    {
        var lines = input.Split(Environment.NewLine);
        
        foreach (var y in Enumerable.Range(0, lines.Length))
        {
            foreach (var x in Enumerable.Range(0, lines[y].Length))
            {
                char tile = lines[y][x];
                if (char.IsAsciiLetter(tile))
                {
                    Point position = new Point(x, y);
                    Amphipod amphipod = new Amphipod { Position = position, Type = tile };
                    Amphipods.Add(amphipod);
                    AmphipodByPosition[position] = amphipod;
                    if (y == CaveSystem.HallwayPositionY)
                    {
                        AmphipodsInHallway.Add(amphipod);
                    }
                }
            }
        }

        Rooms['A'] = MakeRoomState('A', CaveSystem.RoomAPositionX, cave.CaveSystemHeight);
        Rooms['B'] = MakeRoomState('B', CaveSystem.RoomBPositionX, cave.CaveSystemHeight);
        Rooms['C'] = MakeRoomState('C', CaveSystem.RoomCPositionX, cave.CaveSystemHeight);
        Rooms['D'] = MakeRoomState('D', CaveSystem.RoomDPositionX, cave.CaveSystemHeight);
    }

    private RoomState MakeRoomState(char roomType, int roomPositionX, int caveSystemHeight)
    {
        RoomState result = new RoomState { Welcoming = true, WelcomingPoint = null, NextToMoveOut = null };

        for(int y = caveSystemHeight - 2; y > CaveSystem.HallwayPositionY; y--)
        {
            if (AmphipodByPosition.TryGetValue(new Point(roomPositionX, y), out var amphipod))
            {
                if (result.Welcoming && amphipod.Type == roomType) continue;

                result.Welcoming = false;
                result.NextToMoveOut = amphipod;
            }
            else
            {
                result.WelcomingPoint = new Point(roomPositionX, y);
                break;
            }
        }

        return result;
    }

    public string MakeStringAfterMove(Amphipod toMove, Point moveTarget, CaveSystem cave)
    {
        string template = cave.CaveTemplate;
        char[][] lines = template.Split(Environment.NewLine).Select(s => s.ToCharArray()).ToArray();

        foreach(var amphipod in Amphipods)
        {
            Point point = (amphipod == toMove) ? moveTarget : amphipod.Position;
            lines[point.Y][point.X] = amphipod.Type;
        }

        return string.Join(Environment.NewLine, lines.Select(ch => new string(ch)));
    }

    public bool IsFinalState()
    {
        return Rooms.Values.All(r => r.Welcoming) && !AmphipodsInHallway.Any();
    }
}

class RoomState
{
    public bool Welcoming;
    public Point? WelcomingPoint;
    public Amphipod? NextToMoveOut;
}


class Amphipod
{
    public Point Position;
    public char Type;

    public long GetMoveEnergy(int steps)
    {
        return Type switch
        {
            'A' => 1 * steps,
            'B' => 10 * steps,
            'C' => 100 * steps,
            'D' => 1000 * steps,
            _ => throw new ArgumentException("Unknown amphipod type", nameof(Type)),
        };
    }
}

record struct Point(int X, int Y);
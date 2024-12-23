// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day21\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day21\Full.txt";
#endif

    string[] codesToEnter = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Solve(codesToEnter, 2));
    Console.WriteLine("Part 2: " + SolveFast(codesToEnter, 25));
    Console.ReadLine();
}

long Solve(string[] codesToEnter, int countDirectionalKeypads)
{
    Spaceship spaceship = new(countDirectionalKeypads);
    return codesToEnter.Sum(spaceship.GetCodeComplexity);
}

long SolveFast(string[] codesToEnter, int countDirectionalKeypads)
{
    // Just testing now during development
    Spaceship spaceship = new(2);
    FasterSpaceship fasterSpaceship = new(2);

    string testCode = codesToEnter.First();

    Console.WriteLine($"Classic : {spaceship.GetButtonPresses(testCode)}");
    Console.WriteLine($"Faster  : {fasterSpaceship.GetButtonPresses(testCode)}");
    return 0;

    // FasterSpaceship fasterSpaceship = new(countDirectionalKeypads);
    // return codesToEnter.Sum(fasterSpaceship.GetCodeComplexity);
}


// Part 2
// 203531410 too low
// 5369454759343 too low
// 1562056304574850 too high
// 
// 14531421692399 is wrong
// 91582698116141 is wrong
// 621377047148253 is wrong
// 1565783261018851 is wrong


class FasterSpaceship
{
    private int CountDirectionalKeypads;
    private int DigitalKeypadLevel;

    Dictionary<(char From, char To, int Level), long> CostCache = [];
    public FasterSpaceship(int countDirectionalKeypads)
    {
        CountDirectionalKeypads = countDirectionalKeypads;
        DigitalKeypadLevel = CountDirectionalKeypads + 1;
    }

    public long GetCodeComplexity(string code)
    {
        long number = Convert.ToInt64(code.Substring(0, code.Length - 1));
        long buttonPresses = GetButtonPresses(code);

        return number * buttonPresses;
    }

    public long GetButtonPresses(string targetCode)
    {
        return GetSequenceCost(targetCode, DigitalKeypadLevel);
    }

    private long GetSequenceCost(string sequence, int level)
    {
        var movements = ('A' + sequence).Zip(sequence);

        long cost = 0;

        foreach (var movement in movements)
        {
            cost += GetCost(movement.First, movement.Second, level);
            cost += 1; // pressing button
        }

        return cost;
    }

    private long GetCost(char from, char to, int level)
    {
        if (level == 0) return 0;

        if(CostCache.TryGetValue((from, to, level), out var cached))
        {
            return cached;
        }

        long result = 0;

        // TODO
        // Plan:
        // 1. Get all possible paths using tree search
        // 2. For each possible path, get the cost on the lower level
        //    by passing the actual path movements to GetSequenceCost with lower level
        // 3. Take the lowesst cost   

        CostCache.Add((from, to, level), result);

        return result;
    }



}

class Spaceship
{
    private RobotStack RobotStack;

    public Spaceship(int countDirectionalKeypads)
    {
        RobotStack = new(countDirectionalKeypads);
    }

    public long GetCodeComplexity(string code)
    {
        long number = Convert.ToInt64(code.Substring(0, code.Length - 1));
        long buttonPresses = GetButtonPresses(code);

        return number * buttonPresses;
    }

    public long GetButtonPresses(string targetCode)
    {
        // Console.WriteLine("Searching code " + targetCode);

        HashSet<string> seenCount = [];
        
        PriorityQueue<(RobotState[] RobotStates, int DigitsAlreadyFound), long> queue = new();
        queue.Enqueue((RobotStack.CreateInitialState(), 0), 0);

        while (queue.TryDequeue(out var current, out var presses))
        {
            if (current.DigitsAlreadyFound == targetCode.Length)
            {
                // Console.WriteLine("Returning " + presses);
                return presses;
            }

            string hash = RobotStack.GetStringForHash(current.RobotStates);
            if (seenCount.Contains(hash)) continue;
            seenCount.Add(hash);

            char exptectedSignal = targetCode[current.DigitsAlreadyFound];

            _ = CheckNext(Robot.PRESS)
                || CheckNext('>')
                || CheckNext('<')
                || CheckNext('^')
                || CheckNext('v');

            bool CheckNext(char startSignal)
            {
                var answer = RobotStack.ReceiveSignal(current.RobotStates, startSignal);

                if (answer.NewSignal == Robot.PANIC) return false;

                if (answer.NewSignal == Robot.MOVED)
                {
                    string hash = RobotStack.GetStringForHash(answer.NewState!);
                    if (seenCount.Contains(hash)) return false;

                    queue.Enqueue((answer.NewState!, current.DigitsAlreadyFound), presses + 1);
                    return false;
                }

                // An actual signal was pressed at the very last robot
                if(answer.NewSignal == exptectedSignal)
                {
                    // Console.WriteLine("Digit found " + exptectedSignal);
                    
                    queue.Clear();
                    seenCount.Clear();

                    queue.Enqueue((answer.NewState!, current.DigitsAlreadyFound + 1), presses + 1);
                    return true;
                }

                return false;
            }
        }

        return 0;
    }
}

record RobotState
{
    public char CurrentPosition = Robot.PRESS;
    public string VisitedButtons = "" + Robot.PRESS;
}

class Robot
{
    private Func<char, char, char> Movement;

    public Robot(Func<char, char, char> movement)
    {
        Movement = movement;
    }

    public (char NewPositon, char NewSignal) ReceiveSignal(char currentPosition, char signal)
    {
        if(signal == Robot.PRESS)
        {
            return (currentPosition, currentPosition);
        }
        else
        {
            return (Movement(currentPosition, signal), Robot.MOVED);
        }
    }

    public const char PANIC = 'X';
    public const char GAP = 'G';
    public const char PRESS = 'A';
    public const char MOVED = 'M';
}

class RobotStack
{
    Robot[] Robots;

    public RobotStack(int countDirectionalKeypads)
    {
        Robots = 
        [
            ..Enumerable.Range(0, countDirectionalKeypads).Select(_ => CreateDirectionalKeypadRobot()),
            CreateNumericKeypadRobot()
        ];
    }

    public (RobotState[]? NewState, char NewSignal) ReceiveSignal(RobotState[] oldState, char signal)
    {
        List<RobotState> newState = [..oldState];
        char updatedSignal = signal;

        for (int i = 0; i < Robots.Length; i++)
        {
            var robot = Robots[i];
            var currentOldState = oldState[i];

            (var newPosition, updatedSignal) = robot.ReceiveSignal(currentOldState.CurrentPosition, updatedSignal);

            // Any robot moved outside pad => panic
            if (newPosition == Robot.GAP) return (null, Robot.PANIC);

            // Robot moved and visited previously visited spot => panic
            if (updatedSignal == Robot.MOVED
                && currentOldState.VisitedButtons.Contains(newPosition))
            {
                return (null, Robot.PANIC);
            }

            // When robot just moved we update state and can then stop
            if (updatedSignal == Robot.MOVED)
            {
                // Update state
                newState[i] = new RobotState() 
                { 
                    CurrentPosition = newPosition, 
                    VisitedButtons = currentOldState.VisitedButtons + newPosition 
                };

                break;
            }
            else
            {
                // When robot did not move, we pressed button, so reset visited in state
                newState[i] = new RobotState()
                {
                    CurrentPosition = newPosition,
                    VisitedButtons = "" + newPosition
                };
            }
        }

        return ([..newState], updatedSignal);
    }

    public RobotState[] CreateInitialState() => 
        Enumerable.Range(0, Robots.Length)
        .Select(_ => new RobotState())
        .ToArray();

    public string GetStringForHash(RobotState[] robotStates)
    {
        return string.Join("", robotStates.Select(r => r.CurrentPosition));
    }

    private static Robot CreateNumericKeypadRobot() => new Robot(NumericKeypad);
    private static Robot CreateDirectionalKeypadRobot() => new Robot(DirectionalKeypad);

    private static char NumericKeypad(char currentPosition, char movement)
    {
        return (currentPosition, movement) switch
        {
            ('7', '>') => '8',
            ('7', 'v') => '4',
            
            ('8', '<') => '7',
            ('8', '>') => '9',
            ('8', 'v') => '5',
            
            ('9', '<') => '8',
            ('9', 'v') => '6',

            ('4', '^') => '7',
            ('4', '>') => '5',
            ('4', 'v') => '1',

            ('5', '<') => '4',
            ('5', '>') => '6',
            ('5', '^') => '8',
            ('5', 'v') => '2',

            ('6', '<') => '5',
            ('6', '^') => '9',
            ('6', 'v') => '3',
            
            ('1', '^') => '4',
            ('1', '>') => '2',

            ('2', '<') => '1',
            ('2', '>') => '3',
            ('2', '^') => '5',
            ('2', 'v') => '0',

            ('3', '<') => '2',
            ('3', '^') => '6',
            ('3', 'v') => 'A',

            ('0', '>') => 'A',
            ('0', '^') => '2',

            ('A', '<') => '0',
            ('A', '^') => '3',

            _ => Robot.GAP,
        };
    }
    
    private static char DirectionalKeypad(char currentPosition, char movement)
    {
        return (currentPosition, movement) switch
        {
            ('^', '>') => 'A',
            ('^', 'v') => 'v',

            ('A', '<') => '^',
            ('A', 'v') => '>',

            ('<', '>') => 'v',

            ('v', '<') => '<',
            ('v', '>') => '>',
            ('v', '^') => '^',

            ('>', '<') => 'v',
            ('>', '^') => 'A',

            _ => Robot.GAP,
        };
    }
}

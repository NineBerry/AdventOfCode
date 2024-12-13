// See "OtherAttempts.txt" for other coding attempts
// #define Sample

using Python.Runtime;
using System.Text;
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day13\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day13\Full.txt";
#endif

    var clawMachines  = 
        File
        .ReadAllText(fileName)
        .ReplaceLineEndings(Environment.NewLine)
        .Split(Environment.NewLine + Environment.NewLine)
        .Select(s => new ClawMachine(s))
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(clawMachines));
    Console.WriteLine("Part 2: " + Part2(clawMachines));
    Console.ReadLine();
}

long Part1(ClawMachine[] clawMachines)
{
    return PythonHelper.SolveClawMachinesWithPython(clawMachines);
}

long Part2(ClawMachine[] clawMachines)
{
    Point additional = new Point(10000000000000, 10000000000000);

    var biggerMachines = 
        clawMachines
        .Select(m => m with { PrizeAt = m.PrizeAt.AddVector(additional) })
        .ToArray();

    return PythonHelper.SolveClawMachinesWithPython(biggerMachines);
}

record ClawMachine
{
    public Point ButtonAIncrease;
    public Point ButtonBIncrease;
    public Point PrizeAt;
    
    public ClawMachine(string input)
    {
        int[] values = Regex.Matches(input, @"\d+").Select(m => int.Parse(m.Value)).ToArray();

        ButtonAIncrease = new(values[0], values[1]);
        ButtonBIncrease = new(values[2], values[3]);
        PrizeAt = new(values[4], values[5]);
    }
}

record Point(long X, long Y)
{
    public Point AddVector(Point vector)
    {
        return new Point(X + vector.X, Y + vector.Y);
    }
}

static class PythonHelper
{
    static bool initialized = false;

    public static long SolveClawMachinesWithPython(ClawMachine[] machines)
    {
        InitializePythonEngine();
        string code = CreatePythonScript(machines);
        long result = RunPythonScript(code);

        return result;
    }

    private static void InitializePythonEngine()
    {
        if (!initialized)
        {
            Runtime.PythonDLL = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python39_64\python39.dll";
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();
            initialized = true;
        }
    }

    private static string CreatePythonScript(ClawMachine[] machines)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(
            """
            from sympy import solve
            from sympy import symbols

            def findCheapestWin(aIncreaseX, aIncreaseY, bIncreaseX, bIncreaseY, targetX, targetY):
                a, b = symbols("a b")

                solution =  solve([(a * aIncreaseX + b * bIncreaseX - targetX), (a * aIncreaseY + b * bIncreaseY - targetY)])    
                valA = solution[a]
                valB = solution[b]

                if valA.is_integer and valB.is_integer:
                    return valA * 3 + valB

                return 0

            result = 0;
            """);
        
        sb.AppendLine();
        
        foreach(var m in machines)
        {
            sb.AppendLine($"result += findCheapestWin({m.ButtonAIncrease.X}, {m.ButtonAIncrease.Y}, {m.ButtonBIncrease.X}, {m.ButtonBIncrease.Y}, {m.PrizeAt.X}, {m.PrizeAt.Y})");
        }
        
        return sb.ToString();
    }

    private static long RunPythonScript(string code)
    {
        long result = 0;

        using (Py.GIL())
        {
            using (PyModule scope = Py.CreateScope())
            {
                scope.Exec(code);
                PyObject obj = scope.Get("result");
                result = long.Parse("" + obj);
            }
        }

        return result;
    }
}
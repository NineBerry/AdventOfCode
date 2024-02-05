// #define Sample

using Python.Runtime;
using System.Text;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day13\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day13\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    int earliestDeparture = int.Parse(lines[0]);
    int[] busIDs = 
        lines[1]
        .Split((char[])[',', 'x'], StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse)
        .ToArray();

    Console.WriteLine("Part 1: " + Part1(earliestDeparture, busIDs));

    var bussesAndOffsets =
        lines[1]
        .Split(',')
        .Select((s, i) => (Value: s, Index: i))
        .Where(s => s.Value != "x")
        .Select(s => (Bus: int.Parse(s.Value), s.Index))
        .ToArray();
    Console.WriteLine("Part 2: " + Part2(bussesAndOffsets));

    Console.ReadLine();
}

long Part1(int earliestDepartue, int[] busses)
{
    int departure = earliestDepartue;

    while (true)
    {
        foreach(var bus in busses)
        {
            if(departure % bus == 0)
            {
                return (departure - earliestDepartue) * bus;
            }
        }

        departure++;
    }
}

long Part2((int Bus, int Index)[] bussesAndOffsets)
{
    return PythonHelper.SolveChineseRemainderTheoremForBusses(bussesAndOffsets);
}

static class PythonHelper
{
    public static long SolveChineseRemainderTheoremForBusses((int Bus, int Index)[] bussesAndOffsets)
    {
        InitializePythonEngine();
        string code = CreatePythonScript(bussesAndOffsets);
        long result = RunPythonScript(code);

        return result;
    }

    private static void InitializePythonEngine()
    {
        Runtime.PythonDLL = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python39_64\python39.dll";
        PythonEngine.Initialize();
        PythonEngine.BeginAllowThreads();
    }

    private static string CreatePythonScript((int Bus, int Index)[] bussesAndOffsets)
    {
        StringBuilder sb = new StringBuilder();

        string modulos = string.Join(",", bussesAndOffsets.Select(x => x.Bus));
        string remainders = string.Join(",", bussesAndOffsets.Select(x => x.Index));

        sb.AppendLine("from sympy.ntheory.modular import crt");
        sb.AppendLine();
        sb.AppendLine($"solution = crt([{modulos}], [{remainders}])");
        sb.AppendLine();
        sb.AppendLine("result = solution[1] - solution[0]");
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
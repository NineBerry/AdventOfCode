// #define Sample

using System.Text.RegularExpressions;
using System.Text;
using System.Numerics;
using Python.Runtime;

using Vector = (System.Numerics.BigInteger X, System.Numerics.BigInteger Y, System.Numerics.BigInteger Z);
using Line = ((System.Numerics.BigInteger X, System.Numerics.BigInteger Y, System.Numerics.BigInteger Z) Point, 
    (System.Numerics.BigInteger X, System.Numerics.BigInteger Y, System.Numerics.BigInteger Z) Vector);


{
#if Sample
    string gFileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day24\Sample.txt";
    BigInteger gMin = 7;
    BigInteger gMax = 27;
#else
    string gFileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day24\Full.txt";
    BigInteger gMin = 200000000000000;
    BigInteger gMax = 400000000000000;
#endif

    string[] input = File.ReadAllLines(gFileName);

    Line[] lines = input.Select(ParseLine).ToArray();

    Console.WriteLine("Part 1: " + Part1(lines, gMin, gMax));
    Console.WriteLine("Part 2: " + Part2(lines));
    Console.ReadLine();
}

Line ParseLine(string s)
{
    var numbers = Regex.Matches(s, "-?[0-9]+").Select(m => BigInteger.Parse(m.Value)).ToArray();
    return (new Vector(numbers[0], numbers[1], numbers[2]), new Vector(numbers[3], numbers[4], numbers[5]));
}


long Part1(Line[] lines, BigInteger gMin, BigInteger gMax)
{
    lines = lines.Select(l => (l.Point with { Z = 0 }, l.Vector with { Z = 0 })).ToArray();
    
    List<Vector> intersections = new();

    for(int i=0; i < lines.Length; i++)
    {
        for (int j=i+1; j < lines.Length; j++)
        {
            var line1 = lines[i];
            var line2 = lines[j];

            var possibleIntersection = Intersection(line1, line2) ;
            if (possibleIntersection is Vector intersection)
            {
                if(IsVectorInRange(intersection, gMin, gMax)
                    && IsPointInFutureOfLine(intersection, line1)
                    && IsPointInFutureOfLine(intersection, line2))
                {
                    intersections.Add(intersection);
                }
            }
        }
    }

    return intersections.Count;
}
Vector? Intersection(Line line1, Line line2)
{
    // Turn Point + Vector into Point + Point
    Vector a1 = line1.Point;
    Vector a2 = line1.Point with { X = line1.Point.X + line1.Vector.X, Y = line1.Point.Y + line1.Vector.Y, Z = line1.Point.Z + line1.Vector.Z };
    Vector b1 = line2.Point;
    Vector b2 = line2.Point with { X = line2.Point.X + line2.Vector.X, Y = line2.Point.Y + line2.Vector.Y, Z = line2.Point.Z + line2.Vector.Z };

    // Taken from https://stackoverflow.com/a/67964224/101087

    // determinant
    BigInteger d = (a1.X - a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X - b2.X);

    // check if lines are parallel
    if (d == 0) return null;

    BigInteger px = (a1.X * a2.Y - a1.Y * a2.X) * (b1.X - b2.X) - (a1.X - a2.X) * (b1.X * b2.Y - b1.Y * b2.X);
    BigInteger py = (a1.X * a2.Y - a1.Y * a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X * b2.Y - b1.Y * b2.X);

    // TODO: Calculate Z and verify the two lines also intersect at Z.
    // We can skip that here because we ignore Z in part 1 and
    // don't use the function for part 2.

    Vector hitPoint = (px / d, py / d, 0);
    return hitPoint;
}

bool IsVectorInRange(Vector point, BigInteger min, BigInteger max)
{
    return (point.X >= min) && (point.X <= max) && (point.Y >= min) && (point.Y <= max);
}

bool IsPointInFutureOfLine(Vector point, Line line)
{
    return IsCoordinateInFutureOfLineCoordinate(point.X, line.Point.X, line.Vector.X)
        && IsCoordinateInFutureOfLineCoordinate(point.Y, line.Point.Y, line.Vector.Y)
        && IsCoordinateInFutureOfLineCoordinate(point.Z, line.Point.Z, line.Vector.Z);
}

bool IsCoordinateInFutureOfLineCoordinate(BigInteger pointCoordinate, BigInteger lineCoordinate, BigInteger vectorCoorindate)
{
    int pointSign = (pointCoordinate - lineCoordinate).Sign;
    int vectorSign = vectorCoorindate.Sign;

    return pointSign == vectorSign;
}

long Part2(Line[] lines)
{
    InitializePythonEngine();

    string code = CreatePythonScript(lines, 5);
    long result = RunPythonScript(code);

    return result;
}

string CreatePythonScript(Line[] lines, int maxToUse)
{
    StringBuilder sb = new StringBuilder();

    sb.AppendLine("from sympy import Symbol");
    sb.AppendLine("from sympy.solvers import solve");
    sb.AppendLine();
    sb.AppendLine("px = Symbol('px')");
    sb.AppendLine("py = Symbol('py')");
    sb.AppendLine("pz = Symbol('pz')");
    sb.AppendLine("vx = Symbol('vx')");
    sb.AppendLine("vy = Symbol('vy')");
    sb.AppendLine("vz = Symbol('vz')");
    for (int i = 1; i <= maxToUse; i++)
    {
        sb.AppendLine($"t{i} = Symbol('t{i}')");
    }

    sb.AppendLine();
    sb.AppendLine("s = []");

    for (int i = 1; i <= maxToUse; i++)
    {
        Line line = lines[i - 1];
        sb.AppendLine(
            $"s += [{line.Point.X} + {line.Vector.X} * t{i} - (px+vx*t{i}), " +
            $"      {line.Point.Y} + {line.Vector.Y} * t{i} - (py+vy*t{i}), " +
            $"      {line.Point.Z} + {line.Vector.Z} * t{i} - (pz+vz*t{i})]");
    }

    sb.AppendLine();
    sb.AppendLine("q = solve(s)");
    sb.AppendLine("result = q[0][px] + q[0][py] + q[0][pz]");

    string code = sb.ToString();
    return code;
}

long RunPythonScript(string code)
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

void InitializePythonEngine()
{
    Runtime.PythonDLL = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python39_64\python39.dll";
    PythonEngine.Initialize();
    PythonEngine.BeginAllowThreads();
}
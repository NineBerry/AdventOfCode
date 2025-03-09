#define Sample

using System.ComponentModel;
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day19\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day19\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Universe universe = new Universe(input);
    universe.Unify();

    Console.WriteLine("Part 1: " + Part1(universe));
    // Console.WriteLine("Part 2: " + Part2());
    Console.ReadLine();
}

long Part1(Universe universe)
{
    return -1;
}

class Universe
{
    Scanner[] Scanners;

    public Universe(string input)
    {
        Scanners = input.ReplaceLineEndings(Environment.NewLine).Split(Environment.NewLine + Environment.NewLine).Select(s => new Scanner(s)).ToArray();

    }

    public void Unify()
    {
        for (int i = 0; i < Scanners.Length; i++)
        {
            for (int j = i + 1; j < Scanners.Length; j++)
            {
                Scanners[i].TryMatchWith(Scanners[j]);
            }
        }
    }
}

class Scanner
{
    public int Number;
    public Point[] OriginalPoints;
    public DistanceInformation[] DistanceInformation;
    private Dictionary<int, FormulaComponent[]> Formulas = [];

    public Scanner(string input)
    {
        var lines = input.Split(Environment.NewLine);
        Number = lines[0].ExtractNumbers()[0];
        OriginalPoints = lines.Skip(1).Select(Point.Parse).ToArray();

        List<DistanceInformation> distanceInformationList = [];
        for (int i = 0; i < OriginalPoints.Length; i++)
        {
            for (int j = i + 1; j < OriginalPoints.Length; j++)
            {
                distanceInformationList.Add(new DistanceInformation(OriginalPoints[i], OriginalPoints[j]));
            }
        }

        DistanceInformation = distanceInformationList.ToArray();
    }

    internal void TryMatchWith(Scanner otherScanner)
    {
        foreach (var point in OriginalPoints)
        {
            if (TryMatchWith(point, otherScanner)) break;
        }
    }

    private bool TryMatchWith(Point point, Scanner otherScanner)
    {
        var thisCanonicals = DistanceInformation.Where(d => d.FirstPoint == point || d.SecondPoint == point).Select(d => d.CanonicalForm).ToHashSet();
        var matching = otherScanner.DistanceInformation.Where(d => thisCanonicals.Contains(d.CanonicalForm)).ToArray();

        var points = matching.SelectMany(d => new[] { d.FirstPoint, d.SecondPoint }).ToArray();
        var elevenerPoints = points.GroupBy(p => p).Where(g => g.Count() >= 11).ToArray();

        if (elevenerPoints.Any())
        {
            Point matchingPoint = elevenerPoints.Single().First();
            // Console.WriteLine($"{point} matches {matchingPoint}");


            // Find first difference with unique components
            DistanceInformation otherUniqueComonentsDifference = matching.First(d => d.HasUniqueComponents);
            DistanceInformation thisUniqueComonentsDifference = 
                DistanceInformation
                .Where(d => (d.FirstPoint == point || d.SecondPoint == point) && d.CanonicalForm == otherUniqueComonentsDifference.CanonicalForm)
                .Single();


            // TODO: Get Translation information in both directions
            /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            /// And then store in both scanners


            // Then calculate the formula between the two orientations
            FormulaComponent[] orientationFormula = CalculateOrientationFormula(otherUniqueComonentsDifference.OriginalDistance, thisUniqueComonentsDifference.OriginalDistance);

            // Then calculate the formula between the two scanners

            Point otherPointRelativeToThis = ApplyOrientationFormula(matchingPoint, orientationFormula);
            // Console.WriteLine($"Other point relative to this: {otherPointRelativeToThis}");


            

            if ((otherUniqueComonentsDifference.FirstPoint == matchingPoint && thisUniqueComonentsDifference.SecondPoint != point) ||
                (otherUniqueComonentsDifference.SecondPoint == matchingPoint && thisUniqueComonentsDifference.FirstPoint != point))
            {
                otherPointRelativeToThis = otherPointRelativeToThis.MulMinus1();
            }
            
            Point otherScannerRelativeToThis = point.Add(otherPointRelativeToThis);
            Console.WriteLine($"{otherUniqueComonentsDifference.CanonicalForm} Other Scanner {otherScanner.Number} relative to this {this.Number}: {otherScannerRelativeToThis}");

            // Then store formular in scanner and return true

            return true;
        }

        return false;
    }


    private FormulaComponent[] CalculateOrientationFormula(Point from, Point to)
    {
        FormulaComponent[] result = new FormulaComponent[3];
        result[0] = FindComponent(to.X);
        result[1] = FindComponent(to.Y);
        result[2] = FindComponent(to.Z);

        return result;

        FormulaComponent FindComponent(int value)
        {
            if(Math.Abs(value) == Math.Abs(from.X)) return new FormulaComponent(Dimension.X, value / from.X);
            if (Math.Abs(value) == Math.Abs(from.Y)) return new FormulaComponent(Dimension.Y, value / from.Y);
            if (Math.Abs(value) == Math.Abs(from.Z)) return new FormulaComponent(Dimension.Z, value / from.Z);

            throw new Exception("Could not find component");
        }
    }

    private Point ApplyOrientationFormula(Point from, FormulaComponent[] formula)
    {
        int x = Apply(from, formula[0]);
        int y = Apply(from, formula[1]);
        int z = Apply(from, formula[2]);

        return new Point(x, y, z);

        int Apply(Point from, FormulaComponent formula)
        {
            int value = formula.SourceDimension switch
            {
                Dimension.X => from.X,
                Dimension.Y => from.Y,
                Dimension.Z => from.Z,
            };

            return value * formula.Sign;
        }
    }

}

enum Dimension
{
    X,
    Y,
    Z
}

record FormulaComponent
{
    public readonly Dimension SourceDimension;
    public readonly int Sign;

    public FormulaComponent(Dimension sourceDimension, int sign)
    {
        SourceDimension = sourceDimension;
        Sign = sign;
    }
}

record DistanceInformation
{
    public readonly Point FirstPoint;
    public readonly Point SecondPoint;
    public readonly Point OriginalDistance;
    public readonly bool HasUniqueComponents;
    public readonly string CanonicalForm;

    public DistanceInformation(Point firstPoint, Point secondPoint)
    {
        FirstPoint = firstPoint;
        SecondPoint = secondPoint;
        OriginalDistance = firstPoint.GetVectorTo(secondPoint);
        HasUniqueComponents = 
            (Math.Abs(OriginalDistance.X) != Math.Abs(OriginalDistance.Y)) 
            && (Math.Abs(OriginalDistance.Y) != Math.Abs(OriginalDistance.Z)) 
            && (Math.Abs(OriginalDistance.X) != Math.Abs(OriginalDistance.Z))
            && OriginalDistance.X != 0
            && OriginalDistance.Y != 0
            && OriginalDistance.Z != 0;



        int[] components = [Math.Abs(OriginalDistance.X), Math.Abs(OriginalDistance.Y), Math.Abs(OriginalDistance.Z)];
        CanonicalForm = string.Join(",", components.Order());
    }
}

record class Point
{
    public int X;
    public int Y;
    public int Z;

    public static Point Parse(string input)
    {
        var numbers = input.ExtractNumbers(); 
        return new Point(numbers[0], numbers[1], numbers[2]);
    }

    public Point(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Point GetVectorTo(Point to)
    {
        int x = to.X - X;
        int y = to.Y - Y;
        int z = to.Z - Z;

        return new Point(x, y, z);
    }

    public Point MulMinus1()
    {
        return new Point(-X, -Y, -Z);
    }
    public Point Add(Point p)
    {
        return new Point(X + p.X, Y + p.Y, Z + p.Z);
    }
}

public static class Tools
{
    public static int[] ExtractNumbers(this string input)
    {
        return Regex.Matches(input, @"-?\d+").Select(m => int.Parse(m.Value)).ToArray();
    }
}
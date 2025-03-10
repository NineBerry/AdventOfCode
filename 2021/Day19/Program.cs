// #define Sample

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

    Console.WriteLine("Part 1: " + universe.GetAllBeaconsCount());
    Console.WriteLine("Part 2: " + universe.GetMaxScannerDistance());
    Console.ReadLine();
}

class Universe
{
    Scanner[] Scanners;

    public Universe(string input)
    {
        Scanners = 
            input
            .ReplaceLineEndings(Environment.NewLine)
            .Split(Environment.NewLine + Environment.NewLine)
            .Select(s => new Scanner(s))
            .ToArray();
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

    public Point TranslateToZero(Point point, int fromScannerNumber)
    {
        Point? found = TranslateRecursive(point, fromScannerNumber, []);

        if (found == null) throw new Exception("Could not translate");

        return found;
    }

    private Point? TranslateRecursive(Point point, int fromScannerNumber, int[] seen)
    {
        if (fromScannerNumber == 0) return point;
        if (seen.Contains(fromScannerNumber)) return null;

        var fromScanner = Scanners.Where(s => s.Number == fromScannerNumber).Single();

        foreach (var targetScanner in fromScanner.Translations.Keys)
        {
            var currentPoint = fromScanner.Translate(point, targetScanner);
            var result = TranslateRecursive(currentPoint, targetScanner, [.. seen, fromScannerNumber]);

            if (result != null) return result;
        }

        return null;
    }


    public long GetAllBeaconsCount()
    {
        HashSet<Point> beacons = [];

        foreach (var scanner in Scanners)
        {
            foreach (var beacon in scanner.OriginalPoints)
            {
                beacons.Add(TranslateToZero(beacon, scanner.Number));
            }
        }

        return beacons.Count;
    }

    public long GetMaxScannerDistance()
    {
        Point[] allScanners = Scanners.Select(s => TranslateToZero(new Point(0,0,0), s.Number)).ToArray();

        long maxDistance = 0;

        foreach(var scanner in allScanners)
        {
            foreach (var scanner2 in allScanners)
            {
                maxDistance = Math.Max(maxDistance, scanner.GetManhattanDistance(scanner2));
            }
        }

        return maxDistance;
    }
}

class Scanner
{
    public int Number;
    public Point[] OriginalPoints;
    public DistanceInformation[] DistanceInformation;
    public Dictionary<int, TranslationsInformation> Translations = [];

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

    public void TryMatchWith(Scanner otherScanner)
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

            // Find first difference with unique components
            DistanceInformation otherUniqueComonentsDifference = matching.First(d => d.HasUniqueComponents);
            DistanceInformation thisUniqueComonentsDifference =
                DistanceInformation
                .Where(d => (d.FirstPoint == point || d.SecondPoint == point) && d.CanonicalForm == otherUniqueComonentsDifference.CanonicalForm)
                .Single();


            if (matching.Count(d => d.CanonicalForm == otherUniqueComonentsDifference.CanonicalForm) > 1) throw new Exception("Not unique");
            if (DistanceInformation.Count(d => d.CanonicalForm == otherUniqueComonentsDifference.CanonicalForm) > 1) throw new Exception("Not unique");

            var translation = CalculateTranslationInformation(this, otherScanner, point, matchingPoint, thisUniqueComonentsDifference, otherUniqueComonentsDifference);
            var reverseTranslation = CalculateTranslationInformation(otherScanner, this, matchingPoint, point, otherUniqueComonentsDifference, thisUniqueComonentsDifference);

            // Then store translation information in scanner 
            Translations[otherScanner.Number] = reverseTranslation;
            otherScanner.Translations[Number] = translation;

            return true;
        }

        return false;
    }

    private TranslationsInformation CalculateTranslationInformation(Scanner thisScanner, Scanner otherScanner, Point point, Point matchingPoint, DistanceInformation thisUniqueComonentsDifference, DistanceInformation otherUniqueComonentsDifference)
    {
        bool reverseOrder = false;
        if ((otherUniqueComonentsDifference.FirstPoint == matchingPoint && thisUniqueComonentsDifference.SecondPoint != point) 
            || (otherUniqueComonentsDifference.SecondPoint == matchingPoint && thisUniqueComonentsDifference.FirstPoint != point))
        {
            reverseOrder = true;
        }

        // Then calculate the formula between the two orientations
        FormulaComponent[] orientationFormula = CalculateOrientationFormula(otherUniqueComonentsDifference.OriginalDistance, thisUniqueComonentsDifference.OriginalDistance, reverseOrder);

        // Then calculate the formula between the two scanners
        Point otherPointRelativeToThis = ApplyOrientationFormula(matchingPoint, orientationFormula);
        Point otherScannerRelativeToThis = point.Add(otherPointRelativeToThis);

        return new TranslationsInformation(otherScannerRelativeToThis, orientationFormula);
    }

    private FormulaComponent[] CalculateOrientationFormula(Point from, Point to, bool reverseOrder)
    {
        int factor = reverseOrder ? -1 : 1;
        FormulaComponent[] result = new FormulaComponent[3];
        result[0] = FindComponent(to.X);
        result[1] = FindComponent(to.Y);
        result[2] = FindComponent(to.Z);

        return result;

        FormulaComponent FindComponent(int value)
        {
            if (Math.Abs(value) == Math.Abs(from.X)) return new FormulaComponent(Dimension.X, factor * value / from.X);
            if (Math.Abs(value) == Math.Abs(from.Y)) return new FormulaComponent(Dimension.Y, factor * value / from.Y);
            if (Math.Abs(value) == Math.Abs(from.Z)) return new FormulaComponent(Dimension.Z, factor * value / from.Z);

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
                _ => throw new NotImplementedException(),
            };

            return value * formula.Sign;
        }
    }

    public Point Translate(Point point, int targetScanner)
    {
        Point oPoint = point;

        var translation = Translations[targetScanner];

        point = ApplyOrientationFormula(point, translation.Formula);
        point = point.MulMinus1();
        point = point.Add(translation.BasePoint);

        return point;
    }
}

enum Dimension { X, Y, Z }

record TranslationsInformation
{
    public readonly Point BasePoint;
    public readonly FormulaComponent[] Formula;

    public TranslationsInformation(Point basePoint, FormulaComponent[] formula)
    {
        BasePoint = basePoint;
        Formula = formula;
    }
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

    public long GetManhattanDistance(Point scanner2)
    {
        return Math.Abs(X - scanner2.X) + Math.Abs(Y - scanner2.Y) + Math.Abs(Z - scanner2.Z);
    }
}

public static class Tools
{
    public static int[] ExtractNumbers(this string input)
    {
        return Regex.Matches(input, @"-?\d+").Select(m => int.Parse(m.Value)).ToArray();
    }
}
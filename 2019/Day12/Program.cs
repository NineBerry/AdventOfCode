// #define Sample

using System.Text.RegularExpressions;
using System.Xml.Linq;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day12\Sample.txt";
    int part1Steps = 100;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day12\Full.txt";
    int part1Steps = 1000;
#endif

    Space space = new Space(File.ReadAllLines(fileName));
    Console.WriteLine("Part 1: " + Part1(space, part1Steps));
    Console.WriteLine("Part 2: " + Part2(space));
    Console.ReadLine();
}

long Part1(Space space, int steps)
{
    foreach(var _ in Enumerable.Range(1, steps))
    {
        space.Step();
    }

    return space.TotalEnergy;
}

long Part2(Space space)
{
    var cycleLengths = space.FindCoordinateCycleLengths();
    return cycleLengths.Aggregate(LCM);

    long LCM(long a, long b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }

    long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }
}


public class Space
{
    public Space(string[] input)
    {
        Moons = input.Select(x => new Moon(x)).ToArray();
        OriginalMoons = input.Select(x => new Moon(x)).ToArray();
        DetectedCycles = [0, 0, 0];
    }

    public void Step()
    {
        HandleCycleDetection();

        ApplyGravity();
        ApplyVelocity();

        StepsDone++;
    }

    private void HandleCycleDetection()
    {
        if(StepsDone == 0) return;

        HandleCycleDetectionForCoordinate(0, c => c.X);
        HandleCycleDetectionForCoordinate(1, c => c.Y);
        HandleCycleDetectionForCoordinate(2, c => c.Z);
    }

    private void HandleCycleDetectionForCoordinate(int index, Func<Coordinate3D, long> coordCallback)
    {
        if (DetectedCycles[index] == 0)
        {
            bool allSame = true;
            for (int i = 0; i < Moons.Length; i++)
            {
                Moon current = Moons[i];
                Moon original = OriginalMoons[i];

                if ((coordCallback(current.Position), coordCallback(current.Velocity)) != 
                    (coordCallback(original.Position), coordCallback((original.Velocity))))
                {
                    allSame = false;
                    break;
                }
            }

            if (allSame)
            {
                DetectedCycles[index] = StepsDone;
            }
        }
    }

    public void ApplyGravity()
    {
        for(int m1 = 0; m1 < Moons.Length - 1; m1++)
        {
            for (int m2 = m1 + 1; m2 < Moons.Length; m2++)
            {
                Moon moon1 = Moons[m1];
                Moon moon2 = Moons[m2];
                Moon.ApplyGravity(moon1, moon2);
            }
        }
    }

    public void ApplyVelocity()
    {
        foreach (var moon in Moons)
        {
            moon.ApplyVelocity();
        }
    }

    public long[] FindCoordinateCycleLengths()
    {
        while (true) 
        {
            if(DetectedCycles.All(i => i != 0))
            {
                return DetectedCycles;
            }

            Step();
        }
    }

    private long StepsDone = 0;
    private Moon[] Moons;
    private Moon[] OriginalMoons;

    private long[] DetectedCycles;
    
    public long TotalEnergy => Moons.Sum(m => m.TotalEnergy);
}

public record class Moon
{
    public Moon(string input)
    {
        int[] values = Regex.Matches(input, "-?\\d+").Select(m => int.Parse(m.Value)).ToArray();
        Position = new Coordinate3D(values[0], values[1], values[2]);
        Velocity = new Coordinate3D();
    }

    public void ApplyVelocity()
    {
        Position = Position.Add(Velocity);
    }

    private long ApplyGravityToCoordinate(long my, long other)
    {
        return Math.Sign(other - my);
    }

    public void ApplyGravity(Moon other)
    {
        Velocity.X += ApplyGravityToCoordinate(Position.X, other.Position.X);
        Velocity.Y += ApplyGravityToCoordinate(Position.Y, other.Position.Y);
        Velocity.Z += ApplyGravityToCoordinate(Position.Z, other.Position.Z);
    }

    public static void ApplyGravity(Moon m1, Moon m2)
    {
        m1.ApplyGravity(m2);
        m2.ApplyGravity(m1);
    }

    public Coordinate3D Position;
    public Coordinate3D Velocity;

    public long PotentialEnergy => Position.AbsoluteSum;
    public long KineticEnergy => Velocity.AbsoluteSum;
    public long TotalEnergy => PotentialEnergy * KineticEnergy;
}

public record struct Coordinate3D
{
    public Coordinate3D(long x, long y, long z)
    {
        X = x;
        Y = y; 
        Z = z;
    }

    public Coordinate3D Add(Coordinate3D other)
    {
        return new Coordinate3D(X + other.X, Y + other.Y, Z + other.Z);
    }

    public long X;
    public long Y;
    public long Z;

    public long AbsoluteSum => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
}
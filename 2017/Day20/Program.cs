// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day20\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day20\Full.txt";
#endif

    var particles = 
        File.ReadAllLines(fileName)
        .Select((s, i) => new Particle(s, i))
        .ToList();
    Console.WriteLine("Part 1: " + Part1(particles));

    particles =
        File.ReadAllLines(fileName)
        .Select((s, i) => new Particle(s, i))
        .ToList();
    Console.WriteLine("Part 2: " + Part2(particles));
    Console.ReadLine();
}

long Part1(List<Particle> particles)
{
    while(particles.Any(p => !p.IsAligned()))
    {
        foreach(Particle p in particles)
        {
            p.Tick();
        }
    }

    return particles.OrderBy(p => p.Position.ManhattanDistanceToZero).First().ID;
}

long Part2(List<Particle> particles)
{
    while (particles.Any(p => !p.IsAligned()))
    {
        foreach (Particle p in particles)
        {
            p.Tick();
        }

        // Remove collisions
        var collisionParticles = 
            particles
            .GroupBy(p => p.Position)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g.Select(p => p))
            .ToArray();

        foreach (var p in collisionParticles)
        {
            particles.Remove(p);
        }
    }

    return particles.Count;
}

public record Vector3D
{
    public long X;
    public long Y;
    public long Z;  

    public Vector3D(long x, long y, long z)
    {
        X = x; Y = y; Z = z;
    }

    public void AddBy(Vector3D other)
    {
        X = X + other.X;
        Y = Y + other.Y;
        Z = Z + other.Z;
    }
    
    public long ManhattanDistanceToZero => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z); 
}

public record Particle
{
    public Particle(string input, int id)
    {
        ID = id;
        
        long[] values = 
            Regex.Matches(input, "-?\\d+")
            .Select(m => long.Parse(m.Value))
            .ToArray();
        
        Position = new Vector3D(values[0], values[1], values[2]);
        Velocity = new Vector3D(values[3], values[4], values[5]);
        Acceleration = new Vector3D(values[6], values[7], values[8]);
    }

    public int ID;
    public Vector3D Position;
    public Vector3D Velocity;
    public Vector3D Acceleration;

    public void Tick()
    {
        Velocity.AddBy(Acceleration);
        Position.AddBy(Velocity);
    }

    public bool IsAligned()
    {
        return CoordinateAligned([Position.X, Velocity.X, Acceleration.X])
            && CoordinateAligned([Position.Y, Velocity.Y, Acceleration.Y])
            && CoordinateAligned([Position.Z, Velocity.Z, Acceleration.Z]);
    }

    private static bool CoordinateAligned(long[] values)
    {
        HashSet<int> signs = values.Select(Math.Sign).ToHashSet();
        signs.Remove(0);

        return signs.Count <= 1;
    }
}
// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day08\Sample.txt";
    int closestPairsToConnect = 10;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day08\Full.txt";
    int closestPairsToConnect = 1000;
#endif

    JunctionBox[] boxes = 
        File.ReadAllLines(fileName)
        .Select(JunctionBox.Parse)
        .ToArray();

    Network network = Network.BuildNetwork(boxes, closestPairsToConnect);

    Console.WriteLine("Part 1: " + Part1(network));
    Console.WriteLine("Part 2: " + Part2(network));
    Console.ReadLine();
}

long Part1(Network network)
{
    return 
        network.GetCircuitSizes()
        .OrderDescending()
        .Take(3)
        .Aggregate((a, b) => a * b);
}

long Part2(Network network)
{
    (JunctionBox a, JunctionBox b) = network.FindLastMissingLink();
    return a.X * b.X;
}


public class Network
{
    private JunctionBox[] boxes = [];
    private Dictionary<(int, int), double> distances = [];
    private Dictionary<int, HashSet<int>> connections = [];

    public static Network BuildNetwork(JunctionBox[] boxes, int closestPairsToConnect)
    {
        Network network = new Network();

        network.boxes = boxes;
        network.FindDistances();
        network.ConnectClosesBoxes(closestPairsToConnect);

        return network;
    }

    private void ConnectClosesBoxes(int closestPairsToConnect)
    {
        var closest = distances.OrderBy(pair => pair.Value).Take(closestPairsToConnect);

        foreach (var pair in closest)
        {
            AddEdge(pair.Key.Item1, pair.Key.Item2);
        }
    }

    private void FindDistances()
    {
        for(int i=0; i < boxes.Length - 1; i++)
        {
            for (int j = i + 1; j < boxes.Length; j++)
            {
                var boxA = boxes[i];
                var boxB = boxes[j];
                
                double distance = boxA.CalclulateDistance(boxB);

                distances[MakeOrderedTuple(boxA.ID, boxB.ID)] = distance;
            }
        }
    }

    public void AddEdge(int first, int second)
    {
        Add(first, second);
        Add(second, first);

        void Add(int s1, int s2)
        {
            if (!connections.TryGetValue(s1, out var list))
            {
                list = new();
                connections[s1] = list;
            }

            list.Add(s2);
        }
    }

    public HashSet<int> GetDirectConnectedBoxes(int box)
    {
        if(connections.TryGetValue(box, out var result))
        {
            return result;
        }

        return [];
    }

    private (int, int) MakeOrderedTuple(int a, int b)
    {
        return a < b ? (a, b) : (b, a);
    }

    internal long[] GetCircuitSizes()
    {
        HashSet<int> ungroupedBoxes = boxes.Select(box => box.ID).ToHashSet();
        List<long> circuitSizes = [];

        while (ungroupedBoxes.Any())
        {
            int seed = ungroupedBoxes.First();
            HashSet<int> connected = CreateCircuit(seed, ungroupedBoxes);

            circuitSizes.Add(connected.Count);
            ungroupedBoxes.ExceptWith(connected);
        }

        return circuitSizes.ToArray();
    }

    private HashSet<int> CreateCircuit(int seed, HashSet<int> ungroupedBoxes)
    {
        HashSet<int> result = [seed];
        {
            Queue<int> queue = new Queue<int>();  
            queue.Enqueue(seed);

            while(queue.TryDequeue(out var current))
            {
                foreach(var connected in GetDirectConnectedBoxes(current))
                {
                    if (!result.Contains(connected))
                    {
                        result.Add(connected);
                        queue.Enqueue(connected);
                    }
                }
            }
        }

        return result;
    }

    public (JunctionBox a, JunctionBox b) FindLastMissingLink()
    {
        // First try naive brute forcing strategy. Add as long as there are still multiple circuits
        foreach(var pair in distances.OrderBy(pair => pair.Value).Select(pair => pair.Key))
        {
            AddEdge(pair.Item1, pair.Item2);

            if(GetCircuitSizes().Length == 1)
            {
                return (FindByID(pair.Item1), FindByID(pair.Item2));
            }
        }

        throw new Exception("Could not connect all boxes");

        // Naive approach works fast enough to deliver result in < 1s
        // TODO: 
        // Improvement would be to use iterative approach, working with already existing
        // circuits when we add a new connection and not always try to find circuits from
        // a clean slate.
        // This can obviously then be combined with part 1 also
    }

    private JunctionBox FindByID(int id)
    {
        return boxes.First(box => box.ID == id);
    }
}

public class JunctionBox
{
    public int ID;

    public long X;
    public long Y;
    public long Z;

    public static JunctionBox Parse(string s, int id)
    {
        long[] values = s.Split(',').Select(long.Parse).ToArray();
        return new JunctionBox
        {
            ID = id,
            X = values[0],
            Y = values[1],
            Z = values[2]
        };
    }

    public double CalclulateDistance(JunctionBox other)
    {
        double dx = other.X - this.X;
        double dy = other.Y - this.Y;
        double dz = other.Z - this.Z;

        double distanceSquared = (dx * dx) + (dy * dy) + (dz * dz);

        return Math.Sqrt(distanceSquared);
    }
}
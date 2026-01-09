// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day05\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day05\Full.txt";
#endif

    TunnelSystem system = new(File.ReadAllText(fileName));
    long steps = system.Traverse(powered: false);
    
    Console.WriteLine("Part 1: " + steps);
    Console.WriteLine("Part 2: " + system.NotVisited());
    Console.WriteLine("Part 3: " + system.Traverse(powered: true));

    Console.ReadLine();
}

class TunnelSystem
{
    private readonly string Layout;
    private HashSet<char> Visited = [];

    private Dictionary<char, Tunnel> Tunnels = [];

    public TunnelSystem(string input)
    {
        Layout = input;
        InitTunnels();
    }

    private void InitTunnels()
    {
        foreach((int index, char name) in Layout.Index())
        {
            if(Tunnels.TryGetValue(name, out var tunnel))
            {
                tunnel.LaterEntrance = index;
            }
            else
            {
                tunnel = new Tunnel { Name = name, EarlierEntrance = index};
                Tunnels.Add(name, tunnel);
            }
        }
    }
    public long Traverse(bool powered)
    {
        long sum = 0;

        int position = 0;

        while (position < Layout.Length)
        {
            char name = Layout[position];
            Tunnel tunnel = Tunnels[name];
            
            int sign = 1;
            
            if(powered && char.IsAsciiLetterUpper(tunnel.Name))
            {
                sign = -1;
            }

            sum += sign * tunnel.Length;
            
            Visited.Add(name);
            position = tunnel.Traverse(position) + 1;
        }

        return sum;
    }

    public string NotVisited()
    {
        return new string(Layout.Where(ch => !Visited.Contains(ch)).ToArray().Distinct().ToArray());
    }
}

class Tunnel
{
    public char Name;
    public int EarlierEntrance;
    public int LaterEntrance;

    public int Length => Math.Abs(LaterEntrance - EarlierEntrance);

    public int Traverse(int entrance)
    {
        if (entrance == EarlierEntrance) return LaterEntrance;
        if (entrance == LaterEntrance) return EarlierEntrance;

        throw new ArgumentException();
    }
}
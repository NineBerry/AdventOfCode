{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day23\Full.txt";

    long[] program = ReadProgram(fileName);
    Network network = new Network(program);
    network.Run();

    Console.WriteLine("Part 1: " + network.First255Received);
    Console.WriteLine("Part 2: " + network.First255SentTwice);

    Console.ReadLine();
}

static long[] ReadProgram(string fileNamePart2)
{
    return File.ReadAllText(fileNamePart2).Split(',').Select(long.Parse).ToArray();
}

public class Network
{
    private NetworkNode[] Nodes = [];
    private long[] Program;
    private object Sync = new object();
    private HashSet<long> YSentToZero = new HashSet<long>();
    private Packet LastControlPacket = null;

    public long? First255Received = null;
    public long? First255SentTwice = null;

    public Network(long[] program)
    {
        Program = program;
        CreateNodes();
    }

    private void CreateNodes()
    {
        Nodes = Enumerable.Range(0, 50).Select(i => new NetworkNode(this, Program, i)).ToArray();
    }

    public void Run()
    {
        lock (Sync)
        {
            foreach (var node in Nodes)
            {
                Task.Factory.StartNew(() =>
                {
                    node.Run();
                }, TaskCreationOptions.LongRunning);
            }
        }

        while (!First255Received.HasValue || !First255SentTwice.HasValue)
        {
            Thread.Sleep(1);

            if (Nodes.All(n => n.IsIdle()))
            {
                if (LastControlPacket != null)
                {
                    if (YSentToZero.Contains(LastControlPacket.Y))
                    {
                        if (!First255SentTwice.HasValue)
                        {
                            First255SentTwice = LastControlPacket.Y;
                        }
                    }
                    else
                    {
                        YSentToZero.Add(LastControlPacket.Y);
                    }

                    Nodes[0].Send(LastControlPacket);
                }

            }
        }

    }

    public void Send(Packet packet)
    {
        lock (Sync)
        {
            if (packet.Address == 255)
            {
                if (!First255Received.HasValue)
                {
                    First255Received = packet.Y;
                }

                LastControlPacket = packet;
            }

            if (packet.Address >= 0 && packet.Address < Nodes.Length)
            {
                Nodes[packet.Address].Send(packet);
            }
        }
    }
}



public record NetworkNode
{
    public long Address;
    private int IdleCounter = 0;
    private SemaphoreSlim Sempahore = new SemaphoreSlim(1);

    public bool IsIdle()
    {
        return IdleCounter >= 3;
    }
    
    public NetworkNode(Network network, long[] program, long address)
    {
        Network = network;
        Address = address;
        Computer = new Computer(program);
    }

    public void Run()
    {
        InputQueue.Enqueue(Address);

        Computer.Input += (c) =>
        {  
            long result;
            
            lock(InputQueue)
            {
                if (InputQueue.TryDequeue(out var value))
                {
                    result = value;
                }
                else
                {
                    IdleCounter++;
                    result = - 1;
                }
            }

            if (IsIdle())
            {
                Sempahore.Wait();
            }

            return result;
        };

        int outputState = 0;
        long address = 0;
        long x = 0;
        long y = 0;

        Computer.Output += (c, value) =>
        {
            if (outputState == 0)
            {
                address = value;
                outputState = 1;
            }
            else if (outputState == 1)
            {
                x = value;
                outputState = 2;
            }
            else if (outputState == 2)
            {
                y = value;
                outputState = 0;

                Packet packet = new Packet(address, x, y);
                Network.Send(packet);
            }
        };

        Computer.Run();
    }

    public void Send(Packet packet)
    {
        lock (InputQueue)
        {
            InputQueue.Enqueue(packet.X);
            InputQueue.Enqueue(packet.Y);
            IdleCounter = 0;
            Sempahore.Release();
        }
    }

    private Computer Computer;
    private Network Network;
    private Queue<long> InputQueue = [];
}

public record Packet(long Address, long X, long Y);
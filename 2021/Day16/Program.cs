// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day16\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day16\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    PacketDecoder decoder = new PacketDecoder(input);
    Packet packet = decoder.GetPacket();

    Console.WriteLine("Part 1: " + Part1(packet));
    Console.WriteLine("Part 2: " + Part2(packet));
    Console.ReadLine();
}

long Part1(Packet packet)
{
    return SumVersionRecursive(packet);
}

long Part2(Packet packet)
{
    return packet.GetValue();
}

int SumVersionRecursive(Packet packet)
{
    int result = packet.Version;
    if(packet is IPacketWithSubPackets packetWithSub)
    {
        foreach(var subpacket in packetWithSub.SubPackets)
        {
            result += SumVersionRecursive(subpacket); 
        }
    }

    return result;
}

public class PacketDecoder
{
    PacketSource PacketSource;

    public PacketDecoder(string hexInput)
    {
        PacketSource = new(MakeBinary(hexInput));
    }

    private string MakeBinary(string hexInput)
    {
        return string.Join("", hexInput.Select(ch => Convert.ToInt32("" + ch, 16)).Select(i => i.ToString("b4")));
    }

    public Packet GetPacket()
    {
        return Packet.Factory(PacketSource)!;
    }
}

public class PacketSource
{
    string BinaryString;
    int BinaryStringIndex = 0;

    public PacketSource(string source)
    {
        BinaryString = source;
    }

    public string ReadString(int count)
    {
        string binarySubString = BinaryString.Substring(BinaryStringIndex, count);
        BinaryStringIndex += count;
        return binarySubString;
    }

    public int ReadInt(int count)
    {
        string binarySubString = ReadString(count);
        return Convert.ToInt32(binarySubString, 2);
    }

    public PacketSource GetSubSource(int length)
    {
        string subString = ReadString(length);
        return new PacketSource(subString);
    }

    public bool EOF => BinaryStringIndex >= BinaryString.Length - 1;
}

public interface IPacketWithSubPackets
{
    public List<Packet> SubPackets { get; }
}

public abstract class Packet
{
    protected Packet() { }

    public int Version;
    public PacketType Type;

    protected abstract void ReadFrom(PacketSource source);

    public abstract long GetValue();

    public static Packet? Factory(PacketSource source)
    {
        if (source.EOF) return null;
        
        int version = source.ReadInt(3);
        PacketType type =  (PacketType)source.ReadInt(3);

        Packet packet = type switch
        {
            PacketType.LiteralValue => new LiteralPacket(),
            _ => new OperatorPacket(),
        };

        packet.Version = version;
        packet.Type = type;
        packet.ReadFrom(source);

        return packet;
    } 
}

public class LiteralPacket : Packet
{
    public long LiteralValue;

    public override long GetValue() => LiteralValue;

    protected override void ReadFrom(PacketSource source)
    {
        LiteralValue = 0;
        bool continued = true;
        while (continued)
        {
            continued = source.ReadInt(1) == 1;
           
            int numberSegment = source.ReadInt(4);
            LiteralValue <<= 4;
            LiteralValue += numberSegment;
        }
    }
}

public class OperatorPacket : Packet, IPacketWithSubPackets
{
    public List<Packet> SubPackets { get; } = [];

    public override long GetValue()
    {
        long[] values = SubPackets.Select(p => p.GetValue()).ToArray();

        return Type switch
        {
            PacketType.Sum => values.Sum(),
            PacketType.Product => values.Aggregate((a,b) => a * b),
            PacketType.Minimum => values.Min(),
            PacketType.Maximum => values.Max(),
            PacketType.GreaterThan => (values[0] > values[1]) ? 1 : 0,
            PacketType.LessThan => (values[0] < values[1]) ? 1 : 0,
            PacketType.EqualTo => (values[0] == values[1]) ? 1 : 0,
            _ => throw new NotSupportedException()
        };
    }


    protected override void ReadFrom(PacketSource source)
    {
        bool useSubPacketCount = source.ReadInt(1) == 1;
        if (useSubPacketCount)
        {
            ReadFromUsingSubPacketCount(source);
        }
        else
        {
            ReadFromUsingSourceLength(source);
        }
    }

    private void ReadFromUsingSourceLength(PacketSource source)
    {
        int length = source.ReadInt(15);
        var subSource = source.GetSubSource(length);

        while (true)
        {
            var packet = Packet.Factory(subSource);
            if (packet == null) break;
            SubPackets.Add(packet);
        }
    }

    private void ReadFromUsingSubPacketCount(PacketSource source)
    {
        int count = source.ReadInt(11);
        foreach(var _ in Enumerable.Range(1, count))
        {
            SubPackets.Add(Packet.Factory(source)!);
        }
    }
}

public enum PacketType
{
    Sum = 0,
    Product = 1,
    Minimum = 2,
    Maximum = 3,
    LiteralValue = 4,
    GreaterThan = 5,
    LessThan = 6,
    EqualTo = 7

}
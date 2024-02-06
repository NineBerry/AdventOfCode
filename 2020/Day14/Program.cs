// #define Sample

{

#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2020\Day14\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2020\Day14\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2020\Day14\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2020\Day14\Full.txt";
#endif

    string[] commandsPart1 = File.ReadAllLines(fileNamePart1);
    Console.WriteLine("Part 1: " + Part1(commandsPart1));

    string[] commandsPart2 = File.ReadAllLines(fileNamePart2);
    Console.WriteLine("Part 2: " + Part2(commandsPart2));

    Console.ReadLine();
}

long Part1(string[] commands)
{
    MemoryV1 memory = new MemoryV1();   
    memory.ExecuteCommands(commands);
    return memory.GetAllValuesSum();
}

long Part2(string[] commands)
{
    MemoryV2 memory = new MemoryV2();
    memory.ExecuteCommands(commands);
    return memory.GetAllValuesSum();
}

public abstract class MemoryBase
{
    public void ExecuteCommands(string[] commands)
    {
        foreach (string command in commands)
        {
            ExecuteCommand(command);
        }
    }

    public void ExecuteCommand(string command)
    {
        string[] parts = command.Split('=', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts[0] == "mask")
        {
            SetBitMask(parts[1]);
        }
        else
        {
            long address = int.Parse(parts[0].Substring(4, parts[0].Length - 5));
            long value = long.Parse(parts[1]);
            SetMemory(address, value);
        }
    }
    protected abstract void SetBitMask(string bitMask);
    protected abstract void SetMemory(long address, long value);

    public long GetAllValuesSum()
    {
        return memory.Values.Sum();
    }

    private static long[] InitBitValues()
    {
        List<long> values = [];

        long val = 1;
        foreach (var _ in Enumerable.Range(0, 63))
        {
            values.Add(val);
            val <<= 1;
        }

        return values.ToArray();
    }

    protected static long[] BitValues = InitBitValues();
    protected Dictionary<long, long> memory = [];
}


public class MemoryV1: MemoryBase
{ 
    protected override void SetMemory(long address, long value)
    {
        value = CurrentBitMask.ApplyTo(value);
        memory[address] = value;
    }

    protected override void SetBitMask(string bitMask)
    {
        CurrentBitMask = new BitMaskV1(bitMask);
    }

    private BitMaskV1 CurrentBitMask = new BitMaskV1("");

    public class BitMaskV1
    {
        public BitMaskV1(string bitMaskString)
        {
            OrValue = 0; AndNotValue = 0;

            foreach(var pair in bitMaskString.Reverse().Select((ch, i) => (Char: ch, Index: i)))
            {
                if(pair.Char == '1')
                {
                    OrValue += BitValues[pair.Index];
                }
                else if(pair.Char == '0') 
                {
                    AndNotValue += BitValues[pair.Index];
                }
            }
        }

        public long ApplyTo(long value)
        {
            return (value | OrValue) & ~AndNotValue;
        }

        private long OrValue;
        private long AndNotValue; 
    }
}


public class MemoryV2: MemoryBase
{
    protected override void SetBitMask(string bitMask)
    {
        CurrentBitMask = new BitMaskV2(bitMask);
    }

    protected override void SetMemory(long address, long value)
    {
        foreach(var modifiedAddress in CurrentBitMask.ApplyTo(address))
        {
            memory[modifiedAddress] = value;
        }
    }

    private BitMaskV2 CurrentBitMask = new BitMaskV2("");


    public class BitMaskV2
    {
        public BitMaskV2(string bitMaskString)
        {
            OrValue = 0;
            FloatingValues = [];

            foreach (var pair in bitMaskString.Reverse().Select((ch, i) => (Char: ch, Index: i)))
            {
                if (pair.Char == '1')
                {
                    OrValue += BitValues[pair.Index];
                }
                else if (pair.Char == 'X')
                {
                    FloatingValues.Add(BitValues[pair.Index]);
                }
            }
        }

        private long[] MakeFloatingAddressesRecursive(long[] baseAddresses, int floatingIndex)
        {
            if (floatingIndex >= FloatingValues.Count) return baseAddresses;

            List<long> collect = [];

            foreach (var baseAddress in baseAddresses) 
            {
                collect.Add(baseAddress | FloatingValues[floatingIndex]);
                collect.Add(baseAddress & ~FloatingValues[floatingIndex]);
            }

            return MakeFloatingAddressesRecursive(collect.ToArray(), floatingIndex + 1);
        }


        public long[] ApplyTo(long value)
        {
            long baseValue = (value | OrValue);
            return MakeFloatingAddressesRecursive([baseValue], 0);
        }


        private long OrValue;
        private List<long> FloatingValues;

    }
}

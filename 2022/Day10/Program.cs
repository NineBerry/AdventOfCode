// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day10\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day10\Full.txt";
#endif

    Instruction[] instructions = File.ReadAllLines(fileName).Select(Instruction.Factory).ToArray();
    CRT crt = new CRT();

    Console.WriteLine("Part 1: " + Part1(instructions, crt));
    Console.WriteLine("Part 2: \n" + crt.ToString());
    Console.ReadLine();
}

long Part1(Instruction[] instructions, CRT crt)
{
    long sum = 0;   

    Computer computer = new Computer();
    computer.CycleEvent += Computer_CycleEvent;
    computer.Perform(instructions);

    return sum;

    void Computer_CycleEvent(Computer computer)
    {
        crt.Signal(computer.Cycle, computer.X);
        
        if(computer.Cycle % 40 == 20)
        {
            sum += computer.Cycle * computer.X;
        }
    }
}

public class Computer
{
    public int X = 1;   
    public int Cycle = 0;

    public delegate void CycleDelegate(Computer computer);
    public event CycleDelegate? CycleEvent;

    public void DoCycle()
    {
        Cycle++;
        CycleEvent?.Invoke(this);
    }

    public void Perform(Instruction[] instructions)
    {
        foreach (Instruction instruction in instructions)
        {
            instruction.Execute(this);
        }
    }
}

public abstract class Instruction
{
    public abstract void Execute(Computer computer);

    public static Instruction Factory(string input)
    {
        string[] parts = input.Split(' ');

        return parts[0] switch
        {
            "addx" => new AddXInstruction { Operand = int.Parse(parts[1])},
            "noop" => new NoopInstruction(),
            _ => throw new ApplicationException("Unknown instruction")
        };
    }
}

public class NoopInstruction : Instruction
{
    public override void Execute(Computer computer)
    {
        computer.DoCycle(); 
    }
}

public class AddXInstruction : Instruction
{
    public int Operand;

    public override void Execute(Computer computer)
    {
        computer.DoCycle();
        computer.DoCycle();

        computer.X += Operand;
    }
}

public class CRT
{
    public readonly int Height = 6;
    public readonly int Width = 40;

    private HashSet<Point> Spots = [];

    public void Signal(int cycle, int spritePosition)
    {
        int y = (cycle - 1) / 40;
        int x = ((cycle) % 40) - 1;

        if(x >= spritePosition - 1 && x <= spritePosition + 1)
        {
            Spots.Add(new Point(x, y));
        }
    }

    private bool IsInCRT(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }

    private char[] emptyMap = null!;
    public override string ToString()
    {
        if (emptyMap == null)
        {
            string line = new string(Enumerable.Repeat(' ', Width).ToArray());
            emptyMap = string.Join('\n', Enumerable.Range(0, Height).Select(s => line)).ToCharArray();
        }

        char[] map = (char[])emptyMap.Clone();

        foreach (var light in Spots.Where(IsInCRT))
        {
            // +1 for line break
            map[light.Y * (Width + 1) + light.X] = '█';
        }

        return new string(map);
    }
}

public record Point(int X, int Y);

public class Computer
{
    public delegate long DoInput(Computer computer);
    public delegate void DoOutput(Computer computer, long value);
    public event DoInput? Input = null;
    public event DoOutput? Output = null;

    public long[] Program = [];
    public long Pointer;

    public long RegisterA = 0;
    public long RegisterB = 0;
    public long RegisterC = 0;

    public Computer(long[] program)
    {
        Program = [..program];
    }

    public void Run()
    {
        Pointer = 0;

        while (Pointer >= 0 && Pointer < Program.Length)
        {
            var instruction = CreateInstruction();
            Pointer += instruction.Execute(this);
        }
    }

    public long GetInput()
    {
        if (Input == null) throw new ApplicationException("Input event not set");
        return Input(this);
    }

    public void SetOutput(long value)
    {
        if (Output == null) throw new ApplicationException("Output event not set");
        Output(this, value);
    }

    public long GetCombo(long value)
    {
        return value switch
        {
            >= 0 and <= 3 => value,
            4 => RegisterA,
            5 => RegisterB,
            6 => RegisterC,
            _ => throw new Exception("Invalid Combo " + value)
        };
    }

    private long GetMemory(long index)
    {
        return Program[index];
    }

    private Instruction CreateInstruction()
    {
        long opcode = GetMemory(Pointer);
        long operand = GetMemory(Pointer + 1);

        return opcode switch
        {
            0 => new ADVInstruction(operand),
            1 => new BXLInstruction(operand),
            2 => new BSTInstruction(operand),
            3 => new JNZInstruction(operand),
            4 => new BXCInstruction(operand),
            5 => new OUTInstruction(operand),
            6 => new BDVInstruction(operand),
            7 => new CDVInstruction(operand),
            _ => throw new ApplicationException("Unknown opcode")
        };
    }
}

public abstract class Instruction
{

    public Instruction(long operand)
    {
        Operand = operand;
    }

    public long Operand;
    public abstract long Execute(Computer computer);
}

public class ADVInstruction : Instruction
{
    public ADVInstruction(long operand) : base(operand)
    {
    }

    protected long DoCalc(Computer computer)
    {
        int exponent = (int)computer.GetCombo(Operand);
        if (exponent == 0) return computer.RegisterA;
        if (exponent < 0) throw new Exception("Negative exponent");
        return computer.RegisterA / (2L << (exponent - 1));
    }

    public override long Execute(Computer computer)
    {
        computer.RegisterA = DoCalc(computer);
        return 2;
    }
}

public class BXLInstruction : Instruction
{
    public BXLInstruction(long operand) : base(operand)
    {
    }

    public override long Execute(Computer computer)
    {
        computer.RegisterB = computer.RegisterB ^ Operand;
        return 2;
    }
}

public class BSTInstruction : Instruction
{
    public BSTInstruction(long operand) : base(operand)
    {
    }

    public override long Execute(Computer computer)
    {
        computer.RegisterB = computer.GetCombo(Operand) % 8;
        return 2;
    }
}

public class JNZInstruction : Instruction
{
    public JNZInstruction(long operand) : base(operand)
    {
    }

    public override long Execute(Computer computer)
    {
        if (computer.RegisterA == 0) return 2;
        return -computer.Pointer + Operand;
    }
}


public class BXCInstruction : Instruction
{
    public BXCInstruction(long operand) : base(operand)
    {
    }

    public override long Execute(Computer computer)
    {
        computer.RegisterB = computer.RegisterB ^ computer.RegisterC;
        return 2;
    }
}

public class OUTInstruction : Instruction
{
    public OUTInstruction(long operand) : base(operand)
    {
    }

    public override long Execute(Computer computer)
    {
        long value = computer.GetCombo(Operand) % 8;
        computer.SetOutput(value);
        return 2;
    }
}

public class BDVInstruction : ADVInstruction
{
    public BDVInstruction(long operand) : base(operand)
    {
    }

    public override long Execute(Computer computer)
    {
        computer.RegisterB = DoCalc(computer);
        return 2;
    }
}

public class CDVInstruction : ADVInstruction
{
    public CDVInstruction(long operand) : base(operand)
    {
    }

    public override long Execute(Computer computer)
    {
        computer.RegisterC = DoCalc(computer);
        return 2;
    }
}
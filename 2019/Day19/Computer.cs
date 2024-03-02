public class Computer
{
    public delegate long DoInput(Computer computer);
    public delegate void DoOutput(Computer computer, long value);
    public event DoInput? Input = null;
    public event DoOutput? Output = null;

    public Dictionary<long, long> Memory = [];
    public long Pointer;
    public long RelativeBase;

    public Computer(long[] program)
    {
        Memory = program.Select((long value, int index) => (index, value)).ToDictionary(p => (long)p.index, p => p.value);
    }

    public void Run()
    {
        Pointer = 0;

        while (Pointer >= 0)
        {
            var instruction = CreateInstruction();
            Pointer += instruction.Execute(this);
        }
    }

    public long SetValue(long paramNumber, long originalOpcodeWithParameters, long valueToSet)
    {
        long parameterValue = GetMemory(Pointer + paramNumber);
        ParameterMode mode = GetParameterMode(paramNumber, originalOpcodeWithParameters);

        return SetValue(mode, parameterValue, valueToSet);
    }

    public long SetValue(ParameterMode mode, long parameterValue, long valueToSet)
    {
        return mode switch
        {
            ParameterMode.PositionMode => SetMemory(parameterValue, valueToSet),
            ParameterMode.RelativeMode => SetMemory(RelativeBase + parameterValue, valueToSet),
            ParameterMode.ImmediateMode => throw new ApplicationException("Cannot set value in immediate mode"),
            _ => throw new ApplicationException("Invalide Parametermode")
        };
    }

    public long GetValue(long paramNumber, long originalOpcodeWithParameters)
    {
        long parameterValue = GetMemory(Pointer + paramNumber);
        ParameterMode mode = GetParameterMode(paramNumber, originalOpcodeWithParameters);

        return GetValue(mode, parameterValue);
    }

    public long GetValue(ParameterMode mode, long value)
    {
        return mode switch
        {
            ParameterMode.PositionMode => GetMemory(value),
            ParameterMode.RelativeMode => GetMemory(RelativeBase + value),
            ParameterMode.ImmediateMode => value,
            _ => throw new ApplicationException("Invalide Parametermode")
        };
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

    private ParameterMode GetParameterMode(long paramNumber, long originalOpcodeWithParameters)
    {
        long modeBase = paramNumber switch
        {
            1 => originalOpcodeWithParameters / 100,
            2 => originalOpcodeWithParameters / 1000,
            3 => originalOpcodeWithParameters / 10000,
            _ => throw new ApplicationException("Invalid paramNumber")
        };

        modeBase %= 10;

        return (ParameterMode)modeBase;
    }

    private long GetMemory(long index)
    {
        Memory.TryGetValue(index, out var value);
        return value;
    }
    private long SetMemory(long index, long value)
    {
        return Memory[index] = value;
    }

    private Instruction CreateInstruction()
    {
        long opCodeWithParameters = GetMemory(Pointer);
        long opCode = opCodeWithParameters % 100;

        return opCode switch
        {
            1 => new AddInstruction(opCodeWithParameters),
            2 => new MultiplyInstruction(opCodeWithParameters),
            3 => new InputInstruction(opCodeWithParameters),
            4 => new OutputInstruction(opCodeWithParameters),
            5 => new JumpIfTrueInstruction(opCodeWithParameters),
            6 => new JumpIfFalseInstruction(opCodeWithParameters),
            7 => new LessThanInstruction(opCodeWithParameters),
            8 => new EqualsInstruction(opCodeWithParameters),
            9 => new RelativeBaseInstruction(opCodeWithParameters),
            99 => new ExitInstruction(opCodeWithParameters),
            _ => throw new ApplicationException("Unknown opcode")
        };
    }

    public enum ParameterMode
    {
        PositionMode = 0,
        ImmediateMode = 1,
        RelativeMode = 2,
           
    }
}

public abstract class Instruction
{

    public Instruction(long originalOpcode)
    {
        OriginalOpcodeWithParameters = originalOpcode;
    }

    public long OriginalOpcodeWithParameters;
    public abstract long Execute(Computer computer);
}


public class ExitInstruction : Instruction
{
    public ExitInstruction(long originalOpcode) : base(originalOpcode)
    {
    }

    public override long Execute(Computer computer)
    {
        return -computer.Pointer - 2;
    }
}


public class AddInstruction : Instruction
{
    public AddInstruction(long originalOpcode) : base(originalOpcode)
    {
    }

    public override long Execute(Computer computer)
    {
        long operand1 = computer.GetValue(1, OriginalOpcodeWithParameters);
        long operand2 = computer.GetValue(2, OriginalOpcodeWithParameters);
        long operationResult = operand1 + operand2;

        computer.SetValue(3, OriginalOpcodeWithParameters, operationResult);

        return 4;
    }
}

public class MultiplyInstruction : Instruction
{
    public MultiplyInstruction(long originalOpcode) : base(originalOpcode)
    {
    }

    public override long Execute(Computer computer)
    {
        long operand1 = computer.GetValue(1, OriginalOpcodeWithParameters);
        long operand2 = computer.GetValue(2, OriginalOpcodeWithParameters);
        long operationResult = operand1 * operand2;

        computer.SetValue(3, OriginalOpcodeWithParameters, operationResult);

        return 4;
    }
}

public class InputInstruction : Instruction
{
    public InputInstruction(long originalOpcode) : base(originalOpcode)
    {
    }

    public override long Execute(Computer computer)
    {
        long value = computer.GetInput();
        computer.SetValue(1, OriginalOpcodeWithParameters, value);
        return 2;
    }
}

public class OutputInstruction : Instruction
{
    public OutputInstruction(long originalOpcode) : base(originalOpcode)
    {
    }

    public override long Execute(Computer computer)
    {
        long value = computer.GetValue(1, OriginalOpcodeWithParameters);
        computer.SetOutput(value);
        return 2;
    }
}

public class JumpIfTrueInstruction : Instruction
{
    public JumpIfTrueInstruction(long originalOpcode) : base(originalOpcode)
    {
    }

    public override long Execute(Computer computer)
    {
        long checkValue = computer.GetValue(1, OriginalOpcodeWithParameters);
        long gotoValue = computer.GetValue(2, OriginalOpcodeWithParameters);

        if (checkValue != 0)
        {
            return gotoValue - computer.Pointer;
        }
        else
        {
            return 3;
        }
    }
}

public class JumpIfFalseInstruction : Instruction
{
    public JumpIfFalseInstruction(long originalOpcode) : base(originalOpcode)
    {
    }

    public override long Execute(Computer computer)
    {
        long checkValue = computer.GetValue(1, OriginalOpcodeWithParameters);
        long gotoValue = computer.GetValue(2, OriginalOpcodeWithParameters);

        if (checkValue == 0)
        {
            return gotoValue - computer.Pointer;
        }
        else
        {
            return 3;
        }

    }
}

public class LessThanInstruction : Instruction
{
    public LessThanInstruction(long originalOpcode) : base(originalOpcode)
    {
    }

    public override long Execute(Computer computer)
    {
        long firstParameter = computer.GetValue(1, OriginalOpcodeWithParameters);
        long SecondParameter = computer.GetValue(2, OriginalOpcodeWithParameters);

        long toStore = (firstParameter < SecondParameter) ? 1 : 0;
        computer.SetValue(3, OriginalOpcodeWithParameters, toStore);

        return 4;
    }
}

public class EqualsInstruction : Instruction
{
    public EqualsInstruction(long originalOpcode) : base(originalOpcode)
    {
    }

    public override long Execute(Computer computer)
    {
        long firstParameter = computer.GetValue(1, OriginalOpcodeWithParameters);
        long SecondParameter = computer.GetValue(2, OriginalOpcodeWithParameters);

        long toStore = (firstParameter == SecondParameter) ? 1 : 0;
        computer.SetValue(3, OriginalOpcodeWithParameters, toStore);

        return 4;
    }
}

public class RelativeBaseInstruction : Instruction
{
    public RelativeBaseInstruction(long originalOpcode) : base(originalOpcode)
    {
    }

    public override long Execute(Computer computer)
    {
        long firstParameter = computer.GetValue(1, OriginalOpcodeWithParameters);
        computer.RelativeBase += firstParameter;

        return 2;
    }
}
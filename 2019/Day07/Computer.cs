public class Computer
{
    public delegate int DoInput(Computer computer);
    public delegate void DoOutput(Computer computer, int value);
    public event DoInput? Input = null;
    public event DoOutput? Output = null;

    public int[] Memory = [];
    public int Pointer;

    public Computer(int[] program)
    {
        Memory = [.. program];
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

    public int SetValue(int paramNumber, int originalOpcodeWithParameters, int valueToSet)
    {
        int parameterValue = Memory[Pointer + paramNumber];
        ParameterMode mode = GetParameterMode(paramNumber, originalOpcodeWithParameters);

        return SetValue(mode, parameterValue, valueToSet);
    }

    public int SetValue(ParameterMode mode, int parameterValue, int valueToSet)
    {
        return mode switch
        {
            ParameterMode.PositionMode => Memory[parameterValue] = valueToSet,
            ParameterMode.ImmediateMode => throw new ApplicationException("Cannot set value in immediate mode"),
            _ => throw new ApplicationException("Invalide Parametermode")
        };
    }

    public int GetValue(int paramNumber, int originalOpcodeWithParameters)
    {
        int parameterValue = Memory[Pointer + paramNumber];
        ParameterMode mode = GetParameterMode(paramNumber, originalOpcodeWithParameters);

        return GetValue(mode, parameterValue);
    }

    public int GetValue(ParameterMode mode, int value)
    {
        return mode switch
        {
            ParameterMode.PositionMode => Memory[value],
            ParameterMode.ImmediateMode => value,
            _ => throw new ApplicationException("Invalide Parametermode")
        };
    }

    public int GetInput()
    {
        if (Input == null) throw new ApplicationException("Input event not set");
        return Input(this);
    }

    public void SetOutput(int value)
    {
        if (Output == null) throw new ApplicationException("Output event not set");
        Output(this, value);
    }

    private ParameterMode GetParameterMode(int paramNumber, int originalOpcodeWithParameters)
    {
        int modeBase = paramNumber switch
        {
            1 => originalOpcodeWithParameters / 100,
            2 => originalOpcodeWithParameters / 1000,
            3 => originalOpcodeWithParameters / 10000,
            _ => throw new ApplicationException("Invalid paramNumber")
        };

        modeBase %= 10;

        return (ParameterMode)modeBase;
    }

    private Instruction CreateInstruction()
    {
        int opCodeWithParameters = Memory[Pointer];
        int opCode = opCodeWithParameters % 100;

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
            99 => new ExitInstruction(opCodeWithParameters),
            _ => throw new ApplicationException("Unknown opcode")
        };
    }

    public enum ParameterMode
    {
        PositionMode = 0,
        ImmediateMode = 1
    }
}

public abstract class Instruction
{

    public Instruction(int originalOpcode)
    {
        OriginalOpcodeWithParameters = originalOpcode;
    }

    public int OriginalOpcodeWithParameters;
    public abstract int Execute(Computer computer);
}


public class ExitInstruction : Instruction
{
    public ExitInstruction(int originalOpcode) : base(originalOpcode)
    {
    }

    public override int Execute(Computer computer)
    {
        return -computer.Pointer - 2;
    }
}


public class AddInstruction : Instruction
{
    public AddInstruction(int originalOpcode) : base(originalOpcode)
    {
    }

    public override int Execute(Computer computer)
    {
        int operand1 = computer.GetValue(1, OriginalOpcodeWithParameters);
        int operand2 = computer.GetValue(2, OriginalOpcodeWithParameters);
        int operationResult = operand1 + operand2;

        computer.SetValue(3, OriginalOpcodeWithParameters, operationResult);

        return 4;
    }
}

public class MultiplyInstruction : Instruction
{
    public MultiplyInstruction(int originalOpcode) : base(originalOpcode)
    {
    }

    public override int Execute(Computer computer)
    {
        int operand1 = computer.GetValue(1, OriginalOpcodeWithParameters);
        int operand2 = computer.GetValue(2, OriginalOpcodeWithParameters);
        int operationResult = operand1 * operand2;

        computer.SetValue(3, OriginalOpcodeWithParameters, operationResult);

        return 4;
    }
}

public class InputInstruction : Instruction
{
    public InputInstruction(int originalOpcode) : base(originalOpcode)
    {
    }

    public override int Execute(Computer computer)
    {
        int value = computer.GetInput();
        computer.SetValue(1, OriginalOpcodeWithParameters, value);
        return 2;
    }
}

public class OutputInstruction : Instruction
{
    public OutputInstruction(int originalOpcode) : base(originalOpcode)
    {
    }

    public override int Execute(Computer computer)
    {
        int value = computer.GetValue(1, OriginalOpcodeWithParameters);
        computer.SetOutput(value);
        return 2;
    }
}

public class JumpIfTrueInstruction : Instruction
{
    public JumpIfTrueInstruction(int originalOpcode) : base(originalOpcode)
    {
    }

    public override int Execute(Computer computer)
    {
        int checkValue = computer.GetValue(1, OriginalOpcodeWithParameters);
        int gotoValue = computer.GetValue(2, OriginalOpcodeWithParameters);

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
    public JumpIfFalseInstruction(int originalOpcode) : base(originalOpcode)
    {
    }

    public override int Execute(Computer computer)
    {
        int checkValue = computer.GetValue(1, OriginalOpcodeWithParameters);
        int gotoValue = computer.GetValue(2, OriginalOpcodeWithParameters);

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
    public LessThanInstruction(int originalOpcode) : base(originalOpcode)
    {
    }

    public override int Execute(Computer computer)
    {
        int firstParameter = computer.GetValue(1, OriginalOpcodeWithParameters);
        int SecondParameter = computer.GetValue(2, OriginalOpcodeWithParameters);

        int toStore = (firstParameter < SecondParameter) ? 1 : 0;
        computer.SetValue(3, OriginalOpcodeWithParameters, toStore);

        return 4;
    }
}

public class EqualsInstruction : Instruction
{
    public EqualsInstruction(int originalOpcode) : base(originalOpcode)
    {
    }

    public override int Execute(Computer computer)
    {
        int firstParameter = computer.GetValue(1, OriginalOpcodeWithParameters);
        int SecondParameter = computer.GetValue(2, OriginalOpcodeWithParameters);

        int toStore = (firstParameter == SecondParameter) ? 1 : 0;
        computer.SetValue(3, OriginalOpcodeWithParameters, toStore);

        return 4;
    }
}
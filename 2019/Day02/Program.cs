// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day02\Sample.txt";
    bool setAlarmState = false;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day02\Full.txt";
    bool setAlarmState = true;
#endif

    int[] values = File.ReadAllText(fileName).Split(',').Select(int.Parse).ToArray();

    Console.WriteLine("Part 1: " + Part1(values, setAlarmState));
    Console.WriteLine("Part 2: " + Part2(values));
    Console.ReadLine();
}

int Part1(int[] values, bool setAlarmState)
{
    int noun = setAlarmState ? 12 : values[1];
    int verb = setAlarmState ? 2 : values[2];

    return RunWithNounAndVerb(values, noun, verb);
}

int Part2(int[] values)
{
    for(int noun=0; noun <= 99; noun++)
    {
        for(int verb = 0; verb <= 99; verb++)
        {
            int result = RunWithNounAndVerb(values, noun, verb);

            if(result == 19690720)
            {
                return 100 * noun + verb;
            }
        }
    }

    return 0;
}


int RunWithNounAndVerb(int[] values, int noun, int verb)
{
    int[] program = [.. values];
    program[1] = noun;
    program[2] = verb;

    RunIntCodeProgram(program);

    return program[0];
}

void RunIntCodeProgram(int[] values)
{
    int pointer = 0;

    while (values[pointer] != 99)
    {
        int opcode = values[pointer];
        int operand1 = values[values[pointer + 1]];
        int operand2 = values[values[pointer + 2]];

        int result = 0;
        if (opcode == 1)
        {
            result = operand1 + operand2;
        }
        else if (opcode == 2)
        {
            result = operand1 * operand2;
        }
        else
        {
            throw new ApplicationException("Unknown Opcode");
        }

        values[values[pointer + 3]] = result;

        pointer += 4;
    }
}
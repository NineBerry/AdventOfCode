// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day09\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day09\Full.txt";
#endif

    long[] program = ReadProgram(fileName);
    Console.WriteLine("Part 1: " + Solve(program, 1));
    Console.WriteLine("Part 1: " + Solve(program, 2));
    Console.ReadLine();
}

static long[] ReadProgram(string fileNamePart2)
{
    return File.ReadAllText(fileNamePart2).Split(',').Select(long.Parse).ToArray();
}

long Solve(long[] program, int input)
{
    long output = 0;
    
    Computer computer = new Computer(program);

    computer.Input += (c) => input;
    computer.Output += (c, value) => output = value;

    computer.Run();

    return output;
}

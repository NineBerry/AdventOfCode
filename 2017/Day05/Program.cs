// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day05\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day05\Full.txt";
#endif

    int[] program = File.ReadAllLines(fileName).Select(s => int.Parse(s)).ToArray();

    Console.WriteLine("Part 1: " + Part1([.. program]));
    Console.WriteLine("Part 2: " + Part2([.. program]));
    Console.ReadLine();
}

long Part1(int[] program)
{
    int steps = 0;
    int programIndex = 0;

    while(programIndex >= 0 && programIndex < program.Length)
    {
        int currentIndex = programIndex;
        programIndex += program[currentIndex];
        program[currentIndex]++;

        steps++;
    }

    return steps;
}

long Part2(int[] program)
{
    int steps = 0;
    int programIndex = 0;

    while (programIndex >= 0 && programIndex < program.Length)
    {
        int currentIndex = programIndex;
        programIndex += program[currentIndex];

        if (program[currentIndex] >= 3)
        {
            program[currentIndex]--;
        }
        else
        {
            program[currentIndex]++;
        }

        steps++;
    }

    return steps;
}

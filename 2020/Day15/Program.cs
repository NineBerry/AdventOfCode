// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day15\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day15\Full.txt";
#endif

    int[] startValues = File.ReadAllText(fileName)
        .Split(",")
        .Select(int.Parse)
        .ToArray();

    Console.WriteLine("Part 1: " + Solve(startValues, 2020));
    Console.WriteLine("Part 2: " + Solve(startValues, 30_000_000)); 
    Console.ReadLine();
}

long Solve(int[] start, int targetStep)
{
    int[] spoken = new int[targetStep];

    int turn = 1;
    int next = 0;

    foreach (int initial in start)
    {
        next = speak(initial, turn++);
    }

    while (turn < targetStep)
    {
        next = speak(next, turn++);
    }
    return next;


    int speak(int number, int turn)
    {
        int lastSpoken = spoken[number];
        int nextNumber = lastSpoken > 0 ? turn - lastSpoken : 0;
        spoken[number] = turn;
        return nextNumber;
    }



}


// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day01\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string input)
{
    return input.Count(ch => ch == '(') - input.Count(ch => ch == ')');
}

long Part2(string input)
{
    int floor = 0;

    for(int i=1; i <= input.Length; i++)
    {
        switch (input[i-1])
        {
            case '(':
                floor++;
                break;
            case ')': 
                floor--; 
                break;
            default:
                break;
        }

        if (floor < 0) return i;
    }

    return 0;
}

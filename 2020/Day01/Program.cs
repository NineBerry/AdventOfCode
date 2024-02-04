// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day01\Full.txt";
#endif

    int[] numbers = File.ReadAllLines(fileName).Select(int.Parse).ToArray();

    Console.WriteLine("Part 1: " + Part1(numbers));
    Console.WriteLine("Part 2: " + Part2(numbers));
    Console.ReadLine();
}

long Part1(int[] numbers)
{
    HashSet<int> numbersSet = numbers.ToHashSet();

    foreach (int number in numbers)
    {
        int rest = 2020 - number;

        if(numbersSet.Contains(rest))
        {
            return number * rest;
        }
    }

    return 0;    
}

long Part2(int[] numbers)
{
    HashSet<int> numbersSet = numbers.ToHashSet();

    foreach (int number1 in numbers)
    {
        foreach (int number2 in numbers)
        {
            if (number1 == number2) continue;
            
            int rest = 2020 - number1 - number2;
            if (numbersSet.Contains(rest))
            {
                return number1 * number2 * rest;
            }
        }
    }

    return 0;
}

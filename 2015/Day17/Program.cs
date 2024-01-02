// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day17\Sample.txt";
    int eggnog = 25;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day17\Full.txt";
    int eggnog = 150;
#endif

    string[] input = File.ReadAllLines(fileName);
    int[] containers = input.Select(int.Parse).OrderDescending().ToArray();

    Console.WriteLine("Part 1: " + Part1(containers, eggnog));
    Console.WriteLine("Part 2: " + Part2(containers, eggnog));
    Console.ReadLine();
}

long Part1(int[] containers, int eggnog)
{
    var combinations = FillContainers(eggnog, [], containers);
    return combinations.Count;
}

long Part2(int[] containers, int eggnog)
{
    var combinations = FillContainers(eggnog, [], containers);
    int minContainers = combinations.Select(c => c.Length).Min();
    return combinations.Count(c => c.Length == minContainers);
}

List<int[]> FillContainers(int eggnog, int[] fullContainers, int[] emptyContainers)
{
    if(eggnog == 0)
    {
        return [fullContainers];
    }

    if(eggnog < 0)
    {
        return [];
    }
    
    if(emptyContainers is [])
    {
        return [];
    }

    List<int[]> result = new();

    for(int i=0; i < emptyContainers.Length; i++)
    {
        int nextContainer = emptyContainers[i];
        var newCombinations = FillContainers(eggnog - nextContainer, [..fullContainers, nextContainer], emptyContainers.Skip(i + 1).ToArray());
        result.AddRange(newCombinations);
    }

    return result;
}
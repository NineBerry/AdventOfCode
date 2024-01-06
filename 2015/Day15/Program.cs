// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day15\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day15\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Ingredient[] ingredients = input.Select(s => new Ingredient(s)).ToArray();

    Console.WriteLine("Part 1: " + Part1(ingredients));
    Console.WriteLine("Part 2: " + Part2(ingredients));
    Console.ReadLine();
}

long Part1(Ingredient[] ingredients)
{
    return 
        CreateCombinations(ingredients.Length, 100)
        .Select(combination => ScoreCookie(ingredients, combination).Score)
        .Max();
}

long Part2(Ingredient[] ingredients)
{
    return 
        CreateCombinations(ingredients.Length, 100)
        .Select(combination => ScoreCookie(ingredients, combination))
        .Where(scoring => scoring.Calories == 500).
        Select(scoring => scoring.Score)
        .Max();
}

IEnumerable<int[]> CreateCombinations(int countNumbers, int sum)
{
    foreach (var combination in CreateCombinationsRecursive(countNumbers, sum, []))
    {
        yield return combination;
    }
}

IEnumerable<int[]> CreateCombinationsRecursive(int countNumbers, int sum, int[] soFar)
{
    int sumSoFar = soFar.Sum();
    int soFarCount = soFar.Count();

    if (soFarCount == countNumbers - 1)
    {
        yield return [..soFar, sum - sumSoFar];
    }
    else 
    {
        for(int i=0; i<= (sum - sumSoFar); i++)
        {
            foreach(var combination in CreateCombinationsRecursive(countNumbers, sum, [..soFar, i]))
            {
                yield return combination;
            }
        }
    }
}

(long Score, long Calories) ScoreCookie(Ingredient[] ingredients, int[] measures)
{
    long capacity = 0;
    long durability = 0;
    long flavor = 0;
    long texture = 0;
    long calories = 0;

    for(int i=0; i < ingredients.Length; i++)
    {
        capacity += ingredients[i].Capacity * measures[i];
        durability += ingredients[i].Durability * measures[i];
        flavor += ingredients[i].Flavor * measures[i];
        texture += ingredients[i].Texture * measures[i];
        calories += ingredients[i].Calories * measures[i];
    }

    capacity = Math.Max(capacity, 0);
    durability = Math.Max(durability, 0);
    flavor = Math.Max(flavor, 0);
    texture = Math.Max(texture, 0);

    return (capacity * durability * flavor * texture, calories);
}

record struct Ingredient
{
    public Ingredient(string input)
    {
        Name = input.Split(':')[0];

        int[] values = Regex.Matches(input, "-?[0-9]+").Select(m => int.Parse(m.Value)).ToArray();

        Capacity = values[0];
        Durability = values[1];
        Flavor = values[2];
        Texture = values[3];
        Calories = values[4];

    }
    public readonly string Name;

    public readonly int Capacity;
    public readonly int Durability;
    public readonly int Flavor;
    public readonly int Texture;
    public readonly int Calories;
}
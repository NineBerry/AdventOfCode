// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day21\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day21\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Menu menu = new Menu(input);
    menu.DetermineAllergens();

    Console.WriteLine("Part 1: " + Part1(menu));
    Console.WriteLine("Part 2: " + Part2(menu));
    Console.ReadLine();
}

long Part1(Menu menu)
{
    return menu.CountNonAllergenIngredients();
}

string Part2(Menu menu)
{
    return menu.GetCanonicalDangerousIngredientList();
}

public class Menu
{
    public Menu(string[] lines)
    {
        Food = lines.Select(s => new Food(s)).ToArray();
        AllIngredients = Food.SelectMany(f => f.Ingredients).ToHashSet();
        AllAllergens = Food.SelectMany(f => f.Allergens).ToHashSet();
    }

    public Food[] Food = [];
    public HashSet<string> AllIngredients = [];
    public HashSet<string> AllAllergens = [];
    
    Dictionary<string, string> KnownAllergenForIngredient = [];

    internal void DetermineAllergens()
    {
        Dictionary<string, HashSet<string>> possibleIngredientsForAllergen = [];

        foreach(var allergen in AllAllergens)
        {
            var possibleIngredients = 
                Food
                .Where(f => f.Allergens.Contains(allergen))
                .Select(f => f.Ingredients)
                .Aggregate((a, b) => a.Intersect(b).ToHashSet())
                .ToHashSet();

            possibleIngredientsForAllergen.Add(allergen, possibleIngredients);
        }

        while (possibleIngredientsForAllergen.Any())
        {
            var allergen = possibleIngredientsForAllergen.First(pair => pair.Value.Count == 1).Key;
            var ingredient  = possibleIngredientsForAllergen[allergen].Single();

            possibleIngredientsForAllergen.Remove(allergen);
            foreach(var pair in possibleIngredientsForAllergen)
            {
                pair.Value.Remove(ingredient);
            }

            KnownAllergenForIngredient[ingredient] = allergen;
        }
    }

    public int CountNonAllergenIngredients()
    {
        HashSet<string> allergenIngredients = KnownAllergenForIngredient.Keys.ToHashSet();
        HashSet<string> nonAllergens = AllIngredients.Except(allergenIngredients).ToHashSet();

        return Food.Select(f => f.Ingredients.Intersect(nonAllergens).Count()).Sum();
    }

    public string GetCanonicalDangerousIngredientList()
    {
        var dangerousIngredientsSortedByAllergen = 
            KnownAllergenForIngredient
            .OrderBy(pair => pair.Value)
            .Select(pair => pair.Key);
        
        return string.Join(",", dangerousIngredientsSortedByAllergen);
    }
}

public class Food
{
    public Food(string line)
    {
        string[] parts = line.Split(new string[] { " (contains ", ")" }, StringSplitOptions.RemoveEmptyEntries);
        Ingredients = parts[0].Split(" ", StringSplitOptions.TrimEntries).ToHashSet();
        Allergens = parts[1].Split(",", StringSplitOptions.TrimEntries).ToHashSet();
    }

    public HashSet<string> Ingredients;
    public HashSet<string> Allergens;
}
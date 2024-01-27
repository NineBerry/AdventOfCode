// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day14\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day14\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(int.Parse(input)));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

string Part1(int countRecipes)
{
    LinkedList<int> recipes = [];
    
    var elf1Current = recipes.AddLast(3);
    var elf2Current = recipes.AddLast(7);

    while(recipes.Count < countRecipes + 10)
    {
        string nextRecipeValue = (elf1Current.Value + elf2Current.Value).ToString();

        foreach(char nextRecipe in nextRecipeValue ) 
        { 
            recipes.AddLast(int.Parse("" + nextRecipe));
        }

        MoveElf(ref elf1Current);
        MoveElf(ref elf2Current);
    }

    var resultCollect = recipes.First!;
    resultCollect = GetNextClockWiseCount(recipes, resultCollect, countRecipes - 1);

    string result = "";
    foreach(var _ in Enumerable.Range(1, 10))
    {
        resultCollect = GetNextClockWise(recipes, resultCollect);
        result += resultCollect.Value;
    }
    return result;

    void MoveElf(ref LinkedListNode<int> elf)
    {
        int move = 1 + elf.Value;
        elf = GetNextClockWiseCount(recipes, elf, move);
    }
}

int Part2(string wantedSequence)
{
    LinkedList<int> recipes = [];

    var elf1Current = recipes.AddLast(3);
    var elf2Current = recipes.AddLast(7);

    while (true)
    {
        string nextRecipeValue = (elf1Current.Value + elf2Current.Value).ToString();

        foreach (char nextRecipe in nextRecipeValue)
        {
            recipes.AddLast(int.Parse("" + nextRecipe));

            if (CheckSequence())
            {
                return recipes.Count - wantedSequence.Length;
            }
        }

        MoveElf(ref elf1Current);
        MoveElf(ref elf2Current);
    }

    bool CheckSequence()
    {
        var resultCollect = recipes.Last!;
        resultCollect = GetNextCounterClockWiseCount(recipes, resultCollect, wantedSequence.Length - 1);

        string result = "";
        foreach (var _ in Enumerable.Range(1, wantedSequence.Length))
        {
            result += resultCollect.Value;
            resultCollect = GetNextClockWise(recipes, resultCollect);
        }
        
        return result == wantedSequence;
    }

    void MoveElf(ref LinkedListNode<int> elf)
    {
        int move = 1 + elf.Value;
        elf = GetNextClockWiseCount(recipes, elf, move);
    }
}
LinkedListNode<int> GetNextClockWiseCount(LinkedList<int> elves, LinkedListNode<int> current, int count)
{
    foreach (var _ in Enumerable.Range(1, count))
    {
        current = GetNextClockWise(elves, current);
    }

    return current;
}


LinkedListNode<int> GetNextClockWise(LinkedList<int> elves, LinkedListNode<int> current)
{
    return current.Next ?? elves.First!;
}

LinkedListNode<int> GetNextCounterClockWiseCount(LinkedList<int> elves, LinkedListNode<int> current, int count)
{
    foreach (var _ in Enumerable.Range(1, count))
    {
        current = GetNextCounterClockWise(elves, current);
    }

    return current;
}

LinkedListNode<int> GetNextCounterClockWise(LinkedList<int> elves, LinkedListNode<int> current)
{
    return current.Previous ?? elves.Last!;
}
// #define Sample
using System.Diagnostics;
using System.Numerics;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day24\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day24\Full.txt";
#endif

    HashSet<long> numbers = File.ReadAllLines(fileName).Select(l => long.Parse(l)).ToHashSet();

    Console.WriteLine("Part 1: " + Part1(numbers));
    Console.WriteLine("Part 2: " + Part2(numbers));
    Console.ReadLine();
}

long Part1(HashSet<long> numbers)
{
    long sumOfAll = numbers.Sum();
    Debug.Assert(sumOfAll % 3 == 0);
    long groupWeight = sumOfAll / 3;

    int maxCount = 1;

    while (true)
    {
        var combination =
            GetCombinations(numbers, groupWeight, maxCount)
            .Where(x => x.Count == maxCount)
            .OrderBy(x => x.Product())
            .Where(comb => IsValidGroup(comb, numbers))
            .FirstOrDefault();


        if(combination != null) return combination.Product();
        maxCount++;
    }
}

// Our test here "IsValidGroup" is actually not complete
// We only check that there is a second group with the same
// sum but don't check that the remaining numbers can be divided 
// into two groups with the same size.
// However we were lucky enough that the first value returned
// is the correct answer.
long Part2(HashSet<long> numbers)
{
    long sumOfAll = numbers.Sum();
    Debug.Assert(sumOfAll % 4 == 0);
    long groupWeight = sumOfAll / 4;

    int maxCount = 1;

    while (true)
    {
        var combination =
            GetCombinations(numbers, groupWeight, maxCount)
            .Where(x => x.Count == maxCount)
            .OrderBy(x => x.Product())
            .Where(comb => IsValidGroup(comb, numbers))
            .FirstOrDefault();


        if (combination != null) return combination.Product();
        maxCount++;
    }
}

bool IsValidGroup(HashSet<long> group, HashSet<long> allNumbers)
{
    var remaining = new HashSet<long>(allNumbers.Except(group));
    var remainingCount = remaining.Count;

    return
        GetCombinationsDeep([], remaining, group.Sum())
        .Any(group => group.Count < remainingCount);


}

// Variant optimized to find combination with smallest size first
IEnumerable<HashSet<long>> GetCombinations(HashSet<long> numbers, long requestedWeight, int maxCount)
{
    PriorityQueue<(HashSet<long> SoFar, HashSet<long> Remaining, long SoFarCount, long SoFarSum), long> queue = new();
    queue.Enqueue(([], numbers, 0, 0), 0);

    while (queue.TryDequeue(out var task, out _))
    {
        if (task.SoFarCount > maxCount) continue;
        
        if (task.SoFarSum > requestedWeight) continue;

        if (task.SoFarSum == requestedWeight)
        {
            yield return task.SoFar;
            continue;
        }

        if (task.SoFarCount < maxCount)
        {
            long sumLeft = requestedWeight - task.SoFarSum;
            long itemsLeft = maxCount - task.SoFarCount;
            long minNumber = sumLeft / itemsLeft;

            foreach (var number in task.Remaining.OrderDescending())
            {
                if (number < minNumber) break;

                HashSet<long> newSoFar = [.. task.SoFar, number];
                queue.Enqueue((newSoFar, new HashSet<long>(task.Remaining.Except([number])), task.SoFarCount + 1, task.SoFarSum + number), task.SoFarCount + 1);
            }
        }

    }
}

// Variant optimized to find any combination fast
IEnumerable<HashSet<long>> GetCombinationsDeep(HashSet<long> soFar, HashSet<long> remaining, long requestedWeight)
{
    if (soFar.Sum() <= requestedWeight) 
    {
        if (soFar.Sum() == requestedWeight)
        {
            yield return soFar;
        }
        else
        {
            foreach (var number in remaining.OrderDescending())
            {
                var res = GetCombinationsDeep([.. soFar, number], new HashSet<long>(remaining.Except([number])), requestedWeight);
                foreach (var item in res)
                {
                    yield return item;
                }
            }
        }
    }
}

static class Extensions
{
    public static T Product<T>(this IEnumerable<T> list) where T: INumber<T>
    {
        return list.Aggregate(T.One, (a, b) => a * b);
    }
}
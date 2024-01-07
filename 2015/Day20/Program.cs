{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day20\Full.txt";

    int input = int.Parse(File.ReadAllText(fileName));

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}


long Part1(int presentsToReach)
{
    return GetHouseWithPresents(presentsToReach, getPresentCountPart1);
}

long Part2(int presentsToReach)
{
    return GetHouseWithPresents(presentsToReach, getPresentCountPart2);
}

int GetHouseWithPresents(int presentsToReach, Func<int, int[], int> getPresentCount)
{
    List<int[]> elvesAtHouse = [[0], [1], [1, 2]];

    int house = 3;
    while (true)
    {
        AddHouseInformation(elvesAtHouse, house);
        var presents = getPresentCount(house, elvesAtHouse[house]);

        if (presents >= presentsToReach) return house;

        house++;
    }
}

void AddHouseInformation(List<int[]> elvesAtHouse, int houseNumber)
{
    for(int i=2; i <= Math.Sqrt(houseNumber); i++)
    {
        if((houseNumber % i) == 0)
        {
            int div = houseNumber / i;

            int[] newElves = new HashSet<int>([1, ..elvesAtHouse[div], .. elvesAtHouse[div].Select(e => e * i)]).ToArray();

            elvesAtHouse.Add(newElves);
            return;
        }
    }
    elvesAtHouse.Add([1, houseNumber]);
}

int getPresentCountPart1(int houseNumber, int[] elves)
{
    return elves.Sum(e => e * 10);
}

int getPresentCountPart2(int houseNumber, int[] elves)
{
    int sum = 0;

    foreach(int elve in elves)
    {
        if (houseNumber > (50 * elve)) continue;

        sum += elve * 11;
    }

    return sum;
}
// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day13\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day13\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    SeatingHappiness seatingHappiness = new();
    foreach (string line in input)
    {
        var values = ParseLine(line);
        seatingHappiness.AddHappiness(values.Person1, values.Person2, values.Happiness);
    }

    Console.WriteLine("Part 1: " + Part1(seatingHappiness));
    Console.WriteLine("Part 2: " + Part2(seatingHappiness));
    Console.ReadLine();
}

long Part1(SeatingHappiness seatingHappiness)
{
    int[] seatings = seatingHappiness.GetAllSeatings();
    return seatings.Max();
}

long Part2(SeatingHappiness seatingHappiness)
{
    var persons = seatingHappiness.GetPersons();
    const string ego = "NineBerry";

    foreach (var person in persons)
    {
        seatingHappiness.AddHappiness(ego, person, 0);
        seatingHappiness.AddHappiness(person, ego, 0);
    }

    int[] seatings = seatingHappiness.GetAllSeatings();
    return seatings.Max();
}

(string Person1, string Person2, int Happiness) ParseLine(string line)
{
    var groups = Regex.Match(line, "(.*) would (.*) ([0-9]+) happiness units by sitting next to (.*).").Groups;

    int sign = groups[2].Value == "gain" ? 1 : -1;


    return (groups[1].Value, groups[4].Value, sign * int.Parse(groups[3].Value));
}


class SeatingHappiness : Dictionary<(string, string), int>
{
    public void AddHappiness(string person1, string person2, int happiness)
    {
        this[(person1, person2)] = happiness;
    }
    public int GetAbsoluteHappiness(string person1, string person2)
    {
        if (person1 == "" || person2 == "") return 0;

        return this[(person1, person2)] + this[(person2, person1)];
    }

    public HashSet<string> GetPersons()
    {
        var set1 = this.Keys.Select(k => k.Item1).ToHashSet();
        var set2 = this.Keys.Select(k => k.Item2).ToHashSet();

        return set1.Union(set2).ToHashSet();
    }

    public int[] GetAllSeatings()
    {
        var persons = GetPersons();
        string firstPerson = persons.First();
        
        return GetRecursiveSeating(0, firstPerson, firstPerson, persons.Except([firstPerson]));
    }

    private int[] GetRecursiveSeating(int currentHappiness, string firsPerson, string lastPerson, IEnumerable<string> personsLeft)
    {
        if (!personsLeft.Any()) return [currentHappiness + GetAbsoluteHappiness(lastPerson, firsPerson)];

        List<int> seatings = new();
        foreach (var person in personsLeft)
        {
            seatings.AddRange(GetRecursiveSeating(
                currentHappiness + GetAbsoluteHappiness(lastPerson, person),
                firsPerson,
                person,
                personsLeft.Except([person])));
        }

        return seatings.ToArray();
    }
}
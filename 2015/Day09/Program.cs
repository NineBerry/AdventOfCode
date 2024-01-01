// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day09\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day09\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Distances distances = new();
    foreach (string line in input)
    {
        var values = ParseLine(line);
        distances.AddDistance(values.Place1, values.Place2, values.Distance);
    }

    int[] pathDistances = distances.GetAllPathDistances();

    Console.WriteLine("Part 1: " + Part1(pathDistances));
    Console.WriteLine("Part 2: " + Part2(pathDistances));
    Console.ReadLine();
}

long Part1(int[] distances)
{
    return distances.Min();
}

long Part2(int[] distances)
{
    return distances.Max();
}

(string Place1, string Place2, int Distance) ParseLine(string line)
{
    var groups = Regex.Match(line, "(.*) to (.*) = (.*)").Groups;
    return (groups[1].Value, groups[2].Value, int.Parse(groups[3].Value));
}


class Distances: Dictionary<(string, string), int>
{
    public void AddDistance(string place1, string place2, int distance)
    {
        this[GetOrderedPlaceTuple(place1, place2)] = distance;
    }
    public int GetDistance(string place1, string place2)
    {
        if(place1 == "" || place2 == "") return 0;
        
        return this[GetOrderedPlaceTuple(place1, place2)];
    }

    private (string, string) GetOrderedPlaceTuple(string place1, string place2)
    {
        return place1.CompareTo(place2) > 0 ? (place1, place2) : (place2, place1);
    }

    public HashSet<string> GetPlaces()
    {
        var set1 = this.Keys.Select(k => k.Item1).ToHashSet();
        var set2 = this.Keys.Select(k => k.Item2).ToHashSet();

        return set1.Union(set2).ToHashSet();
    }

    public int[] GetAllPathDistances()
    {
        return GetRecursivePathDistances(0, "", GetPlaces());
    }

    private int[] GetRecursivePathDistances(int currentDistance, string lastPlace, IEnumerable<string> placesLeft)
    {
        if (!placesLeft.Any()) return [currentDistance];
        
        List<int> pathDistances = new();
        foreach (var place in placesLeft)
        {
            pathDistances.AddRange(GetRecursivePathDistances(
                currentDistance + GetDistance(lastPlace, place), 
                place, 
                placesLeft.Except([place])));
        }

        return pathDistances.ToArray();
    }
}
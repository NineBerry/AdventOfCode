// #define Sample

using System.Text.Json;
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day12\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day12\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string input)
{
    return Regex.Matches(input, "-?[0-9]+").Select(m => int.Parse(m.Value)).Sum();
}

long Part2(string input)
{
    var document = JsonDocument.Parse(input);
    var element = document.RootElement;

    return RecursiveCount(element);
}

long RecursiveCount(JsonElement element)
{
    long result = 0;

    switch (element.ValueKind)
    {
        case JsonValueKind.Object:

            var poisoned = false;

            foreach (var elementProperty in element.EnumerateObject())
            {
                if(elementProperty.Value.ValueKind == JsonValueKind.String 
                    && elementProperty.Value.GetString() == "red")
                {
                    poisoned = true; 
                    break;
                }
            }

            if (!poisoned)
            {
                foreach (var elementProperty in element.EnumerateObject())
                {
                    result += RecursiveCount(elementProperty.Value);
                }
            }
            break;
        case JsonValueKind.Array:
            foreach(var arrayElement in element.EnumerateArray())
            {
                result += RecursiveCount(arrayElement);
            }
            break;
        case JsonValueKind.Number:
            result += element.GetInt32();
            break;
        case JsonValueKind.String:
        case JsonValueKind.True:
        case JsonValueKind.False:
        case JsonValueKind.Null:
        case JsonValueKind.Undefined:
        default:
            break;
    }

    return result;
}

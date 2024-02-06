// #define Sample

using System.Text.RegularExpressions;
using Ticket = long[];

{

#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2020\Day16\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2020\Day16\Sample2.txt";
    string part2FieldStart = "seat";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2020\Day16\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2020\Day16\Full.txt";
    string part2FieldStart = "departure";
#endif

    ParseInput(fileNamePart1, out var fieldDefinitions, out var otherTickets, out var myTicket);
    Console.WriteLine("Part 1: " + Part1(fieldDefinitions, otherTickets));

    ParseInput(fileNamePart2, out fieldDefinitions, out otherTickets, out myTicket);
    Console.WriteLine("Part 2: " + Part2(fieldDefinitions, otherTickets, myTicket, part2FieldStart));
    Console.ReadLine();
}


void ParseInput(string fileName, out FieldDefinition[] fieldDefinitions, out Ticket[] otherTickets, out Ticket myTicket)
{
    string[] lines = File.ReadAllLines(fileName);

    fieldDefinitions =
        lines
        .TakeWhile(s => s != "")
        .Select(s => new FieldDefinition(s))
        .ToArray();

    myTicket =
        lines
        .SkipWhile(s => s != "your ticket:")
        .Skip(1)
        .Take(1)
        .Select(ParseTicket)
        .Single();

    otherTickets =
        lines
        .SkipWhile(s => s != "nearby tickets:")
        .Skip(1)
        .Select(ParseTicket)
        .ToArray();
}


long Part1(FieldDefinition[] fieldDefinitions, Ticket[] otherTickets)
{
    return DetermineInvalidValues(fieldDefinitions, otherTickets).Sum();
}

long Part2(FieldDefinition[] fieldDefinitions, Ticket[] otherTickets, Ticket myTicket, string fieldStart)
{
    long[] invalidValues = DetermineInvalidValues(fieldDefinitions, otherTickets);
    Ticket[] validTickets = otherTickets.Where(t => !invalidValues.Intersect(t).Any()).ToArray();

    DetermineFieldOrders(fieldDefinitions, validTickets);

    var interestingFieldDefinitions = fieldDefinitions.Where(def => def.Name.StartsWith(fieldStart));

    var interestingValues = interestingFieldDefinitions.Select(def => myTicket[def.TicketOrder!.Value]);

    return interestingValues.Aggregate((a, b) => a * b);
}

long[] DetermineInvalidValues(FieldDefinition[] fieldDefinitions, Ticket[] otherTickets)
{
    long[] allValues = otherTickets.SelectMany(s => s).ToArray();
    InclusiveRange[] allRanges = fieldDefinitions.SelectMany(s => s.Ranges).ToArray();
    return allValues.Where(value => allRanges.All(range => !range.Contains(value))).ToArray();
}

void DetermineFieldOrders(FieldDefinition[] fieldDefinitions, Ticket[] validTickets)
{
    HashSet<FieldDefinition> unassignedFieldDefinitions = new(fieldDefinitions);
    HashSet<int> unassignedOrders = new(Enumerable.Range(0, fieldDefinitions.Length));

    while (unassignedOrders.Any())
    {
        Dictionary<int, HashSet<FieldDefinition>> possibleDefinitionsForOrder = [];

        foreach(int order in unassignedOrders)
        {
            foreach(var fieldDef in unassignedFieldDefinitions)
            {
                if(validTickets.All(ticket => fieldDef.Ranges.Any(r => r.Contains(ticket[order]))))
                {
                    if(!possibleDefinitionsForOrder.TryGetValue(order, out var set))
                    {
                        set = new();
                        possibleDefinitionsForOrder.Add(order, set);

                    }
                    set.Add(fieldDef);
                }
            }
        }

        foreach(var pair in possibleDefinitionsForOrder.Where(p => p.Value.Count == 1))
        {
            var fieldDef = pair.Value.Single();
            var order = pair.Key;
            
            fieldDef.TicketOrder = order;
            unassignedFieldDefinitions.Remove(fieldDef);
            unassignedOrders.Remove(order);
        }
    }
}


Ticket ParseTicket(string line)
{
    return line.Split(',').Select(long.Parse).ToArray();
}

public record InclusiveRange
{
    public InclusiveRange(long start, long end)
    {
        Start = start;
        End = end;
    }

    public bool Contains(long value)
    {
        return value >= Start && value <= End;
    }

    public long Start;
    public long End;
}

public class FieldDefinition
{
    public FieldDefinition(string line)
    {
        Name = line.Split(':')[0];
        
        long[] values = Regex.Matches(line, "\\d+").Select(m => long.Parse(m.Value)).ToArray();
        Ranges = [new InclusiveRange(values[0], values[1]), new InclusiveRange(values[2], values[3])];
    }

    public string Name;
    public InclusiveRange[] Ranges;
    public int? TicketOrder = null; 
}
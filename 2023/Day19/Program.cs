using System.Text;
using System.Text.RegularExpressions;


// string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day19\Sample.txt";
string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day19\Full.txt";

string[] input = File.ReadAllLines(fileName);

var workflowInput = input.TakeWhile(s=> s != "").ToArray();
Dictionary<string, Workflow> workflows = BuildWorkflows(workflowInput);

long part1 = Part1(input, workflows);
Console.WriteLine($"Part 1: " + part1);

long part2 = Part2(workflows);
Console.WriteLine($"Part 2: {part2}");

Console.ReadLine();


/// <summary>
/// Part 1
/// </summary>
long Part1(string[] input, Dictionary<string, Workflow> workflows)
{
    var machinePartInput = input.SkipWhile(s => s != "").Skip(1).ToArray();
    
    long sum = 0;
    foreach (var machinePart in machinePartInput.Select(s => new MachinePart(s)))
    {
        if (ProcessMachinePart(machinePart, workflows))
        {
            sum += machinePart.Rating();
        }
    }
    return sum;
}

/// <summary>
/// Part 2
/// </summary>
long Part2(Dictionary<string, Workflow> workflows)
{
    var foundRanges = FindValidPropertyRanges(Workflow.StartWorkflow, 0, new MachinePartyPropertyRanges(), workflows);
    
    return foundRanges.Sum(ranges => ranges.CalculatePossibilities());
}


/// <summary>
/// Parse Workflows into dictionary
/// </summary>
Dictionary<string, Workflow> BuildWorkflows(string[] input)
{
    Dictionary<string, Workflow> workflows = new();

    foreach (string line in input)
    {
        Workflow workflow = new Workflow(line);
        workflows.Add(workflow.Name, workflow);
    }

    return workflows;
}

/// <summary>
/// For Part 1: Process 1 Machine Part and return whether it is accepted
/// </summary>
bool ProcessMachinePart(MachinePart machinePart, Dictionary<string, Workflow> workflows)
{
    string workflowName = Workflow.StartWorkflow;

    while (workflowName is not (Workflow.RejectedWorkflow or Workflow.AcceptedWorkflow))
    {
        var workflow = workflows[workflowName];
        workflowName = workflow.Apply(machinePart);
    }

    return workflowName == Workflow.AcceptedWorkflow;
}

/// <summary>
/// Recursive algorithm for Part 2:
/// 
/// Perform the given Rule on the given range.
/// Retrieve the resulting next steps. Branch on the next steps.
/// 
/// If Accept state was found, return the valid ranges that reached the
/// Accept state.
/// </summary>

MachinePartyPropertyRanges[] FindValidPropertyRanges(string workflowName, int workflowRuleIndex, MachinePartyPropertyRanges ranges, Dictionary<string, Workflow> workflows)
{
    if (!ranges.IsPossible) return [];
    if (workflowName == Workflow.RejectedWorkflow) return [];
    if (workflowName == Workflow.AcceptedWorkflow) return [ranges];

    Workflow workflow = workflows[workflowName];
    if (workflowRuleIndex >= workflow.Rules.Count) return [];

    var nextSteps = workflow.Rules[workflowRuleIndex].Apply(ranges);

    List<MachinePartyPropertyRanges> result = new();

    foreach (var next in nextSteps) 
    { 
        if(next.WorkflowName == "")
        {
            result.AddRange(FindValidPropertyRanges(workflowName, workflowRuleIndex + 1, next.Ranges, workflows));
        }
        else
        {
            result.AddRange(FindValidPropertyRanges(next.WorkflowName, 0, next.Ranges, workflows));
        }
    }

    return result.ToArray();
}


/// <summary>
/// MachinePartProperty
/// </summary>

enum MachinePartProperty
{
    ExtremelyCoolLooking = 'x',
    Musical = 'm',
    Aerodynamic = 'a',
    Shiny = 's'
}

/// <summary>
/// MachinePart
/// </summary>

record struct MachinePart
{
    public MachinePart(string input)
    {
        var matches = Regex.Matches(input, "[a-z]=[0-9]+").Select(m => m.Value);
        foreach (string match in matches) 
        {
            MachinePartProperty property = (MachinePartProperty)match[0];
            int value = int.Parse(match.Substring(2));
            
            Properties[property] = value;
        }
    }

    public Dictionary<MachinePartProperty, int> Properties = new();

    internal long Rating()
    {
        long sum = 0;
        
        foreach(var property in AllMachinePartProperties)
        {
            sum += Properties[property];
        }
        
        return sum;
    }

    public static MachinePartProperty[] AllMachinePartProperties =
        [
            MachinePartProperty.ExtremelyCoolLooking,
            MachinePartProperty.Musical,
            MachinePartProperty.Aerodynamic,
            MachinePartProperty.Shiny
        ];

    public const int MinPropertyValue = 1;
    public const int MaxPropertyValue = 4000;
}

/// <summary>
/// Operator
/// </summary>

enum Operator
{
    LessThan = '<',
    GreaterThan = '>'
}

/// <summary>
/// Condition
/// </summary>

record struct Condition
{
    public Condition(string input)
    {
        Property = (MachinePartProperty)input[0];
        Operator = (Operator)input[1];
        Value = int.Parse(input.Substring(2));
    }

    public MachinePartProperty Property;
    public Operator Operator;
    public int Value;

    public bool Apply(MachinePart machinePart)
    {
        return Operator switch
        {
            Operator.LessThan => machinePart.Properties[Property] < Value,
            Operator.GreaterThan => machinePart.Properties[Property] > Value,
            _ => throw new ApplicationException("Unknown operator")
        }; 
    }
}

/// <summary>
/// WorkflowRule
/// </summary>

record struct WorkflowRule
{
    public WorkflowRule(string input)
    {
        string[] parts = input.Split(':');

        if (parts.Length == 2)
        {
            Condition = new Condition(parts[0]);
            NextWorkflow = parts[1];
        }
        else if (parts.Length == 1)
        {
            NextWorkflow = parts[0];
        }
        else throw new ApplicationException("Unexptected input");
    }
        

    public Condition? Condition = null;
    public string NextWorkflow;

    public string Apply(MachinePart machinePart)
    {
        if (Condition.HasValue)
        {
            if (Condition.Value.Apply(machinePart))
            {
                return NextWorkflow;
            }
        }
        else
        {
            return NextWorkflow;
        }

        return "";
    }

    /// <summary>
    /// Applies the range to the rule.
    /// Returns a set of next steps.
    /// Each next step contains the name of the next workflow to perform. 
    /// If empty, remain in the same workflow but apply the next rule.
    /// Each next step contains the set of valid ranges for the next step.
    /// </summary>
    public (string WorkflowName, MachinePartyPropertyRanges Ranges)[] Apply(MachinePartyPropertyRanges ranges)
    {
        if (Condition.HasValue)
        {
            var range = ranges[Condition.Value.Property];
            var splitted = range.Split(Condition.Value);

            var trueClone = ranges.Clone();
            trueClone[Condition.Value.Property] = splitted[0];

            var falseClone = ranges.Clone();
            falseClone[Condition.Value.Property] = splitted[1];

            return [(NextWorkflow, trueClone), ("", falseClone)];
        }
        else 
        {
            return [(NextWorkflow, ranges)];
        }
    }
}

/// <summary>
/// Workflow
/// </summary>

record struct Workflow
{
    public Workflow(string input)
    {
        Name = Regex.Match(input, "[a-z]+").Value;

        string rules = Regex.Match(input, "\\{(.*)\\}").Groups[1].Value;
        foreach (var ruleString in rules.Split(',')) 
        {
            Rules.Add(new WorkflowRule(ruleString));
        }
    }

    public string Name;
    public List<WorkflowRule> Rules = new List<WorkflowRule>();

    public string Apply(MachinePart machinePart)
    {
        foreach (var rule in Rules) 
        { 
            string nextWorkflow = rule.Apply(machinePart);
            if (nextWorkflow != "")
            {
                return nextWorkflow;
            }
        }

        throw new ApplicationException("Workflow ended without next workflow");
    }

    public const string StartWorkflow = "in";
    public const string AcceptedWorkflow = "A";
    public const string RejectedWorkflow = "R";
}

/// <summary>
/// Range
/// 
/// Manages a Range of numbers that can split
/// based on a condition 
/// </summary>

record struct Range(int InclusiveStart, int InclusiveEnd)
{

    /// <summary>
    /// First Range returned fullfils the condition.
    /// Second Range returned doesn't fulfill the condition
    /// </summary>
    public Range[] Split(Condition condition)
    {
        return condition.Operator switch
        {
            Operator.LessThan => [
                new Range(InclusiveStart, Math.Min(InclusiveEnd, condition.Value - 1)),
                new Range(Math.Max(InclusiveStart, condition.Value), InclusiveEnd)
                ],
            Operator.GreaterThan => [
                new Range(Math.Max(InclusiveStart, condition.Value + 1), InclusiveEnd),
                new Range(InclusiveStart, Math.Min(InclusiveEnd, condition.Value))
                ],
             _ => throw new ApplicationException("Unknown operator")
        };
    }

    public bool IsPossible => InclusiveEnd >= InclusiveStart && (InclusiveStart != 0);

    public override string ToString()
    {
        return $"[{InclusiveStart},{InclusiveEnd}]";
    }
}

/// <summary>
/// MachinePartyPropertyRanges 
/// 
/// Manages a set of Ranges, one range
/// for each property of a Machine part
/// </summary>

class MachinePartyPropertyRanges : Dictionary<MachinePartProperty, Range>
{
    public MachinePartyPropertyRanges()
    {
        foreach(var property in MachinePart.AllMachinePartProperties)
        {
            Add(property, new Range(MachinePart.MinPropertyValue, MachinePart.MaxPropertyValue));
        }
    }

    public MachinePartyPropertyRanges Clone()
    {
        MachinePartyPropertyRanges clone = new MachinePartyPropertyRanges();

        foreach (var property in MachinePart.AllMachinePartProperties)
        {
            clone[property] = this[property];
        }

        return clone;
    }

    public bool IsPossible => Values.All(property => property.IsPossible);

    public long CalculatePossibilities()
    {
        if (!IsPossible) return 0;

        long product = 1;

        foreach (var property in MachinePart.AllMachinePartProperties)
        {
            long diff = this[property].InclusiveEnd - this[property].InclusiveStart + 1;
            product *= diff;
        }

        return product;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var property in MachinePart.AllMachinePartProperties)
        {
            sb.Append($"{(char)property}={this[property]}");
        }

        return sb.ToString();
    }
}
// #define Sample

using System.Linq;
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day24\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day24\Full.txt";
#endif

    Console.WriteLine("Part 1: " + Part1(fileName));
    Console.WriteLine("Part 2: " + Part2(fileName));
    Console.ReadLine();
}

long Part1(string fileName)
{
    Game game = new Game(fileName, 0);
    var result = game.RunGame();
    return Math.Max(result.ImmuneSystemLeft, result.InfectionLeft);
}

long Part2(string fileName)
{
    int boost = 0;
    try
    {
        while (true)
        {
            Console.Write("\rTesting boost " + boost);
            Game game = new Game(fileName, boost);

            try
            {

                var result = game.RunGame();

                if (result.ImmuneSystemLeft > 0) return result.ImmuneSystemLeft;
            }
            catch (FreezeDetectedException)
            {
                // Ignore freeze, just try next boost value
            }

            boost++;
        }
    }
    finally
    {
        // Clear current line
        Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
    }
}


public class Game
{
    public Army ImmuneSystem;
    public Army Infection;

    public Game(string fileName, int immuneSystemBoost)
    {
        string[] armyDescriptions = File.ReadAllText(fileName).ReplaceLineEndings("\n").Split("\n\n");

        ImmuneSystem = new Army(armyDescriptions[0], immuneSystemBoost);
        Infection = new Army(armyDescriptions[1], 0);
    }

    public bool Fight()
    {
        int unitsBefore = ImmuneSystem.TotalUnits + Infection.TotalUnits;

        (Group Attacker, Group Attacked)[] attacks = TargetSelection();

        foreach (var attack in attacks.OrderByDescending(g => g.Attacker.Initiative).ToArray())
        {
            attack.Attacker.Attack(attack.Attacked);
        }

        int unitsAfter = ImmuneSystem.TotalUnits + Infection.TotalUnits;
        if (unitsAfter == unitsBefore) throw new FreezeDetectedException();

        return ImmuneSystem.TotalUnits > 0 && Infection.TotalUnits > 0;
    }

    private (Group Attacker, Group Attacked)[] TargetSelection()
    {
        List<(Group Attacker, Group Attacked)> result = [];

        HashSet<Group> chosenTargets = new HashSet<Group>();  
        var attackers = 
            ImmuneSystem.Groups.Union(Infection.Groups)
            .Where(g => g.Alive)
            .OrderByDescending(g => g.EffectivePower)
            .ThenByDescending(g => g.Initiative)
            .ToArray();

        foreach (var attacker in attackers)
        {
            List<Group> potentialTargets = [];
            if (ImmuneSystem.Groups.Contains(attacker))
            { 
                potentialTargets = Infection.Groups;
            }
            else
            {
                potentialTargets = ImmuneSystem.Groups;
            }

            potentialTargets = potentialTargets.Except(chosenTargets).Where(g => g.Alive).ToList();

            Group? chosen = attacker.SelectTarget(potentialTargets);

            if(chosen != null)
            {
                chosenTargets.Add(chosen);
                result.Add((attacker, chosen));
            }
        }

        return result.ToArray();
    }

    public (int ImmuneSystemLeft, int InfectionLeft) RunGame()
    {
        while (Fight()) ;
        return (ImmuneSystem.TotalUnits, Infection.TotalUnits);
    }
}

public class Army
{
    public List<Group> Groups;

    public Army(string description, int boost)
    {
        Groups = description.Split("\n").Skip(1).Select(s => new Group(s, boost)).ToList();
    }

    public int TotalUnits => Groups.Sum(g => g.Units);
}


public class Group
{
    public int Units;
    public int HitPointsPerUnit;
    public int Initiative;
    public int AttackDamage;
    public string DamageType;
    public HashSet<string> WeakTo = [];
    public HashSet<string> ImmuneTo = [];

    public Group(string description, int boost)
    {
        Units = int.Parse(Regex.Match(description, "(\\d+) units").Groups[1].Value);
        HitPointsPerUnit = int.Parse(Regex.Match(description, "(\\d+) hit points").Groups[1].Value);
        Initiative = int.Parse(Regex.Match(description, "initiative (\\d+)").Groups[1].Value);
        AttackDamage = int.Parse(Regex.Match(description, "does (\\d+)").Groups[1].Value) + boost;

        DamageType = Regex.Match(description, "does \\d+ (.*) damage").Groups[1].Value;

        WeakTo = Regex.Match(description, "weak to ([a-z, ]*)").Groups[1].Value.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        ImmuneTo = Regex.Match(description, "immune to ([a-z, ]*)").Groups[1].Value.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToHashSet();
    }

    public bool Alive => Units > 0;
    public int EffectivePower => AttackDamage * Units;

    public void Attack(Group attacked)
    {
        if (!Alive) return;

        int damage = CalculatePotentialDamage(attacked);
        attacked.Attack(damage);
    }

    private int CalculatePotentialDamage(Group attacked)
    {
        int damage = EffectivePower;

        if (attacked.WeakTo.Contains(DamageType))
        {
            damage *= 2;
        }
        
        if (attacked.ImmuneTo.Contains(DamageType))
        {
            damage = 0;
        };

        return damage;
    }

    public void Attack(int damage)
    {
        int unitsKilled = damage / HitPointsPerUnit;
        Units = Math.Max(0, Units - unitsKilled);
    }

    public Group? SelectTarget(List<Group> potentialTargets)
    {
        return potentialTargets
            .Where(g => CalculatePotentialDamage(g) != 0)
            .OrderByDescending(CalculatePotentialDamage)
            .ThenByDescending(g => g.EffectivePower)
            .ThenByDescending(g => g.Initiative)
            .FirstOrDefault();
    }
}

public class FreezeDetectedException : Exception{ }

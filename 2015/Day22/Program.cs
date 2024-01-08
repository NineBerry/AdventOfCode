using System.Text.RegularExpressions;

string gFileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day22\Full.txt";
{

    Console.WriteLine("Part 1: " + Part1());
    Console.WriteLine("Part 2: " + Part2());
    Console.ReadLine();
}

long Part1()
{
    return PlayGame(hard: false);
}

long Part2()
{
    return PlayGame(hard: true);
}

long PlayGame(bool hard)
{
    PriorityQueue<Game, int> queue = new();

    Game start = new Game 
    { 
        Boss = Player.CreateBossFromFile(gFileName), 
        Player = Player.CreatePlayer() 
    };

    queue.Enqueue(start, 0);

    while(queue.TryDequeue(out var currentGame, out _))
    {
        if (hard)
        {
            currentGame = currentGame with { Player = currentGame.Player with { HitPoints = currentGame.Player.HitPoints - 1 } };
        }

        if (currentGame.BossWon) continue;

        if (currentGame.PlayerWon) return currentGame.ManaSpent;

        Game[] afterTurns = Game.PerformTurn(currentGame); 
        foreach(Game game in afterTurns)
        {
            queue.Enqueue(game, game.ManaSpent);
        }
    }
    
    return 0;
}


public enum SpellType
{
    MagicMissile,
    Drain,
    Shield,
    Poison,
    Recharge
}

public readonly record struct Game
{
    public Game() { }

    public static Game[] PerformTurn(Game game)
    {
        (game, int armor) = ApplyEffects(game);

        if (game.PlayerWon || game.BossWon) return [game];

        bool bossNext = game.NextTurnBoss;
        game = game with { NextTurnBoss = !game.NextTurnBoss };

        if (bossNext)
        {
            int damage = Math.Max(1, game.Boss.Damage - (game.Player.Armor + armor));
            game = game with { Player = game.Player with { HitPoints = game.Player.HitPoints - damage } };

            return [game];
        }
        else
        {
            return 
                game
                .GetPossibleSpells()
                .Select(spell => ApplySpell(game, spell))
                .ToArray();
        }
    }

    private static Game ApplySpell(Game game, SpellType spell)
    {
        game = game with { ManaSpent = game.ManaSpent + spell.GetSpellCost() };
        game = game with { Player = game.Player with { Mana = game.Player.Mana - spell.GetSpellCost() } };

        switch (spell)
        {
            case SpellType.MagicMissile:
                game = game with { Boss = game.Boss with { HitPoints = game.Boss.HitPoints - 4 } };
                return game;
            case SpellType.Drain:
                game = game with { Boss = game.Boss with { HitPoints = game.Boss.HitPoints - 2 } };
                game = game with { Player = game.Player with { HitPoints = game.Player.HitPoints + 2 } };
                return game;
            case SpellType.Shield:
                game = game with { ShieldEffectTurns = 6 };
                return game;
            case SpellType.Poison:
                game = game with { PoisonEffectTurns = 6 };
                return game;
            case SpellType.Recharge:
                game = game with { RechargeEffectTurns = 5 };
                return game;
            default:
                throw new ApplicationException("Unknown Spell");
        }
    }

    private SpellType[] GetPossibleSpells()
    {
        List<SpellType> possibleSpells = new();

        if(Player.Mana >= SpellType.MagicMissile.GetSpellCost())
        {
            possibleSpells.Add(SpellType.MagicMissile);
        }

        if (Player.Mana >= SpellType.Drain.GetSpellCost())
        {
            possibleSpells.Add(SpellType.Drain);
        }

        if (Player.Mana >= SpellType.Shield.GetSpellCost() && ShieldEffectTurns == 0)
        {
            possibleSpells.Add(SpellType.Shield);
        }

        if (Player.Mana >= SpellType.Poison.GetSpellCost() && PoisonEffectTurns == 0)
        {
            possibleSpells.Add(SpellType.Poison);
        }

        if (Player.Mana >= SpellType.Recharge.GetSpellCost() && RechargeEffectTurns == 0)
        {
            possibleSpells.Add(SpellType.Recharge);
        }

        return possibleSpells.ToArray();
    }

    private static (Game modifiedGame, int armor) ApplyEffects(Game game)
    {
        int armor = 0;

        if (game.ShieldEffectTurns > 0)
        {
            armor = 7;
            game = game with { ShieldEffectTurns = game.ShieldEffectTurns - 1 };
        }

        if (game.PoisonEffectTurns > 0)
        {
            game = game with { Boss = game.Boss with { HitPoints = game.Boss.HitPoints - 3 } };
            game = game with { PoisonEffectTurns = game.PoisonEffectTurns - 1 };
        }

        if (game.RechargeEffectTurns > 0)
        {
            game = game with { Player = game.Player with { Mana = game.Player.Mana + 101 } };
            game = game with { RechargeEffectTurns = game.RechargeEffectTurns - 1 };
        }


        return (game, armor);
    }

    public bool NextTurnBoss { get; init; } = false; 

    public Player Player { get; init; }
    public Player Boss { get; init; }
    public int ManaSpent { get; init; } = 0;

    public int ShieldEffectTurns { get; init; } = 0;
    public int PoisonEffectTurns { get; init; } = 0;
    public int RechargeEffectTurns { get; init; } = 0;

    public bool BossWon => Player.HitPoints <= 0;
    public bool PlayerWon => Boss.HitPoints <= 0;
}


public readonly record struct Player
{
    public Player() {}

    public string Name {get; init;} = "";
    public int HitPoints { get; init; } = 0;
    public int Damage { get; init; } = 0;
    public int Armor { get; init; } = 0;
    public int Mana { get; init; } = 0;    

    public static Player CreateBossFromFile(string fileName)
    {
        int[] values = Regex
            .Matches(File.ReadAllText(fileName), "\\d+")
            .Select(m => int.Parse(m.Value))
            .ToArray();

        return new Player { Name = "Boss", HitPoints = values[0], Damage = values[1], Mana = 1 };
    }

    public static Player CreatePlayer()
    {
        return new Player { Name = "NineBerry", HitPoints = 50, Mana = 500, Damage = 0, Armor = 0 };
    }
}

public static class Extensions
{
    public static int GetSpellCost(this SpellType type) => type switch
    {
        SpellType.MagicMissile => 53,
        SpellType.Drain => 73,
        SpellType.Shield => 113,
        SpellType.Poison => 173,
        SpellType.Recharge => 229,
        _ => throw new NotImplementedException(),
    };
}
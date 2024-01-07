using System.Text.RegularExpressions;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day21\Full.txt";

    Player boss = Player.CreateBossFromFile(fileName);
    Basket[] baskets = GetPossibleBaskets();

    Console.WriteLine("Part 1: " + Part1(boss, baskets));
    Console.WriteLine("Part 2: " + Part2(boss, baskets));
    Console.ReadLine();
}


long Part1(Player boss, Basket[] baskets)
{
    return 
        baskets
        .Where(basket => Player.CanPlayerWin(Player.CreatePlayer(basket), boss))
        .Min(basket => basket.Cost);
}

long Part2(Player boss, Basket[] baskets)
{
    return
        baskets
        .Where(basket => !Player.CanPlayerWin(Player.CreatePlayer(basket), boss))
        .Max(basket => basket.Cost);
}

Basket[] GetPossibleBaskets()
{
    Basket[] items = ChooseWeapon();

    Basket[] ChooseWeapon()
    {
        List<Basket> list = new();

        // 1 weapon
        foreach(var item in Item.Weapons)
        {
            list.AddRange(ChooseArmor([item]));
        }

        return list.ToArray();  
    }

    Basket[] ChooseArmor(Item[] items)
    {
        List<Basket> list = new();

        // No armor
        list.AddRange(ChooseRings(items));

        // 1 armor piece
        foreach (var item in Item.ArmorItems)
        {
            list.AddRange(ChooseRings([..items, item]));
        }

        return list.ToArray();
    }

    Basket[] ChooseRings(Item[] items)
    {
        List<Basket> list = new();

        //No ring
        list.Add(new Basket(items));

        // One ring
        foreach (var item in Item.Rings)
        {
            list.Add(new Basket([.. items, item]));
        }

        // Two rings
        for(int i=0; i < Item.Rings.Length - 1; i++)
        {
            for (int j = i + 1; j < Item.Rings.Length; j++)
            {
                list.Add(new Basket([.. items, Item.Rings[i], Item.Rings[j]]));
            }
        }

        return list.ToArray();
    }

    return items.ToArray();

}

public record Basket
{
    public Basket(Item[] items)
    {
        Items = items;  
    }

    public Item[] Items = [];
    public int Cost => Items.Sum(x => x.Cost);
    public int Damage => Items.Sum(x => x.Damage);
    public int Armor => Items.Sum(x => x.Armor);
}

public record Item
{
    public string Name = "";
    public int Cost = 0;
    public int Damage = 0;
    public int Armor = 0;

    public static Item Dagger = new Item { Name = "Dagger", Cost = 8, Damage = 4, Armor = 0 };
    public static Item Shortsword = new Item { Name = "Shortsword", Cost = 10, Damage = 5, Armor = 0 };
    public static Item Warhammer = new Item { Name = "Warhammer", Cost = 25, Damage = 6, Armor = 0 };
    public static Item Longsword = new Item { Name = "Longsword", Cost = 40, Damage = 7, Armor = 0 };
    public static Item Greataxe = new Item { Name = "Greataxe", Cost = 74, Damage = 8, Armor = 0 };
    
    public static Item Leather = new Item { Name = "Leather", Cost = 13, Damage = 0, Armor = 1 };
    public static Item Chainmail = new Item { Name = "Chainmail", Cost = 31, Damage = 0, Armor = 2 };
    public static Item Splintmail = new Item { Name = "Splintmail", Cost = 53, Damage = 0, Armor = 3 };
    public static Item Bandedmail = new Item { Name = "Bandedmail", Cost = 75, Damage = 0, Armor = 4 };
    public static Item Platemail = new Item { Name = "Platemail", Cost = 102, Damage = 0, Armor = 5 };

    public static Item RingDamage1 = new Item { Name = "RingDamage1", Cost = 25, Damage = 1, Armor = 0 };
    public static Item RingDamage2 = new Item { Name = "RingDamage2", Cost = 50, Damage = 2, Armor = 0 };
    public static Item RingDamage3 = new Item { Name = "RingDamage3", Cost = 100, Damage = 3, Armor = 0 };
    public static Item RingDefense1 = new Item { Name = "RingDefense1", Cost = 20, Damage = 0, Armor = 1 };
    public static Item RingDefense2 = new Item { Name = "RingDefense2", Cost = 40, Damage = 0, Armor = 2 };
    public static Item RingDefense3 = new Item { Name = "RingDefense3", Cost = 80, Damage = 0, Armor = 3 };

    public static Item[] Weapons = [Dagger, Shortsword, Warhammer, Longsword, Greataxe];
    public static Item[] ArmorItems = [Leather, Chainmail, Splintmail, Bandedmail, Platemail];
    public static Item[] Rings = [RingDamage1, RingDamage2, RingDamage3, RingDefense1, RingDefense2, RingDefense3];
}

public record Player
{
    public string Name = "";
    public int HitPoints = 0;
    public int Damage = 0;
    public int Armor = 0;

    public static Player CreateBossFromFile(string fileName)
    {
        Player boss = new Player();
        int[] values = Regex
            .Matches(File.ReadAllText(fileName), "\\d+")
            .Select(m => int.Parse(m.Value))
            .ToArray();

        boss.Name = "Boss";
        boss.HitPoints = values[0];
        boss.Damage = values[1];
        boss.Armor = values[2];

        return boss;
    }
    public static Player CreatePlayer(Basket basket)
    {
        Player player = new Player();
        player.Name = "NineBerry";
        player.HitPoints = 100;

        player.Damage = basket.Damage;
        player.Armor = basket.Armor;

        return player;
    }

    public static bool CanPlayerWin(Player player, Player boss)
    {
        int actualDamageToBoss = Math.Max(1, player.Damage - boss.Armor);
        int actualDamageToPlayer = Math.Max(1, boss.Damage - player.Armor);
        int roundsToZeroBoss = (int)Math.Ceiling((double)boss.HitPoints / actualDamageToBoss);
        int roundsToZeroPlayer = (int)Math.Ceiling((double)player.HitPoints / actualDamageToPlayer);

        return roundsToZeroPlayer >= roundsToZeroBoss;
    }
}
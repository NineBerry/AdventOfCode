// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day04\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day04\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName).Order().ToArray();
    var statistic = new ElvesSleepStatistic(input);

    Console.WriteLine("Part 1: " + Part1(statistic));
    Console.WriteLine("Part 2: " + Part2(statistic));
    Console.ReadLine();
}

long Part1(ElvesSleepStatistic statistic)
{
    var mostSleepingElf = statistic.Values.OrderByDescending(elf => elf.Values.Sum()).First();
    var minute = mostSleepingElf.OrderByDescending(min => min.Value).Select(min => min.Key).First();

    return minute * mostSleepingElf.ID;
}

long Part2(ElvesSleepStatistic statistic)
{
    var mostSleepingElf = statistic.Values.OrderByDescending(elf => elf.Values.Count == 0? 0 : elf.Values.Max()).First();
    var minute = mostSleepingElf.OrderByDescending(min => min.Value).Select(min => min.Key).First();

    return minute * mostSleepingElf.ID;
}

public class ElfSleepStatistic: Dictionary<int, int>
{
    public ElfSleepStatistic(int id)
    {
        ID = id;
    }

    public int ID;

    public void AddRange(int from, int to)
    {
        for(int i=from; i <= to; i++)
        {
            TryGetValue(i, out int counter);
            this[i] = counter + 1;
        }
    }
}

public class ElvesSleepStatistic : Dictionary<int, ElfSleepStatistic>
{
    public ElvesSleepStatistic(string[] input)
    {
        ElfSleepStatistic elfStatistic = null;
        int startSleep = 0;

        foreach (var line in input)
        {
            var minute = int.Parse(line.Substring(15, 2));
            var command = line[19];

            switch (command)
            {
                case 'G':
                    int elfID = int.Parse(Regex.Match(line, "#(\\d+)").Groups[1].Value);
                    elfStatistic = GetStaticForElf(elfID);
                    break;
                case 'f':
                    startSleep = minute;
                    break;
                case 'w':
                    elfStatistic?.AddRange(startSleep, minute - 1);
                    break;
                default:
                    throw new ApplicationException("Unknown command");
            }
        }
    }

    public ElfSleepStatistic GetStaticForElf(int elf)
    {
        if(!TryGetValue(elf, out var statistic))
        {
            statistic = new ElfSleepStatistic(elf);
            Add(elf, statistic);
        }

        return statistic;
    }
}

// #define Sample
using System.Text;
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day16\Sample.txt";
    int countPrograms = 5;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day16\Full.txt";
    int countPrograms = 16;
#endif

    string[] commands = File.ReadAllText(fileName).Split(',');

    Console.WriteLine("Part 1: " + PerformDances(commands, countPrograms, 1));
    Console.WriteLine("Part 2: " + PerformDances(commands, countPrograms, 1_000_000_000));
    Console.ReadLine();
}

string PerformDances(string[] commands, int countPrograms, int countDances)
{
    Dictionary<string, int> seenAt = [];
    bool alreadySeen = false;

    StringBuilder programs = new StringBuilder(MakeInitialPrograms(countPrograms));
    int dances = 0;

    while (dances < countDances)
    {
        foreach (string command in commands)
        {
            ApplyCommand(programs, command);
        }
        dances++;

        if (!alreadySeen) 
        {
            string current = programs.ToString();

            if (seenAt.TryGetValue(current, out int danceSeenAfter))
            {
                alreadySeen = true;

                int cycleLength = dances - danceSeenAfter;
                int dancesStillToGo = countDances - dances;
                int cyclesStillToGo = dancesStillToGo / cycleLength;
                dances += cyclesStillToGo * cycleLength;

            }
            else
            {
                seenAt.Add(current, dances);
            }
        }
    }

    return programs.ToString();
}

string MakeInitialPrograms(int countPrograms)
{
    string result = "";

    foreach (int offset in Enumerable.Range(0, countPrograms))
    {
        char letter = (char)('a' + offset);
        result += letter;
    }

    return result;
}

void ApplyCommand(StringBuilder programs, string command)
{
    switch (command[0])
    {
        case 's':
            int size = int.Parse(command.Substring(1));
            Spin(programs, size);
            break;
        case 'x':
            int[] ints = Regex.Matches(command, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
            Exchange(programs, ints[0], ints[1]);
            break;
        case 'p':
            Partner(programs, command[1], command[3]);
            break;
        default:
            throw new ApplicationException("Invalid command");
    }
}

void Partner(StringBuilder programs, char ch1, char ch2)
{
    programs.Replace(ch1, '*');
    programs.Replace(ch2, ch1);
    programs.Replace('*', ch2);
}

void Exchange(StringBuilder programs, int pos1, int pos2)
{
    char temp = programs[pos1];
    programs[pos1] = programs[pos2];
    programs[pos2] = temp;
}

void Spin(StringBuilder programs, int size)
{
    string moving = programs.ToString().Substring(programs.Length - size);
    programs.Remove(programs.Length - size, size);
    programs.Insert(0, moving);
}
// #define Sample

using System.Text;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day10\Sample.txt";
    int cycleCountPart1 = 1;
    int cycleCountPart2 = 1;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day10\Full.txt";
    int cycleCountPart1 = 40;
    int cycleCountPart2 = 50;
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Solve(input, cycleCountPart1));
    Console.WriteLine("Part 2: " + Solve(input, cycleCountPart2));
    Console.ReadLine();
}

long Solve(string input, int cycles)
{
    for(int i=1; i <= cycles; i++)
    {
        input = LookAndSay(input);
    }

    return input.Length;
}

string LookAndSay(string previous)
{
    StringBuilder sb = new();
    
    char lastDigit = '\0';
    int count = 0;

    foreach(char c in previous)
    {
        if(c != lastDigit)
        {
            AddRest();
            
            lastDigit = c;
            count = 1;
        }
        else
        {
            count++;
        }
    }
    AddRest();

    void AddRest()
    {
        if (count > 0)
        {
            sb.Append(count.ToString());
            sb.Append(lastDigit);
        }
        if (count > 9) throw new ApplicationException("Unexpected Count");
    }

    return sb.ToString();
}
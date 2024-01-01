// #define Sample
using System.Text;

const int gPasswordLength = 8;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day11\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day11\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    string part1 = Solve(input);
    Console.WriteLine("Part 1: " + Solve(input));
    Console.WriteLine("Part 2: " + Solve(part1));
    Console.ReadLine();
}

string Solve(string password)
{
    do
    {
        password = Increase(password);
    }
    while (!PasswordAllowed(password));

    return password;
}

bool PasswordAllowed(string password)
{
    return !ContainsForbiddenChar(password) && ContainsSequence(password) && ContainsTwoPairs(password);
}

bool ContainsTwoPairs(string password)
{
    HashSet<char> repeatedCharacters = new();

    for (int i = 0; i < password.Length - 1; i++)
    {
        char a = password[i];
        char b = password[i + 1];

        if(a == b) repeatedCharacters.Add(a);
    }

    return repeatedCharacters.Count >= 2;
}

bool ContainsSequence(string password)
{
    for(int i=0; i < password.Length - 2; i++)
    {
        char a = password[i];
        char b = password[i + 1];
        char c = password[i + 2];

        if ((c - b) == 1 && (b - a) == 1) return true;
    }

    return false;
}

bool ContainsForbiddenChar(string password)
{
    foreach (char c in password)
    {
        if (c is 'i' or 'o' or 'l') return true;
    }

    return false;
}

string Increase(string source)
{
    StringBuilder sb = new StringBuilder(source);

    int index = gPasswordLength - 1;

    char nextDigit;
    do
    {
        nextDigit = NextDigit(sb[index]);
        sb[index] = nextDigit;
        index--;

    } while (nextDigit == 'a');

    return sb.ToString();
}

char NextDigit(char source)
{
    return source switch
    {
        'h' => 'j', // No i
        'n' => 'p', // No o
        'k' => 'm', // No l
        'z' => 'a',
        _ => (char)(source + 1) 
    };
}
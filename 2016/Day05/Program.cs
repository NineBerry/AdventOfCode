// #define Sample
using System.Text;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day05\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day05\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

string Part1(string input)
{
    return Solve(input, "00000", 8);
}

string Part2(string input)
{
    return SolveCinematic(input, "00000", 8);
}

string SolveCinematic(string input, string requestedPrefix, int passwordLength)
{
    int test = 0;
    var result = new StringBuilder(new string('_', passwordLength));
    Console.Write(result);

    while (true)
    {
        string md5 = CreateMD5(input + test);
        
        if (md5.StartsWith(requestedPrefix))
        {
            char ch = md5[6];
            int index = int.TryParse("" + md5[5], out var parsed) ? parsed : -1;

            if(index >= 0 && index < passwordLength && result[index] == '_')
            {
                result[index] = ch;
                Console.Write("\r" + result);
                if (!result.ToString().Contains('_')) break;
            }
        }
        test++;

    }
    
    Console.Write("\r");
    return result.ToString();
}

string Solve(string input, string requestedPrefix, int passwordLength)
{
    int test = 0;
    string result = "";

    while (true)
    {
        string md5 = CreateMD5(input + test);

        if (md5.StartsWith(requestedPrefix))
        {
            result += md5[5];
            Console.Write("\r" + result + new string('_', passwordLength - result.Length));
        }
        test++;

        if (result.Length == passwordLength)
        {
            Console.Write("\r");
            return result;
        }
    }
}

string CreateMD5(string input)
{
    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
    {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes); // .NET 5 +
    }
}
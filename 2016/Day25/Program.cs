using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

{
    const string explanation =
        """
        I looked at the bunnycode in the input and simplified the instructions to get the following pseudo-code:

        magicnumber = a + (number1 * number2)
        while(true) 
        {
            temp = magicnumber
            while (temp != 0)
            {
                output temp % 2
                temp = temp / 2
            }
        }

        So in order to produce a constant stream of 01010101... we need a magicnumber with the form 1010...10. 
        We are looking for the smallest number with that form greater than number1 * number2.

        So what we need to do: 
        Take the two numbers from the beginning of the bunnycode. 
        Then look for the smallest number in the correct format greater than the product of the two numbers.
        Then the difference is the correct quiz answer.
        """;


    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day25\Full.txt";

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine();
    Console.WriteLine(explanation);
    Console.ReadLine();
}

long Part1(string input)
{
    int[] values = 
        Regex.Matches(input, "\\d+")
        .Take(2)
        .Select(m => int.Parse(m.Value))
        .ToArray();

    int offset = values[0] * values[1];

    string binaryNumberString = "";
    int binaryNumber = 0;

    while (binaryNumber < offset)
    {
        binaryNumberString += "10";
        binaryNumber = Convert.ToInt32(binaryNumberString, 2);
    }

    return binaryNumber - offset;
}

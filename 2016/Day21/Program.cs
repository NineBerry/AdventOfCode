// #define Sample
using System.Text;
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day21\Sample.txt";
    string inputPart1 = "abcde";
    string inputPart2 = "decab";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day21\Full.txt";
    string inputPart1 = "abcdefgh";
    string inputPart2 = "fbgdceah";
#endif

    Command[] commands = File.ReadAllLines(fileName).Select(Command.Factory).ToArray();

    Console.WriteLine("Part 1: " + Part1(commands, inputPart1));
    Console.WriteLine("Part 2: " + Part2(commands, inputPart2));
    Console.ReadLine();
}


string Part1(Command[] commands, string text)
{
    foreach (var command in commands)
    {
        text = command.Apply(text);
    }
    return text;
}

string Part2(Command[] commands, string text)
{
    foreach (var command in commands.Reverse())
    {
        text = command.ReverseCommand().Apply(text);
    }
    return text;
}

public abstract class Command
{
    public abstract string Apply(string input);
    public abstract Command ReverseCommand();

    public static Command Factory(string line)
    {
        int[] values = Regex.Matches(line, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
        char[] chars = Regex.Matches(line, "(?<=letter )[a-z]").Select(m => m.Value[0]).ToArray();

        return line.Substring(0, 11) switch
        {
            "swap positi" => new SwapPositionsCommand { X = values[0], Y = values[1] },
            "swap letter" => new SwapLettersCommand { X = chars[0], Y = chars[1] },
            "rotate left" => new RotateLeftStepsCommand { Steps = values[0] },
            "rotate righ" => new RotateRightStepsCommand { Steps = values[0] },
            "rotate base" => new RotateBasedOnLetterCommand { Letter = chars[0] },
            "reverse pos" => new ReverseRangeCommand {X = values[0], Y= values[1] },
            "move positi" => new MovePositionCommand { X = values[0], Y = values[1] },
            _ => throw new ApplicationException("Unknown command")
        };
    }
}

public class SwapPositionsCommand: Command
{
    public int X;
    public int Y;

    public override string Apply(string input)
    {
        StringBuilder sb = new StringBuilder(input);

        char atX = sb[X];
        sb[X]  = sb[Y];
        sb[Y] = atX;

        return sb.ToString();
    }

    public override Command ReverseCommand()
    {
        return this;
    }
}

public class SwapLettersCommand: Command
{
    public char X;
    public char Y;

    public override string Apply(string input)
    {
        StringBuilder sb = new StringBuilder(input);

        sb.Replace(X, '*');
        sb.Replace(Y, X);
        sb.Replace('*', Y);

        return sb.ToString();
    }
    public override Command ReverseCommand()
    {
        return this;
    }
}

public class RotateLeftStepsCommand: Command
{
    public int Steps;

    public override string Apply(string input)
    {
        return RotateLeft(input, Steps);
    }
    public override Command ReverseCommand()
    {
        return new RotateRightStepsCommand { Steps = Steps };
    }

    public static string RotateLeft(string input, int width)
    {
        int rotate = width % input.Length;

        if (rotate != 0)
        {
            input = new string([.. input.Skip(rotate), .. input.Take(rotate)]);
        }

        return input;
    }

}

public class RotateRightStepsCommand: Command
{
    public int Steps;

    public override string Apply(string input)
    {
        return RotateRight(input, Steps);
    }

    public override Command ReverseCommand()
    {
        return new RotateLeftStepsCommand { Steps = Steps};
    }


    public static string RotateRight(string input, int width)
    {
        int rotate = width % input.Length;

        if (rotate != 0)
        {
            input = new string([.. input.Skip(input.Length - rotate), .. input.Take(input.Length - rotate)]);
        }

        return input;
    }
}

public class RotateBasedOnLetterCommand: Command
{
    public char Letter;

    public override string Apply(string input)
    {
        return RotateBasedOnLetter(input, Letter);
    }

    public override Command ReverseCommand()
    {
        return new ReverseRotateBasedOnLetterCommand { Letter = Letter};
    }

    public static string RotateBasedOnLetter(string input, char letter)
    {
        int width = input.IndexOf(letter);
        if (width >= 4) width++;
        width++;

        return RotateRightStepsCommand.RotateRight(input, width);
    }
}

public class ReverseRotateBasedOnLetterCommand : Command
{
    public char Letter;

    public override string Apply(string input)
    {
        for(int i=0; i < input.Length; i++)
        {
            string test = RotateLeftStepsCommand.RotateLeft(input, i);
            string testResult = RotateBasedOnLetterCommand.RotateBasedOnLetter(test, Letter);
            if (testResult == input)
            {
                return test;
            }
        }

        throw new ApplicationException("Cannot reverse");
    }

    public override Command ReverseCommand()
    {
        return new RotateBasedOnLetterCommand { Letter = Letter };
    }
}

    public class ReverseRangeCommand: Command
{
    public int X;
    public int Y;

    public override string Apply(string input)
    {
        StringBuilder sb = new StringBuilder(input);

        for (int lower = X, higher = Y; lower < higher; lower++, higher--)
        {
            char atLower = sb[lower];
            sb[lower] = sb[higher];
            sb[higher] = atLower;
        }

        return sb.ToString();
    }

    public override Command ReverseCommand()
    {
        return this;
    }
}

public class MovePositionCommand: Command
{
    public int X;
    public int Y;

    public override string Apply(string input)
    {
        StringBuilder sb = new StringBuilder(input);

        char ch = sb[X];
        sb.Remove(X, 1);
        sb.Insert(Y, ch);

        return sb.ToString();
    }

    public override Command ReverseCommand()
    {
        return new MovePositionCommand { X = Y, Y = X};
    }

}

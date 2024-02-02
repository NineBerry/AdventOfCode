// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day11\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day11\Full.txt";
#endif

    int gridSerial = int.Parse(File.ReadAllText(fileName));
    Grid grid = new Grid(gridSerial, 300, 300);

    Console.WriteLine("Part 1: " + Part1(grid));
    Console.WriteLine("Part 2: " + Part2(grid));
    Console.ReadLine();
}

string Part1(Grid grid)
{
    var result = grid.GetHighestTotalPowerSquare(3) ;

    return result.X + "," + result.Y;   
}

string Part2(Grid grid)
{
    var result = grid.GetHighestTotalPowerSquare();


    return result.X + "," + result.Y + "," + result.SquareSize;
}

public class Grid
{
    public Grid(int serial, int width, int height)
    {
        Serial = serial;
        Width = width;
        Height = height;

        PowerLevels = new int[Width + 1, Height + 1];

        for (int x = 1; x <= Width; x++)
        {
            for (int y = 1; y <= Height; y++)
            {
                PowerLevels[x,y] = CalcPowerLevel(x, y, Serial);
            }
        }
    }

    public int Serial;
    public int Width;
    public int Height;

    int[,] PowerLevels;

    private static int CalcPowerLevel(int x, int y, int serial)
    {
        int rackId = x + 10;
        int powerLevel = rackId * y;
        powerLevel += serial;
        powerLevel *= rackId;
        powerLevel %= 1000;
        powerLevel /= 100;
        powerLevel -= 5;

        return powerLevel;
    }

    internal (int X, int Y) GetHighestTotalPowerSquare(int squareSize)
    {
        int highestPointX = 0;
        int highestPointY = 0;
        int highestTotalPower = int.MinValue;

        for (int x = 1; x < Width - squareSize; x++)
        {
            for (int y = 1; y < Height - squareSize; y++)
            {
                int sum = 0;

                for(int xx = x; xx < x + squareSize; xx++)
                {
                    for (int yy = y; yy < y + squareSize; yy++)
                    {
                        sum += PowerLevels[xx, yy];
                    }
                }

                if(sum > highestTotalPower)
                {
                    highestTotalPower = sum;
                    highestPointX = x;
                    highestPointY = y;
                }
            }
        }

        return (highestPointX, highestPointY);
    }

    public (int X, int Y, int SquareSize)  GetHighestTotalPowerSquare()
    {
        int highestPointX = 0;
        int highestPointY = 0!;
        int highestSquareSize = 0;
        int highestTotalPower = int.MinValue;

        for (int x = 1; x <= Width; x++)
        {
            if (x % 10 == 1)
            {
                Console.Write("\r" + (x * 100) / 300 + "%");
            }

            for (int y = 1; y <= Height; y++)
            {

                int sum = 0;
                int squareSize = 1;

                while(x + squareSize - 1 <= Width && y + squareSize - 1 <= Height)
                {
                    for (int i = x; i < x + squareSize; i++)
                    {
                        sum += PowerLevels[i, y + squareSize - 1];
                    }

                    for (int i = y; i < y + squareSize -1; i++)
                    {
                        sum += PowerLevels[x + squareSize - 1, i];
                    }

                    if (sum > highestTotalPower)
                    {
                        highestTotalPower = sum;
                        highestPointX = x;
                        highestPointY = y;
                        highestSquareSize = squareSize;
                    }

                    squareSize++;
                }

            }
        }

        Console.Write("\r");

        return (highestPointX, highestPointY, highestSquareSize);
    }
}


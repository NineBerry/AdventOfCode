// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day02\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day02\Full.txt";
#endif

    int[] commands = File.ReadAllText(fileName).Select(ch => ch == '>' ? 1 : -1).ToArray();
    RobotAndWall robotAndWall = new RobotAndWall();
    robotAndWall.ApplyRobot(commands);


    Console.WriteLine("Part 1: " + Part1(robotAndWall));
    Console.WriteLine("Part 2: " + Part2(robotAndWall));
    Console.WriteLine("Part 3: " + Part3(robotAndWall));

    Console.ReadLine();
}

long Part1(RobotAndWall robotAndWall)
{
    var hottestSegments = robotAndWall.GetHottestStaticSegments();    
    return hottestSegments.Min(s => s.segment) * hottestSegments.First().temperature;
}

long Part2(RobotAndWall robotAndWall)
{
    return robotAndWall.GetMovingWallTemperature(1);
}

long Part3(RobotAndWall robotAndWall)
{
    var hottestSegments = robotAndWall.GetHottestMovingSegments();
    return hottestSegments.Min(s => s.segment) * hottestSegments.First().temperature;
}

class RobotAndWall
{
    private int[] staticWall = new int[101];
    private int[] movingWall = new int[101];

    public void ApplyRobot(int[] commands)
    {
        int robotPosition = 1;
        int movingWallOffset = 0;

        int[] robotCommands = commands;
        int[] movingWallCommands = commands.Reverse().ToArray();  

        foreach (int i in Enumerable.Range(0, commands.Length))
        {
            robotPosition += robotCommands[i];
            movingWallOffset += movingWallCommands[i] * -1;

            int staticWallPosition = TranslatePosition(robotPosition);
            int movingWallPosition = TranslatePosition(robotPosition + movingWallOffset);

            staticWall[staticWallPosition]++;
            movingWall[movingWallPosition]++;
        }
    }

    public IEnumerable<(int segment, int temperature)> GetHottestStaticSegments()
    {
        int hottestTemperature = staticWall.Max();

        return staticWall
            .Select((temperature, segment) => (segment, temperature))
                   .Where(s => s.temperature == hottestTemperature);
    }

    public IEnumerable<(int segment, int temperature)> GetHottestMovingSegments()
    {
        int hottestTemperature = movingWall.Max();

        return movingWall
            .Select((temperature, segment) => (segment, temperature))
                   .Where(s => s.temperature == hottestTemperature);
    }

    public long GetMovingWallTemperature(int segment)
    {
        return movingWall[segment];
    }

    private int TranslatePosition(int position)
    {
        while (position <= 0) position += 100;
        while (position >= 101) position -= 100;

        return position;
    }
}
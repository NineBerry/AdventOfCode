// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day17\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day17\Full.txt";
#endif

    int spinLockSteps = int.Parse(File.ReadAllText(fileName));

    Console.WriteLine("Part 1: " + Part1(spinLockSteps));
    Console.WriteLine("Part 2 optimized: " + Part2Optimized(spinLockSteps));
    Console.WriteLine("Part 2 not optimized: " + Part2NotOptimized(spinLockSteps));
    Console.ReadLine();
}

long Part1(int spinLockSteps)
{
    List<int> strip = [0];
    int currentPosition = 0;
    
    for(int i=1; i <= 2017; i++)
    {
        currentPosition = (currentPosition + spinLockSteps) % i;
        strip.Insert(currentPosition + 1, i);
        currentPosition++;
    }

    return strip[currentPosition + 1];
}

long Part2Optimized(int spinLockSteps)
{
    int currentPosition = 0;
    int lastInsertedAfter0 = 0;

    for (int i = 1; i <= 50_000_000; i++)
    {
        currentPosition = (currentPosition + spinLockSteps) % i;
        
        if (currentPosition == 0) lastInsertedAfter0 = i;
        
        currentPosition++;
    }

    return lastInsertedAfter0;
}

long Part2NotOptimized(int spinLockSteps)
{
    

    LinkedList<int> strip = [];
    var node = strip.AddFirst(0);

    for (int i = 1; i <= 50_000_000; i++)
    {
        if(i % 50_000_0 == 0)
        {
            Console.Write("\r" + i / 50_000_0 + "%");
        }

        int toMove = spinLockSteps % i;

        for (int move = 1; move <= toMove; move++)
        {
            if (node.Next == null)
            {
                node = strip.First;
            }
            else
            {
                node = node.Next;
            }
        }

        node = strip.AddAfter(node, i);
    }
    Console.Write("\r");

    node = strip.First;
    while(node.Value != 0) node = node.Next;

    return node.Next.Value;
    
}


// #define Sample

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day11\Sample1.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day11\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day11\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\FlipFlop\2026\Day11\Full.txt";
#endif

    Console.WriteLine("Part 1: " + Part1(File.ReadAllLines(fileNamePart1)));
    Console.WriteLine("Part 2: " + Part2(File.ReadAllLines(fileNamePart2)));
    Console.WriteLine("Part 3: " + Part3(File.ReadAllLines(fileNamePart2)));

    Console.ReadLine();
}

long Part1(string[] lines)
{
    Tree[] trees = Tree.ParseMultipleTrees(lines);

    foreach (var tree in trees)
    {
        tree.TreeGroup = [tree];
    }

    long sum = 0;

    foreach(Tree tree in trees)
    {
        while (!tree.IsDead())
        {
            tree.GrowOneYear();
        }

        sum += tree.GetBiologicalMass();
    }

    return sum;
}

long Part2(string[] lines)
{
    Tree[] trees = Tree.ParseMultipleTrees(lines);

    foreach (var tree in trees)
    {
        tree.TreeGroup = trees.ToList();
    }

    GrowTreeGroupUntilAllDead(trees);

    return trees.Sum(t => t.GetBiologicalMass());
}

long Part3(string[] lines)
{
    Tree[] trees = Tree.ParseMultipleTrees(lines);

    foreach (var tree in trees)
    {
        tree.TreeGroup = trees.ToList();
    }

    GrowTreeGroupUntilAllDead(trees);
    trees = MakeOffspring(trees);
    GrowTreeGroupUntilAllDead(trees);
    trees = MakeOffspring(trees);
    GrowTreeGroupUntilAllDead(trees);

    return trees.Sum(t => t.GetBiologicalMass());
}

Tree[] MakeOffspring(Tree[] trees)
{
    List<Tree> result = [];

    int nextNewTreeId = 1;

    var allSprouts = trees.SelectMany(t => t.Sprouts).ToDictionary();
    var distinctColumns = allSprouts.Select(s => s.Key.X).Distinct().Order().ToArray();

    foreach (var column in distinctColumns)
    {
        var winningSprout = allSprouts.Where(s => s.Key.X == column).MinBy(s => s.Key.Y);

        var originalTree = trees.Single(t => t.TreeId == winningSprout.Value.TreeID);

        Tree newTree = originalTree.MakeOffspring(winningSprout.Key with {Y = 0}, nextNewTreeId++);
        result.Add(newTree);
    }

    foreach(var tree in result)
    {
        tree.TreeGroup = result;
    }

    return result.ToArray();
}

static void GrowTreeGroupUntilAllDead(Tree[] trees)
{
    while (true)
    {
        var livingTrees = trees.Where(t => !t.IsDead()).OrderBy(t => t.TreeId);

        if (!livingTrees.Any()) break;

        foreach (Tree tree in livingTrees)
        {
            tree.GrowOneYear();
        }
    }
}


// This is still a bit slow.
// We could maybe improve performance 
// by keeping a common list of all stems and sprouts for 
// the whole tree group always updated


class Tree
{
    public List<Tree> TreeGroup = [];

    private Dictionary<int, DnaSegment> DnaSegments { get; init; } = [];
    private Dictionary<Point, DnaSegment> Stems { get; init; } = [];
    public Dictionary<Point, DnaSegment> Sprouts { get; init; } = [];

    public int Age { get; private set; } = 0;
    public int TreeId { get; private set; } = 0;

    public void GrowOneYear()
    {
        Age++;

        Point[] growingSprouts = Sprouts.Keys.ToArray();

        foreach(var p in growingSprouts)
        {
            Stems.Add(p, Sprouts[p]);
            Sprouts.Remove(p);
        }

        List<(Point Point, DnaSegment Sprout)> potentialNewSprouts = [];

        foreach(var p in growingSprouts)
        {
            DnaSegment growingSprout = Stems[p];

            AddPotentialSprout(growingSprout.TopSproutID, Point.Direction.North);
            AddPotentialSprout(growingSprout.LeftSproutID, Point.Direction.West);
            AddPotentialSprout(growingSprout.RightSproutID, Point.Direction.East);

            void AddPotentialSprout(int? newId, Point.Direction direction)
            {
                if (newId.HasValue)
                {
                    potentialNewSprouts.Add((p.GetNeightboringPoint(direction), DnaSegments[newId.Value]));
                }
            }
        }

        var groups = potentialNewSprouts.GroupBy(e => e.Point);

        var treeGroupStems = GetAllGroupStemPoints();
        var treeGroupSprouts = GetAllGroupSprouts();

        foreach (var group in groups)
        {
            var winningSprout = group.MaxBy(e => e.Sprout.DnaSegmentID);

            if (treeGroupStems.Contains(winningSprout.Point)) continue;
            if (treeGroupSprouts.Contains(winningSprout.Point)) continue;

            if (!Stems.Keys.Contains(winningSprout.Point))
            {
                Sprouts.Add(winningSprout.Point, winningSprout.Sprout);
            }
        }
    }

    private HashSet<Point> GetAllGroupStemPoints() 
    {
        return TreeGroup.SelectMany(t => t.Stems).Select(t => t.Key).ToHashSet();
    }

    private Dictionary<Point, DnaSegment> GetAllGroupStems()
    {
        return TreeGroup.SelectMany(t => t.Stems).ToDictionary();
    }

    private HashSet<Point> GetAllGroupSprouts() 
    {
        return TreeGroup.SelectMany(t => t.Sprouts).Select(t => t.Key).ToHashSet();
    }

    public bool IsDead()
    {
        if(Age >= 100) return true;
        
        if(Age >= 5)
        {
            if(!HasEnoughEnergy()) return true;
        }

        return false;
    }

    public int GetBiologicalMass()
    {
        return Sprouts.Count + Stems.Count;
    }

    private bool HasEnoughEnergy()
    {
        var energyRequired = GetEnergyRequired();
        var energyProduced = GetEnergyProduced();
        return energyRequired <= energyProduced;
    }

    private int GetEnergyRequired()
    {
        return GetBiologicalMass() * 3;
    }

    private Dictionary<Point, DnaSegment> cachedTreeGroupStems = [];

    private int GetEnergyProduced()
    {
        cachedTreeGroupStems = GetAllGroupStems();
        var distinctColumns = Stems.Keys.Select(p => p.X).Distinct();
        return distinctColumns.Sum(c => GetEnergyProducedInColumn(c)); 
    }

    private int GetEnergyProducedInColumn(int column)
    {
        var topStems = cachedTreeGroupStems.Where(s => s.Key.X == column).OrderBy(s => s.Key.Y).Take(3);

        int energy = 0;
        int multiplier = 3;

        foreach (var stem in topStems)
        {
            if (stem.Value.TreeID == this.TreeId)
            {
                int actualHeight = -stem.Key.Y + 1;
                int heightForFormula = Math.Min(actualHeight, 10);
                energy += (multiplier * heightForFormula);
            }
            multiplier--;
        }

        return energy;
    }

    private static Tree ParseSingleTree(string lineAbove, string lineMain, int treeID, int rootXPosition)
    {
        lineAbove += "    ";
        
        Dictionary<int, DnaSegment> dnaSegments = [];

        int charIndex = 0;
        List<Tree> trees = [];

        while (charIndex < lineMain.Length)
        {
            string segmentAbove = lineAbove.Substring(charIndex, 10);
            string segmentMain = lineMain.Substring(charIndex, 10);
            charIndex+= 12;

            DnaSegment segment = DnaSegment.Parse(segmentAbove, segmentMain, treeID);
            dnaSegments.Add(segment.DnaSegmentID, segment);
        }
        
        Tree tree = new Tree { DnaSegments = dnaSegments, TreeId = treeID };
        tree.Sprouts.Add(new Point(rootXPosition, 0), dnaSegments[0]);

        return tree;
    }

    public static Tree[] ParseMultipleTrees(string[] lines)
    {
        int treeID = 1;
        int rootXPosition = 0;
        int lineIndex = 0;
        List<Tree> trees = [];

        while(lineIndex < lines.Length)
        {
            string lineAbove = lines[lineIndex++];
            string lineMain = lines[lineIndex++];
            lineIndex++;

            Tree newTree = Tree.ParseSingleTree(lineAbove, lineMain, treeID, rootXPosition);

            trees.Add(newTree);
            treeID++;
            rootXPosition += 10;
        }

        return trees.ToArray();
    }

    internal Tree MakeOffspring(Point seedPoint, int newTreeId)
    {
        Tree result = new Tree
        {
            TreeId = newTreeId,
        };

        foreach(var segment in DnaSegments.Values)
        {
            result.DnaSegments.Add(segment.DnaSegmentID, segment with { TreeID = newTreeId });
        }

        result.Sprouts.Add(seedPoint, DnaSegments[0]);

        return result;
    }
}

record class DnaSegment
{
    public int DnaSegmentID { get; private set; }
    public int TreeID { get; init; }

    public int? LeftSproutID { get; private set; } = null;
    public int? RightSproutID { get; private set; } = null;
    public int? TopSproutID { get; private set; } = null;

    public static DnaSegment Parse(string segmentAbove, string segmentMain, int treeID)
    {
        DnaSegment segment = new DnaSegment
        {
            DnaSegmentID = ParseID(segmentMain.Substring(4, 2))!.Value,
            TreeID = treeID,
            TopSproutID = ParseID(segmentAbove.Substring(4, 2)),
            LeftSproutID = ParseID(segmentMain.Substring(0, 2)),
            RightSproutID = ParseID(segmentMain.Substring(8, 2))
        };

        return segment;


        int? ParseID(string s)
        {
            if(s == "XX") return null;
            return int.Parse(s);
        }
    }
}


record class Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y + 1 },
            Direction.West => this with { X = this.X - 1 },
            Direction.North => this with { Y = this.Y - 1 },
            Direction.East => this with { X = this.X + 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public enum Direction
    {
        None = '\0',
        South = 'v',
        West = '<',
        North = '^',
        East = '>'
    }
}
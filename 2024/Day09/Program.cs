// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day09\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day09\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string input)
{
    FileSystem fileSystem = new(input);
    fileSystem.MoveBlocks();
    return fileSystem.CalculateChecksum();
}

long Part2(string input)
{
    FileSystem fileSystem = new(input);
    fileSystem.MoveFiles();
    return fileSystem.CalculateChecksum();
}

class FileSystem
{
    private LinkedList<BlockRange> blocks = [];

    public FileSystem(string input)
    {
        int fileId = 0;
        int position = 0;
        
        foreach((int index, char ch) in input.Index())
        {
            int length = int.Parse("" + ch);
            int? nextFileId = int.IsOddInteger(index) ? null : fileId++;

            blocks.AddLast(new BlockRange(length, nextFileId, position));

            position += length;
        }
    }

    private void MoveLeft(LinkedListNode<BlockRange> left, LinkedListNode<BlockRange> right, int toCopyLength)
    {
        // Add moved blocks at left side and adapt gap
        blocks.AddBefore(left, new BlockRange(toCopyLength, right.Value.FileId, left.Value.Position));
        left.Value.Length = left.Value.Length - toCopyLength;
        left.Value.Position += toCopyLength;

        // Add gap at right side and adapt file length
        blocks.AddBefore(right, new BlockRange(toCopyLength, null, right.Value.Position + (right.Value.Length - toCopyLength)));
        right.Value.Length = right.Value.Length - toCopyLength;
    }

    public void MoveBlocks()
    {
        var left = blocks.First!;
        var right = blocks.Last!;

        // While left is left of right
        while (left.Value.Position < right.Value.Position)
        {
            // On left side skip non-gaps
            if (!IsValidGap(left.Value))
            {
                left = left.Next!;
                continue;
            }

            // On right side skip non-files
            if (!IsValidFile(right.Value))
            {
                right= right.Previous!;
                continue;
            }

            // We have a gap left and a file right
            int toCopyLength = Math.Min(left.Value.Length, right.Value.Length);
            MoveLeft(left, right, toCopyLength);
        }
    }

    public void MoveFiles()
    {
        // Get list of files in order descending by file id
        var filesToMove = GetFilesByIdDescending();

        // Move all the way from left to right handling all gaps
        var left = blocks.First;
        while (left != null)
        {
            // Optimization: We can break when we have reached the
            // most right remaining file.
            // The gaps after that will not be filled
            if (left == filesToMove.FirstOrDefault()) break;

            // Skip over non-gaps
            if (!IsValidGap(left.Value))
            {
                left = left.Next;
                continue;
            }

            // Find the first file with the highest id that
            // fits inside the current gap and is right of that gap
            var toMove = filesToMove.FirstOrDefault(b =>
                (b.Value.Length <= left.Value.Length)
                && (b.Value.Position > left.Value.Position));

            if (toMove != null)
            {
                filesToMove.Remove(toMove);
                MoveLeft(left, toMove, toMove.Value.Length);
            }
            else
            {
                // No file fits the gap.
                left = left.Next;
            }
        }
    }

    private List<LinkedListNode<BlockRange>> GetFilesByIdDescending()
    {
        List<LinkedListNode<BlockRange>> files = [];

        for(var current = blocks.Last; current != null; current = current.Previous)
        { 
            if (IsValidFile(current.Value))
            {
                files.Add(current);
            }
        }

        return files;
    }

    private static bool IsValidGap(BlockRange blockRange) 
        => blockRange.Length > 0 && !blockRange.FileId.HasValue;
    private static bool IsValidFile(BlockRange blockRange) 
        => blockRange.Length > 0 && blockRange.FileId.HasValue;

    public long CalculateChecksum() => blocks.Sum(b => b.GetChecksum());
}

class BlockRange
{
    public int? FileId = null;
    public int Length;
    public int Position;

    public BlockRange(int length, int? fileID, int position)
    {
        Length = length;
        FileId = fileID;
        Position = position;
    }

    public long GetChecksum()
    {
        if(!FileId.HasValue) return 0;

        return Enumerable
            .Range(0, Length)
            .Sum(i => (Position + (long)i) * FileId.Value);
    }
}

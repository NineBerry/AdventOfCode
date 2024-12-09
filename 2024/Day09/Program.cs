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
    fileSystem.DefragmentBlocks();
    return fileSystem.CalculateChecksum();
}

long Part2(string input)
{
    FileSystem fileSystem = new(input);
    fileSystem.DefragmentFiles();
    return fileSystem.CalculateChecksum();
}


class FileSystem
{
    private LinkedList<BlockRange> blocks = new();

    public FileSystem(string input)
    {
        bool IsFreeBlock = false;
        long fileId = 0;
        for (int position = 0; position < input.Length; position++)
        {
            char ch = input[position];
            long length = long.Parse(""+ch); ;

            if (IsFreeBlock)
            {
                blocks.AddLast(new BlockRange(length, null, position));
            }
            else
            {
                blocks.AddLast(new BlockRange(length, fileId, position));
                fileId++;
            }

            IsFreeBlock = !IsFreeBlock;
        }
    }

    public void DefragmentBlocks()
    {
        var left = blocks.First!;
        var right = blocks.Last!;

        // Until left and right meet
        while (left != right)
        {
            // On left side skip files
            if (!left.Value.IsEmpty)
            {
                left = left.Next!;
                continue;
            }

            // On right side skip gaps
            if (right.Value.IsEmpty)
            {
                right= right.Previous!;
                continue;
            }

            // We have a gap left and a file right
            // Decide how many blocks to move from right to left
            long toCopyLength = Math.Min(left.Value.Length, right.Value.Length);

            // Add moved blocks at left side and adapt gap
            var newLeft = blocks.AddBefore(left, new BlockRange(toCopyLength, right.Value.FileId, 0));
            long emptyRest = left.Value.Length - toCopyLength;

            if(emptyRest == 0)
            {
                blocks.Remove(left);
                left = newLeft;
            }
            else
            {
                left.Value.Length = emptyRest;
            }

            // Add gap at right side and afapt file length
            var newRight = blocks.AddBefore(right, new BlockRange(toCopyLength, null, 0));
            long fileRest = right.Value.Length - toCopyLength;

            if (fileRest == 0)
            {
                blocks.Remove(right);
                right = newRight;
            }
            else
            {
                right.Value.Length = fileRest;
            }
        }
    }

    public void DefragmentFiles()
    {
        // Get list of files in order descending by file id
        var filesToMove = GetFilesByIdDescending();

        var left = blocks.First;

        // Move all the way from left to right handling all gaps
        while(left != null)
        {
            // Optimization: We can break when we have reached the last file
            // The gaps after that will not be filled
            if (left == filesToMove.FirstOrDefault()) break;

            // Skip over files
            if (!left.Value.IsEmpty)
            {
                left = left.Next;
                continue;
            }

            // Find the first file with the  highest id that
            // fits inside the current gap and is right of that gap
            var toMove = filesToMove.FirstOrDefault(b => 
                (b.Value.Length <= left.Value.Length) 
                && b.Value.Position > left.Value.Position);

            if (toMove != null)
            {
                // Left side: Add file in gap and adapt gap
                var addedFile = blocks.AddBefore(left, toMove.Value);
                long emptyRest = left.Value.Length - toMove.Value.Length;

                if (emptyRest == 0)
                {
                    blocks.Remove(left);
                    left = addedFile;
                }
                else
                {
                    left.Value.Length = emptyRest;
                }
                
                // Right side: Replace file with gap
                blocks.AddAfter(toMove, new BlockRange(toMove.Value.Length, null, toMove.Value.Position));
                blocks.Remove(toMove);

                filesToMove.Remove(toMove);
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
        List<LinkedListNode<BlockRange>> filesToMove = [];

        var current = blocks.Last;
        while (current != null)
        {
            if (!current.Value.IsEmpty)
            {
                filesToMove.Add(current);
            }

            current = current.Previous;
        }

        return filesToMove;
    }

    public long CalculateChecksum()
    {
        long sum = 0;
        long position = 0;

        foreach (var block in blocks)
        {
            sum += block.GetChecksum(position);
            position += block.Length;
        }        

        return sum;
    }
}

class BlockRange
{
    public long? FileId = null;
    public long Length;
    public long Position;

    public BlockRange(long length, long? fileID, long originalPosition)
    {
        Length = length;
        FileId = fileID;
        Position = originalPosition;
    }

    public bool IsEmpty { get => !FileId.HasValue; }

    public long GetChecksum(long startPosition)
    {
        if(IsEmpty) return 0;

        long sum = 0;
        for (long i = 0; i < Length; i++)
        {
            sum += (startPosition + i) * FileId!.Value;
        }

        return sum;
    }
}

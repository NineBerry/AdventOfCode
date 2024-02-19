#define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day07\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day07\Full.txt";
#endif

    string[] input = System.IO.File.ReadAllLines(fileName);

    FileSystem fs = new FileSystem(input);

    Console.WriteLine("Part 1: " + Part1(fs));
    Console.WriteLine("Part 2: " + Part2(fs));
    Console.ReadLine();
}

long Part1(FileSystem fs)
{
    long sum = 0;
    fs.TraverseDirectories(OnDirectory);
    return sum; 

    void OnDirectory(Directory directory)
    {
        int size = directory.Size;
        if(size <= 100000) 
        {
            sum += size;
        }
    }
}

long Part2(FileSystem fs)
{
    long availableSize = 70000000 - fs.Root.Size;
    long requiredSize = 30000000 - availableSize;
    long foundDirectorySize = long.MaxValue;

    fs.TraverseDirectories(OnDirectory);
    return foundDirectorySize;

    void OnDirectory(Directory directory)
    {
        int size = directory.Size;
        if (size >= requiredSize )
        {
            foundDirectorySize = Math.Min(foundDirectorySize, size);
        }
    }
}

public class FileSystem
{
    public FileSystem(string[] commands) 
    {
        Directory current = Root;

        foreach (string command in commands.Skip(1))
        {
            PerformCommand(command, ref current);
        }
    }

    public void TraverseDirectories(Action<Directory> callback)
    {
        TraverseDirectoryRecursive(Root, callback);
    }

    private void TraverseDirectoryRecursive(Directory directory, Action<Directory> callback)
    {
        callback(directory);

        foreach(var subdir in directory.Children.OfType<Directory>())
        {
            TraverseDirectoryRecursive(subdir, callback);
        }
    }

    private void PerformCommand(string command, ref Directory current)
    {
        switch(command.Substring(0, 3))
        {
            case "$ c":
                PerformCDCommand(command, ref current);
                break;
            case "$ l":
                // LS command, do nothing
                break;
            case "dir":
                PerformDirEntry(command, current);
                // LS command, do nothing
                break;
            default:
                PerformFileEntry(command, current);
                break;
        }
    }

    private void PerformFileEntry(string command, Directory current)
    {
        string[] parts = command.Split(" ");
        int size = int.Parse(parts[0]);
        string fileName = parts[1];
        current.Children.Add(new File(fileName, size, current));
    }

    private void PerformDirEntry(string command, Directory current)
    {
        string[] parts = command.Split(" ");
        string dirName = parts[1];
        current.Children.Add(new Directory(dirName, current));
    }

    private void PerformCDCommand(string command, ref Directory current)
    {
        string[] parts = command.Split(" ");
        string dirName = parts[2];
        if(dirName == "..")
        {
            current = current.Parent;
        }
        else
        {
            current = current.Children.OfType<Directory>().Where(d => d.Name == dirName).Single();
        }
    }

    public Directory Root = new Directory("/", null!);
}


public interface IFileSystemObject
{
    public string Name { get; set; }
    public int Size{ get; }
    public Directory Parent { get; }
}

public class Directory : IFileSystemObject
{
    public Directory(string name, Directory parent)
    {
        Name = name;
        Parent = parent;
    }

    public string Name { get; set; }
    public int Size { get => GetSizeRecursive(); }
    public Directory Parent { get; }

    public List<IFileSystemObject> Children = [];

    private int GetSizeRecursive()
    {
        return Children.Sum(c => c.Size);
    }
}

public class File : IFileSystemObject
{
    public File(string name, int size, Directory parent)
    {
        Name = name;
        Size = size;
        Parent = parent;
    }

    public string Name { get; set; }
    public int Size { get; }
    public Directory Parent { get; }
}
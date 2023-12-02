using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class NoSpaceLeftOnDevice() : AdventOfCodeChallenge("No Space Left On Device", 2022, 07)
{
    private const int FILESYSTEM_MAX_SIZE = 70_000_000;
    private const int UPDATE_REQUIRED_SIZE = 30_000_000;
    
    private static readonly Regex ChangeDirectoryRegex = new(@"\$ cd (.*)", RegexOptions.Compiled);
    private static readonly Regex FileListingRegex = new(@"([\d]+) ([\w.]+)", RegexOptions.Compiled);
    
    private IReadOnlyDictionary<string, HashSet<File>> _allFiles = null!;

    public override void LoadData()
    {
        var currentDirectory = string.Empty;

        var files = new Dictionary<string, HashSet<File>>();
        foreach (var line in _input.Split('\n'))
        {
            var match = ChangeDirectoryRegex.Match(line);
            if (match.Success)
            {
                var newDirectory = match.Groups[1].Value;
                currentDirectory = newDirectory switch
                {
                    ".." => string.Join('/', currentDirectory.Split('/')[..^1]),
                    "/" => currentDirectory,
                    _ => $"{currentDirectory}/{newDirectory}"
                };
            }

            match = FileListingRegex.Match(line);
            if (match.Success)
            {
                var size = int.Parse(match.Groups[1].Value);
                var filename = match.Groups[2].Value;

                if (files.TryGetValue(currentDirectory, out var directory))
                    directory.Add(new File(currentDirectory, size, filename));
                else
                    files[currentDirectory] = new HashSet<File> { new(currentDirectory, size, filename) };


                var directoryCopy = currentDirectory;
                do
                {
                    directoryCopy = string.Join('/', directoryCopy.Split('/')[..^1]);
                    
                    if (files.TryGetValue(directoryCopy, out directory))
                        directory.Add(new File(currentDirectory, size, filename));
                    else
                        files[directoryCopy] = new HashSet<File> { new(currentDirectory, size, filename) };
                    
                } while (!string.IsNullOrWhiteSpace(directoryCopy));
            }
        }

        _allFiles = files;
    }

    public override string SolvePart1()
    {
        var totalSize = 0;
        foreach (var directory in _allFiles)
        {
            var directorySize = directory.Value.Sum(x => x.Size);
            if (directorySize <= 100_000)
                totalSize += directorySize;
        }

        return $"The total of all directories at or under 100,000 is {totalSize} bytes.";
    }

    public override string SolvePart2()
    {
        var currentFilesystemSize = _allFiles[""].Sum(x => x.Size);
        var freeSpace = FILESYSTEM_MAX_SIZE - currentFilesystemSize;
        var neededSpace = UPDATE_REQUIRED_SIZE - freeSpace;

        var smallestDirectorySize = _allFiles.Values.Select(x => x.Sum(y => y.Size)).Order().First(x => x >= neededSpace);
        return $"The smallest directory containing enough space to clear up for the update is {smallestDirectorySize} bytes.";
    }

    private sealed record File(string Directory, int Size, string Filename)
    {
        public bool Equals(File? other)
        {
            return Directory == other?.Directory && Filename == other.Filename;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Directory, Filename);
        }
    }
}
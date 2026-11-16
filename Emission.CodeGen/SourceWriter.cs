using Emission.Builders;

namespace Emission.CodeGen;

public static class SourceWriter
{
    private const string _solutionFile = "Emission.slnx";
    private static readonly Lazy<DirectoryInfo> _repoRoot = new(() => GetRepositoryRoot(new(AppContext.BaseDirectory)));
    public static DirectoryInfo RepoRoot => _repoRoot.Value;

    public static void WriteSource(string relativePath, BuiltSource src)
    {
        string outputDirectory = Path.Combine(RepoRoot.FullName, relativePath);
        Directory.CreateDirectory(outputDirectory);
        string path = Path.Combine(outputDirectory, src.HintName);
        File.WriteAllText(path, src.Source);
    }

    private static DirectoryInfo GetRepositoryRoot(DirectoryInfo directory)
    {
        if (File.Exists(Path.Combine(directory.FullName, _solutionFile)))
            return directory;

        if (directory.Parent is not null)
            return GetRepositoryRoot(directory.Parent);
        
        throw new InvalidOperationException(_solutionFile + " was not found in the parent path tree");
    }
}

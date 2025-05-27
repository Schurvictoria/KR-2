namespace Core.Utils;

public static class FileStorageHelper
{
    public static void EnsureDirectory(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    public static string GetStoragePath(string storageRoot, string filename)
    {
        if (string.IsNullOrWhiteSpace(storageRoot))
            throw new ArgumentException("Storage path cannot be empty", nameof(storageRoot));
        return Path.Combine(storageRoot, filename);
    }
}

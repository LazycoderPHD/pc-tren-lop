namespace SecurityInspector;
public static class SafeWalker
{
    public static IEnumerable<string> Files(string root, Action<string> issue, CancellationToken token)
    {
        root = PathRules.Canonical(root);
        var pending = new Stack<string>(); pending.Push(root);
        while (pending.Count > 0)
        {
            token.ThrowIfCancellationRequested(); string dir = pending.Pop(); string[] entries;
            try
            {
                if ((File.GetAttributes(dir) & FileAttributes.ReparsePoint) != 0)
                { issue("Bỏ qua liên kết/reparse point: " + dir); continue; }
                entries = Directory.GetFileSystemEntries(dir);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            { issue(dir + ": " + ex.Message); continue; }
            foreach (string path in entries)
            {
                token.ThrowIfCancellationRequested(); FileAttributes attrs;
                try { attrs = File.GetAttributes(path); }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                { issue(path + ": " + ex.Message); continue; }
                if ((attrs & FileAttributes.ReparsePoint) != 0)
                { issue("Bỏ qua liên kết/reparse point: " + path); continue; }
                if ((attrs & FileAttributes.Directory) != 0) pending.Push(path);
                else yield return path;
            }
        }
    }
}
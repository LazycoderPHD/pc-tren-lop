using System.Security.Cryptography;
namespace SecurityInspector;
public static class HashService
{
    public static FileSignature Signature(string path)
    {
        var info = new FileInfo(path); info.Refresh();
        if (!info.Exists) throw new FileNotFoundException("Tệp không còn tồn tại.", path);
        return new(info.Length, info.LastWriteTimeUtc);
    }
    public static async Task<(FileSignature Signature, string Hash)> ReadAsync(string path, CancellationToken token)
    {
        var before = Signature(path);
        await using var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            65536, FileOptions.Asynchronous | FileOptions.SequentialScan);
        string hash = Convert.ToHexString(await SHA256.HashDataAsync(input, token));
        var after = Signature(path);
        if (before != after) throw new IOException("Tệp thay đổi khi tính hash: " + path);
        return (after, hash);
    }
}
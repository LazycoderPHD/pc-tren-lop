namespace SecurityInspector;
public record Finding(string TimeUtc, string Area, string Kind, string Path, string Detail, string Source)
{
    public static Finding New(string area, string kind, string path, string detail, string source = "Windows trực tiếp")
        => new(DateTimeOffset.UtcNow.ToString("O"), area, kind, path, detail, source);
}
public class DriveChoice { public string Root { get; set; } = ""; public bool Selected { get; set; } = false; }
public record FileStamp(string RelativePath, long Length, DateTime LastWriteUtc, string Sha256);
public record Baseline(int Version, string Root, DateTimeOffset CreatedUtc, List<FileStamp> Files, List<string> Issues);
public record FileSignature(long Length, DateTime LastWriteUtc);
public record MonitorMessage(string Root, string Kind, string Path, string OldPath, DateTimeOffset SeenUtc);
public record ExeMetadata(string Description);
public record LogRow(string RecordId, int EventId, DateTimeOffset TimeUtc, string Account, string SourceIp);
public record LogReadResult(List<LogRow> Rows, List<string> Errors);
public record LoginAlert(string Account, string SourceIp, List<LogRow> Evidence);
// [MODELS]
using System.Runtime.InteropServices;
namespace SecurityInspector;
public static partial class SystemService
{
    public static string Describe() => $"Máy: {Environment.MachineName}\nWindows: {RuntimeInformation.OSDescription}\nKiến trúc OS: {RuntimeInformation.OSArchitecture}\nCPU logic: {Environment.ProcessorCount}".Replace("\n", Environment.NewLine);

    public static List<Finding> ParseFirewall(string json)
    {
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var elements = doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array
            ? doc.RootElement.EnumerateArray().ToArray() : new[] { doc.RootElement };
        var answer = new List<Finding>();
        foreach (string name in new[] { "Domain", "Private", "Public" })
        {
            var row = elements.FirstOrDefault(x => x.ValueKind == System.Text.Json.JsonValueKind.Object
                && x.TryGetProperty("Name", out var p) && p.GetString() == name);
            string state = "Không đọc được";
            if (row.ValueKind == System.Text.Json.JsonValueKind.Object && row.TryGetProperty("Enabled", out var value))
            {
                string raw = value.ToString();
                if (raw.Equals("true", StringComparison.OrdinalIgnoreCase) || raw == "1") state = "Bật";
                else if (raw.Equals("false", StringComparison.OrdinalIgnoreCase) || raw == "0") state = "Tắt";
            }
            answer.Add(Finding.New("Firewall", state, name, "Cấu hình profile; không khẳng định profile đang là mạng hoạt động."));
        }
        return answer;
    }

    public static async Task<List<Finding>> FirewallAsync(CancellationToken token)
    {
        try { return ParseFirewall(await FixedCommand.FirewallAsync(token)); }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            return new[] { "Domain", "Private", "Public" }.Select(p => Finding.New("Firewall", "Không đọc được", p, ex.Message)).ToList();
        }
    }
    // [SYSTEM]
}
using System.Diagnostics;
namespace SecurityInspector;
public static class FixedCommand
{
    // Chỉ lệnh Firewall cố định; không nhận chuỗi PowerShell do người dùng nhập.
    public static async Task<string> FirewallAsync(CancellationToken token)
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("Chức năng này cần Windows.");
        string exe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell", "v1.0", "powershell.exe");
        var start = new ProcessStartInfo(exe)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        foreach (string a in new[] { "-NoProfile", "-NonInteractive", "-Command",
            "Get-NetFirewallProfile -PolicyStore ActiveStore -ErrorAction Stop | Select-Object Name,Enabled | ConvertTo-Json -Compress" }) start.ArgumentList.Add(a);
        using var process = new Process { StartInfo = start };
        process.Start();
        var stdout = process.StandardOutput.ReadToEndAsync(); var stderr = process.StandardError.ReadToEndAsync();
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(token); deadline.CancelAfter(TimeSpan.FromSeconds(10));
        try
        {
            await process.WaitForExitAsync(deadline.Token);
            string text = await stdout, error = await stderr;
            if (process.ExitCode != 0) throw new IOException("Không đọc được Firewall: " + error);
            return text;
        }
        finally
        {
            try { if (!process.HasExited) process.Kill(true); } catch (InvalidOperationException) { }
            await process.WaitForExitAsync(); await stdout; await stderr;
        }
    }
}
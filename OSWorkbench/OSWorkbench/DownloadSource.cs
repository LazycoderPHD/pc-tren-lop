using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace OSWorkbench
{
    // Nguồn dữ liệu cho worker: đọc một "mảnh" (chunk) mỗi lần gọi
    public interface IDownloadSource
    {
        int ReadChunk(DownloadJob job, byte[] buffer);
    }

    // ---------- Nguồn GIẢ LẬP: không cần Internet, dùng để học và chạy trong phòng lab ----------
    public class SimulatedSource : IDownloadSource
    {
        public int ChunkBytes = 64 * 1024;      // Mỗi mảnh 64 KB
        public int DelayMs = 25;                // Thời gian "mạng" cho mỗi mảnh
        public int ServerLimit = 2;             // Số file tối đa một máy chủ phục vụ song song

        private readonly Dictionary<string, SemaphoreSlim> _limits = new Dictionary<string, SemaphoreSlim>();
        private readonly object _gate = new object();

        private SemaphoreSlim LimitFor(string host)
        {
            lock (_gate)
            {
                if (!_limits.TryGetValue(host, out SemaphoreSlim? s))
                {
                    s = new SemaphoreSlim(ServerLimit, ServerLimit);
                    _limits[host] = s;
                }
                return s;
            }
        }

        public int ReadChunk(DownloadJob job, byte[] buffer)
        {
            long remain = job.TotalBytes - job.DoneBytes;
            if (remain <= 0) return 0;

            SemaphoreSlim limit = LimitFor(job.Host);
            limit.Wait();
            try
            {
                // File có tên chứa "loi" sẽ lỗi đúng một lần (để luyện xử lý lỗi + thử lại)
                if (job.Name.Contains("loi") && job.Retry == 0)
                    throw new IOException("Mất kết nối tới " + job.Host + " (lỗi giả lập)");

                Thread.Sleep(DelayMs);              // Giả lập độ trễ mạng
                return (int)Math.Min(ChunkBytes, remain);
            }
            finally
            {
                limit.Release();
            }
        }
    }

    // ---------- Nguồn THẬT: tải bằng HTTP, có Range để tải tiếp khi bị dừng ----------
    public class HttpSource : IDownloadSource
    {
        public int ChunkBytes = 64 * 1024;
        private readonly HttpClient _client;

        public HttpSource()
        {
            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        }

        public int ReadChunk(DownloadJob job, byte[] buffer)
        {
            long remain = job.TotalBytes - job.DoneBytes;
            if (remain <= 0) return 0;
            int want = (int)Math.Min(ChunkBytes, remain);

            using var request = new HttpRequestMessage(HttpMethod.Get, job.Source);
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(job.DoneBytes, job.DoneBytes + want - 1);
            using HttpResponseMessage response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            using Stream stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
            int total = 0;
            while (total < want)
            {
                int n = stream.Read(buffer, total, want - total);
                if (n <= 0) break;
                total += n;
            }
            return total;
        }
    }
}
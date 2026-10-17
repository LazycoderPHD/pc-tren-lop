using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace OSWorkbench
{
    /// <summary>
    /// Bộ tải file đa luồng: một HÀNG ĐỢI file dùng chung + N worker thread cùng lấy việc.
    /// </summary>
    public sealed class DownloadEngine
    {
        private readonly object _gate = new object();                  // "chìa khóa" của đoạn găng
        private readonly Queue<DownloadJob> _queue = new Queue<DownloadJob>();
        private readonly List<DownloadJob> _jobs = new List<DownloadJob>();
        private readonly List<Thread> _workers = new List<Thread>();

        private readonly ManualResetEventSlim _resume = new ManualResetEventSlim(true);  // Đóng = tạm dừng
        private volatile bool _stop = false;
        private long _totalDone = 0;                                  // Bộ đếm DÙNG CHUNG
        private long _chunks = 0;                                     // Bộ đếm DÙNG CHUNG
        private readonly Stopwatch _watch = new Stopwatch();
        private int _nextId = 1;
        private readonly List<string> _workerErrors = new List<string>();

        public IDownloadSource Source { get; set; } = new SimulatedSource();
        public int WorkerCount { get; set; } = 4;
        public bool UseLock { get; set; } = true;                     // Bật/tắt khóa để làm thí nghiệm
        public bool WidenRace { get; set; } = false;                  // "Phóng đại khe hở" như buổi 3
        public int MaxRetry { get; set; } = 2;

        private void Enter() { if (UseLock) Monitor.Enter(_gate); }
        private void Exit() { if (UseLock) Monitor.Exit(_gate); }

        public bool IsRunning { get; private set; }
        public bool IsPaused => !_resume.IsSet;

        // ---------- Thêm việc ----------
        public DownloadJob Add(string name, long totalBytes, string host, string source)
        {
            var job = new DownloadJob
            {
                Id = _nextId++,
                Name = name,
                TotalBytes = totalBytes,
                Host = host,
                Source = source,
                State = JobState.Cho,
                StartTime = DateTime.Now,
                LastTick = DateTime.Now
            };
            Enter();
            _jobs.Add(job);
            _queue.Enqueue(job);
            Exit();
            return job;
        }

        public void AddSamples(int count)
        {
            string[] names = { "baocao.pdf", "video-bai-giang.mp4", "dataset.zip", "slide-chuong-3.pptx", "file-loi.log", "ghichu.txt" };
            for (int i = 0; i < count; i++)
            {
                string name = names[i % names.Length];
                string host = i % 2 == 0 ? "maychu-A" : "maychu-B";
                long size = (i % 3 == 0 ? 4L : 2L) * 1024 * 1024;
                Add(name + (i >= names.Length ? "-" + i : ""), size, host, "sim://" + host + "/" + name);
            }
        }

        public void ClearAll()
        {
            if (IsRunning) return;
            Enter();
            _jobs.Clear();
            _queue.Clear();
            _totalDone = 0;
            _chunks = 0;
            _nextId = 1;
            Exit();
        }

        // ---------- Điều khiển ----------
        public void Start()
        {
            if (IsRunning) return;

            Enter();
            foreach (var j in _jobs)
            {
                if (j.State == JobState.DaHuy)
                {
                    j.State = JobState.Cho;
                    _queue.Enqueue(j);
                }
            }
            Exit();

            IsRunning = true;
            _stop = false;
            _resume.Set();
            _watch.Restart();
            _workerErrors.Clear();

            int n = Math.Max(1, Math.Min(16, WorkerCount));
            for (int i = 0; i < n; i++)
            {
                var th = new Thread(WorkerLoop) { IsBackground = true };
                _workers.Add(th);
                th.Start();
            }
        }

        public void Pause() => _resume.Reset();
        public void Resume() => _resume.Set();

        public void CancelAll()
        {
            _stop = true;
            _resume.Set();
            foreach (var th in _workers) th.Join();
            Enter();
            _queue.Clear();                           // Dọn hàng đợi sót tránh xếp trùng
            foreach (var j in _jobs)
                if (j.State == JobState.Cho || j.State == JobState.DangTai) j.State = JobState.DaHuy;
            Exit();
            _workers.Clear();
            _watch.Stop();
            IsRunning = false;
        }

        // ---------- Thân của một worker ----------
        private void WorkerLoop()
        {
            byte[] buffer = new byte[64 * 1024];
            while (!_stop)
            {
                _resume.Wait();
                if (_stop) break;

                DownloadJob? job = null;
                try
                {
                    Enter();
                    if (_queue.Count > 0)
                    {
                        job = _queue.Dequeue();
                        job.State = JobState.DangTai;
                    }
                    Exit();
                }
                catch (Exception ex)
                {
                    Exit();
                    lock (_workerErrors) { _workerErrors.Add("hàng đợi: " + ex.GetType().Name); }
                    continue;
                }

                if (job == null)
                {
                    Thread.Sleep(30);
                    continue;
                }

                try
                {
                    while (!_stop && job.DoneBytes < job.TotalBytes)
                    {
                        _resume.Wait();
                        if (_stop) break;

                        int n = Source.ReadChunk(job, buffer);
                        if (n <= 0) break;

                        long remain = job.TotalBytes - job.DoneBytes;
                        if (n > remain) n = (int)remain;

                        job.DoneBytes += n;

                        Enter();
                        long temp = _totalDone;
                        if (WidenRace) Thread.SpinWait(20);
                        _totalDone = temp + n;
                        _chunks++;
                        Exit();
                    }
                    if (job.DoneBytes >= job.TotalBytes) job.State = JobState.Xong;
                }
                catch (Exception ex)
                {
                    job.Retry++;
                    if (job.Retry <= MaxRetry)
                    {
                        job.Note = "Lỗi " + ex.GetType().Name + " → thử lại lần " + job.Retry;
                        Enter(); _queue.Enqueue(job); job.State = JobState.Cho; Exit();
                    }
                    else
                    {
                        job.State = JobState.Loi;
                        job.Note = "Thất bại sau " + job.Retry + " lần: " + ex.Message;
                    }
                }
            }
        }

        public List<DownloadRow> Snapshot()
        {
            var list = new List<DownloadRow>();
            Enter();
            foreach (var j in _jobs)
            {
                list.Add(new DownloadRow
                {
                    Id = j.Id,
                    Name = j.Name,
                    Source = j.Source,
                    TienDo = Mb(j.DoneBytes) + " / " + Mb(j.TotalBytes),
                    PhanTram = j.Percent(),
                    TocDo = Speed(j),
                    TrangThai = Ten(j.State),
                    GhiChu = j.Note
                });
            }
            Exit();
            return list;
        }

        public DownloadSummary Summary()
        {
            Enter();
            long done = _totalDone;
            long sumJob = 0;
            int xong = 0, dangTai = 0, cho = 0, loi = 0, huy = 0, quaCao = 0;
            foreach (var j in _jobs)
            {
                sumJob += j.DoneBytes;
                if (j.DoneBytes > j.TotalBytes) quaCao++;
                switch (j.State)
                {
                    case JobState.Xong: xong++; break;
                    case JobState.DangTai: dangTai++; break;
                    case JobState.Cho: cho++; break;
                    case JobState.Loi: loi++; break;
                    default: huy++; break;
                }
            }
            var errors = _workerErrors.Distinct().ToList();
            Exit();

            bool ok = (done == sumJob) && quaCao == 0 && errors.Count == 0;
            string msg = ok
                ? "OK (bộ đếm chung khớp tổng từng file)"
                : "SAI: bộ đếm chung " + done + " ≠ tổng từng file " + sumJob +
                  (quaCao > 0 ? " · " + quaCao + " file vượt quá dung lượng" : "") +
                  (errors.Count > 0 ? " · lỗi: " + string.Join(", ", errors) : "");

            double sec = _watch.Elapsed.TotalSeconds;
            return new DownloadSummary
            {
                ThoiGian = sec.ToString("F2") + " s",
                TongDaTai = Mb(done) + " MB",
                TocDoTong = sec <= 0.01 ? "—" : (done / 1024.0 / sec).ToString("F0") + " KB/s",
                SoJob = "xong " + xong + " · đang tải " + dangTai + " · chờ " + cho + " · lỗi " + loi + " · huỷ " + huy,
                BatBien = msg,
                BatBienOk = ok,
                ChunksTotal = _chunks
            };
        }

        private static string Ten(JobState s) => s switch
        {
            JobState.Cho => "chờ",
            JobState.DangTai => "đang tải",
            JobState.Xong => "xong",
            JobState.Loi => "lỗi",
            _ => "đã huỷ"
        };

        private static string Mb(long bytes) => (bytes / 1024.0 / 1024.0).ToString("F2");

        private static string Speed(DownloadJob j)
        {
            double sec = (DateTime.Now - j.LastTick).TotalSeconds;
            if (sec <= 0.001) return "—";
            long delta = j.DoneBytes - j.LastBytes;
            if (delta < 0) return "—";
            return (delta / 1024.0 / sec).ToString("F0") + " KB/s";
        }

        public void TickSpeedMarks()
        {
            Enter();
            foreach (var j in _jobs)
            {
                j.LastBytes = j.DoneBytes;
                j.LastTick = DateTime.Now;
            }
            Exit();
        }
    }
}
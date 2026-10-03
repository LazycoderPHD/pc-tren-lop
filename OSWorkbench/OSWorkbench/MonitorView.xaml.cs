using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OSWorkbench
{
    public partial class MonitorView : UserControl
    {
        // ===== Dữ liệu DÙNG CHUNG giữa thread nền và UI thread =====
        private readonly object _dataLock = new object();
        private readonly List<ProcessEvent> _events = new List<ProcessEvent>();
        private List<CpuInfo> _cpuTable = new List<CpuInfo>();
        private int _snapshotCount = 0;

        // ===== Cờ điều khiển thread nền =====
        private Thread? _monitorThread;
        private volatile bool _running = false;
        private volatile bool _busyWait = false;
        private int _threshold = 50;

        // ===== Dữ liệu RIÊNG của thread nền =====
        private Dictionary<int, string> _lastSnapshot = new Dictionary<int, string>();
        private readonly Dictionary<int, TimeSpan> _lastCpuTime = new Dictionary<int, TimeSpan>();
        private readonly Dictionary<int, int> _highCount = new Dictionary<int, int>();
        private DateTime _lastSampleTime;

        // ===== Dùng cho UI thread =====
        private DispatcherTimer _uiTimer;
        private TimeSpan _selfLastCpu;
        private DateTime _selfLastTime;

        // ===== Phòng thí nghiệm =====
        private long _counter;
        private volatile int _flag;
        private readonly object _counterLock = new object();
        private readonly List<RaceResult> _raceResults = new List<RaceResult>();

        public MonitorView()
        {
            InitializeComponent();
            TxtThreads.Text = (Environment.ProcessorCount * 2).ToString();

            _uiTimer = new DispatcherTimer();
            _uiTimer.Interval = TimeSpan.FromSeconds(1);
            _uiTimer.Tick += (s, e) => RefreshUi();
            _uiTimer.Start();
        }

        // ================= PHÒNG THÍ NGHIỆM =================
        private void AddNoLock(bool widen)
        {
            long temp = _counter;
            if (widen) Thread.SpinWait(20);
            _counter = temp + 1;
        }

        private void AddWithFlag(bool widen)
        {
            while (_flag == 1) { }
            _flag = 1;
            long temp = _counter;
            if (widen) Thread.SpinWait(20);
            _counter = temp + 1;
            _flag = 0;
        }

        private void AddWithLock(bool widen)
        {
            lock (_counterLock)
            {
                long temp = _counter;
                if (widen) Thread.SpinWait(20);
                _counter = temp + 1;
            }
        }

        private void AddInterlocked()
        {
            Interlocked.Increment(ref _counter);
        }

        private static TimeSpan GetMyCpuTime()
        {
            using var me = Process.GetCurrentProcess();
            return me.TotalProcessorTime;
        }

        private RaceResult RunRace(string method, int threads, int increments, bool widen)
        {
            Action add = method switch
            {
                "Cờ tự chế" => () => AddWithFlag(widen),
                "lock" => () => AddWithLock(widen),
                "Interlocked" => AddInterlocked,
                _ => () => AddNoLock(widen)
            };

            _counter = 0;
            _flag = 0;
            using var startSignal = new ManualResetEventSlim(false);
            var list = new List<Thread>();
            for (int i = 0; i < threads; i++)
            {
                var t = new Thread(() =>
                {
                    startSignal.Wait();
                    for (int k = 0; k < increments; k++)
                    {
                        add();
                    }
                });
                list.Add(t);
                t.Start();
            }

            TimeSpan cpuBefore = GetMyCpuTime();
            var stopwatch = Stopwatch.StartNew();
            startSignal.Set();

            foreach (var t in list)
            {
                t.Join();
            }

            stopwatch.Stop();
            TimeSpan cpuAfter = GetMyCpuTime();

            long expected = (long)threads * increments;
            return new RaceResult
            {
                Method = widen && method != "Interlocked" ? method + " (phóng đại)" : method,
                Threads = threads,
                Increments = increments,
                Expected = expected,
                Actual = _counter,
                Lost = expected - _counter,
                TimeMs = stopwatch.ElapsedMilliseconds,
                CpuMs = (long)(cpuAfter - cpuBefore).TotalMilliseconds
            };
        }

        private async Task RunRaceTimes(int times)
        {
            if (!int.TryParse(TxtThreads.Text, out int threads) || threads < 1)
                threads = Environment.ProcessorCount;
            if (!int.TryParse(TxtIncrements.Text, out int increments) || increments < 1)
                increments = 1000000;
            string method = (CmbMethod.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Không khóa";
            bool widen = ChkWiden.IsChecked == true;

            BtnRace.IsEnabled = false;
            BtnRace5.IsEnabled = false;

            for (int i = 1; i <= times; i++)
            {
                TxtRaceStatus.Text = $"Đang chạy lần {i}/{times}...";
                RaceResult result = await Task.Run(() => RunRace(method, threads, increments, widen));
                _raceResults.Insert(0, result);
                GridRace.ItemsSource = _raceResults.ToList();
            }

            TxtRaceStatus.Text = "Xong.";
            BtnRace.IsEnabled = true;
            BtnRace5.IsEnabled = true;
        }

        private async void BtnRace_Click(object sender, RoutedEventArgs e)
        {
            await RunRaceTimes(1);
        }

        private async void BtnRace5_Click(object sender, RoutedEventArgs e)
        {
            await RunRaceTimes(5);
        }

        private void BtnClearRace_Click(object sender, RoutedEventArgs e)
        {
            _raceResults.Clear();
            GridRace.ItemsSource = null;
        }

        // ================= GIÁM SÁT =================
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtThreshold.Text, out _threshold) || _threshold <= 0)
                _threshold = 50;

            _running = true;
            _monitorThread = new Thread(MonitorLoop);
            _monitorThread.IsBackground = true;
            _monitorThread.Start();

            BtnStart.IsEnabled = false;
            BtnStop.IsEnabled = true;
        }

        private async void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _running = false;
            BtnStop.IsEnabled = false;

            Thread? t = _monitorThread;
            if (t != null)
            {
                await Task.Run(() => t.Join());
            }

            BtnStart.IsEnabled = true;
        }

        private void ChkBusyWait_Click(object sender, RoutedEventArgs e)
        {
            _busyWait = ChkBusyWait.IsChecked == true;
        }

        private void MonitorLoop()
        {
            _lastSnapshot.Clear();
            _lastCpuTime.Clear();
            _highCount.Clear();
            _lastSampleTime = DateTime.Now;
            bool first = true;

            while (_running)
            {
                TakeSnapshot(first);
                first = false;
                Interlocked.Increment(ref _snapshotCount);
                WaitOneSecond();
            }
        }

        private void WaitOneSecond()
        {
            if (_busyWait)
            {
                DateTime next = DateTime.Now.AddSeconds(1);
                while (DateTime.Now < next) { }
            }
            else
            {
                Thread.Sleep(1000);
            }
        }

        private void TakeSnapshot(bool first)
        {
            DateTime now = DateTime.Now;
            double wallMs = (now - _lastSampleTime).TotalMilliseconds;
            int cores = Environment.ProcessorCount;

            var current = new Dictionary<int, string>();
            var newEvents = new List<ProcessEvent>();
            var cpuList = new List<CpuInfo>();

            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    int pid = p.Id;
                    string name = p.ProcessName;
                    current[pid] = name;

                    if (!first && !_lastSnapshot.ContainsKey(pid))
                    {
                        newEvents.Add(new ProcessEvent
                        {
                            Time = now.ToString("HH:mm:ss"),
                            Kind = "Tạo mới",
                            Name = name,
                            Pid = pid
                        });
                    }

                    TimeSpan cpuNow;
                    try { cpuNow = p.TotalProcessorTime; }
                    catch { continue; }

                    if (_lastCpuTime.TryGetValue(pid, out TimeSpan cpuBefore) && wallMs > 0)
                    {
                        double percent = (cpuNow - cpuBefore).TotalMilliseconds / (wallMs * cores) * 100.0;

                        _highCount.TryGetValue(pid, out int high);
                        high = percent >= _threshold ? high + 1 : 0;
                        _highCount[pid] = high;

                        cpuList.Add(new CpuInfo
                        {
                            Pid = pid,
                            Name = name,
                            CpuPercent = Math.Round(percent, 1),
                            HighCount = high,
                            Warning = high >= 5 ? "⚠ CPU cao kéo dài" : ""
                        });
                    }
                    _lastCpuTime[pid] = cpuNow;
                }
                catch
                {
                }
                finally
                {
                    p.Dispose();
                }
            }

            foreach (var old in _lastSnapshot)
            {
                if (!current.ContainsKey(old.Key))
                {
                    newEvents.Add(new ProcessEvent
                    {
                        Time = now.ToString("HH:mm:ss"),
                        Kind = "Kết thúc",
                        Name = old.Value,
                        Pid = old.Key
                    });
                    _lastCpuTime.Remove(old.Key);
                    _highCount.Remove(old.Key);
                }
            }

            _lastSnapshot = current;
            _lastSampleTime = now;

            // Đoạn găng an toàn với lock
            lock (_dataLock)
            {
                _events.InsertRange(0, newEvents);
                if (_events.Count > 500)
                {
                    _events.RemoveRange(500, _events.Count - 500);
                }
                _cpuTable = cpuList.OrderByDescending(c => c.CpuPercent).Take(15).ToList();
            }
        }

        private void RefreshUi()
        {
            List<ProcessEvent> eventsCopy;
            List<CpuInfo> cpuCopy;

            lock (_dataLock)
            {
                eventsCopy = _events.ToList();
                cpuCopy = _cpuTable;
            }

            GridEvents.ItemsSource = eventsCopy;
            GridCpu.ItemsSource = cpuCopy;

            using var me = Process.GetCurrentProcess();
            TimeSpan cpuNow = me.TotalProcessorTime;
            DateTime now = DateTime.Now;
            string selfCpu = "—";
            if (_selfLastTime != default)
            {
                double percent = (cpuNow - _selfLastCpu).TotalMilliseconds
                                 / ((now - _selfLastTime).TotalMilliseconds * Environment.ProcessorCount) * 100.0;
                selfCpu = $"{percent:F1}%";
            }
            _selfLastCpu = cpuNow;
            _selfLastTime = now;

            string state = _running ? "Đang giám sát" : "Chưa giám sát";
            TxtMonitorStatus.Text = $"{state} · Đã chụp {_snapshotCount} lần · CPU của OS Workbench: {selfCpu}";
        }
    }
}
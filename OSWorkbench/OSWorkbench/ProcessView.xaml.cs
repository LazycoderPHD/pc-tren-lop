using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OSWorkbench
{
    public partial class ProcessView : UserControl
    {
        private DispatcherTimer _timer;

        public ProcessView()
        {
            InitializeComponent();
            LoadProcesses();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick += (s, e) => LoadProcesses();
            _timer.Start();
        }

        private static string SafeRead(Func<string> read)
        {
            try { return read(); }
            catch { return "(không có quyền)"; }
        }

        private void LoadProcesses()
        {
            int? selectedPid = (GridProcesses.SelectedItem as ProcessInfo)?.Id;
            var list = new List<ProcessInfo>();

            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    list.Add(new ProcessInfo
                    {
                        Id = p.Id,
                        ProcessName = p.ProcessName,
                        RamMB = Math.Round(p.WorkingSet64 / 1024.0 / 1024.0, 1),
                        ThreadCount = p.Threads.Count,
                        PriorityClassText = SafeRead(() => p.PriorityClass.ToString())
                    });
                }
                catch { }
                finally { p.Dispose(); }
            }

            var sorted = list.OrderBy(x => x.ProcessName).ToList();
            GridProcesses.ItemsSource = sorted;

            if (selectedPid != null)
            {
                GridProcesses.SelectedItem = sorted.FirstOrDefault(x => x.Id == selectedPid);
            }

            TxtStatus.Text = $"Cập nhật lúc {DateTime.Now:HH:mm:ss} — {list.Count} tiến trình";
        }

        private void LoadThreads(int pid)
        {
            var list = new List<ThreadInfo>();

            try
            {
                using var process = Process.GetProcessById(pid);
                foreach (ProcessThread t in process.Threads)
                {
                    list.Add(new ThreadInfo
                    {
                        ThreadId = t.Id,
                        StateText = SafeRead(() => t.ThreadState.ToString()),
                        PriorityText = SafeRead(() => t.PriorityLevel.ToString()),
                        StartTimeText = SafeRead(() => t.StartTime.ToString("HH:mm:ss"))
                    });
                }
            }
            catch { }

            GridThreads.ItemsSource = list;
            TxtThreadInfo.Text = $"Tiến trình PID {pid} có {list.Count} thread (Phần C: Khảo sát Priority).";

            if (list.Count > 50)
            {
                TxtSecurityWarning.Text =
                    "⚠ Số thread bất thường (> 50). Công cụ giám sát bảo mật (EDR) dùng kiểu tín hiệu này " +
                    "để đánh dấu tiến trình CẦN XEM XÉT KỸ HƠN.";
                TxtSecurityWarning.Visibility = Visibility.Visible;
            }
            else
            {
                TxtSecurityWarning.Visibility = Visibility.Collapsed;
            }
        }

        private void GridProcesses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridProcesses.SelectedItem is ProcessInfo selected)
            {
                LoadThreads(selected.Id);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProcesses();
        }

        private void BtnStartChild_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe");
            TxtStatus.Text = "Đã tạo tiến trình con notepad.exe.";
        }

        private void BtnMakeThreads_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                var worker = new System.Threading.Thread(() =>
                {
                    System.Threading.Thread.Sleep(30000);
                });
                worker.IsBackground = true;
                worker.Start();
            }
            TxtStatus.Text = "Đã tạo 10 thread nền để kiểm tra EDR.";
        }

        // Xử lý nút bấm Phần C: Thay đổi độ ưu tiên Thread
        private void BtnBoostPriority_Click(object sender, RoutedEventArgs e)
        {
            if (GridProcesses.SelectedItem is ProcessInfo selected)
            {
                try
                {
                    using var process = Process.GetProcessById(selected.Id);
                    foreach (ProcessThread t in process.Threads)
                    {
                        // Thử nâng mức độ ưu tiên của thread lên mức cao nhất có thể
                        t.PriorityLevel = ThreadPriorityLevel.Highest;
                    }
                    MessageBox.Show($"Đã nâng độ ưu tiên tất cả thread của PID {selected.Id} lên Highest thành công!", "Phần C - Khảo sát Priority");
                    LoadThreads(selected.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không đủ quyền để thay đổi độ ưu tiên thread hệ thống: {ex.Message}", "Thông báo");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một tiến trình ở bảng trên trước khi thao tác!", "Nhắc nhở");
            }
        }
    }
}
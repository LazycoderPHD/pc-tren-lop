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
            InitializeComponent();   // dựng giao diện từ file .xaml
            LoadProcesses();         // nạp dữ liệu lần đầu

            // Tự làm mới mỗi 2 giây
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick += (s, e) => LoadProcesses();
            _timer.Start();
        }

        // Đọc một thông tin "an toàn": nếu Windows từ chối quyền đọc
        // thì trả về một dòng chữ thay vì làm chương trình crash
        private static string SafeRead(Func<string> read)
        {
            try { return read(); }
            catch { return "(không có quyền)"; }
        }

        private void LoadProcesses()
        {
            // Nhớ PID của dòng đang chọn để chọn lại sau khi làm mới
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
                catch
                {
                    // Tiến trình vừa thoát đúng lúc đang đọc — bỏ qua
                }
                finally
                {
                    p.Dispose();   // trả lại tài nguyên (handle) cho Windows
                }
            }

            var sorted = list.OrderBy(x => x.ProcessName).ToList();
            GridProcesses.ItemsSource = sorted;

            if (selectedPid != null)
            {
                GridProcesses.SelectedItem = sorted.FirstOrDefault(x => x.Id == selectedPid);
            }

            TxtStatus.Text = $"Cập nhật lúc {DateTime.Now:HH:mm:ss} — {list.Count} tiến trình";
        }

        // Bước 6 sẽ thêm các hàm xử lý nút bấm vào ngay dưới dòng này

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProcesses();
        }

        private void BtnStartChild_Click(object sender, RoutedEventArgs e)
        {
            // Yêu cầu Windows tạo một tiến trình mới chạy notepad.exe
            Process.Start("notepad.exe");
            TxtStatus.Text = "Đã tạo tiến trình con notepad.exe — chờ tối đa 2 giây để thấy nó trong bảng.";
        }
    }
}
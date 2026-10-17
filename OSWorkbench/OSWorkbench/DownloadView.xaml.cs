using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace OSWorkbench
{
    public partial class DownloadView : UserControl
    {
        private readonly DownloadEngine _engine = new DownloadEngine();
        private readonly DispatcherTimer _timer;

        private readonly SolidColorBrush _okBrush = new SolidColorBrush(Color.FromRgb(0x2F, 0x73, 0x50));
        private readonly SolidColorBrush _badBrush = new SolidColorBrush(Color.FromRgb(0x9C, 0x3B, 0x22));

        public DownloadView()
        {
            InitializeComponent();
            _engine.WorkerCount = 4;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _timer.Tick += (s, e) => RefreshUi();
            _timer.Start();
        }

        private void RefreshUi()
        {
            _engine.TickSpeedMarks();

            List<DownloadRow> rows = _engine.Snapshot();
            GridJobs.ItemsSource = rows;

            DownloadSummary sum = _engine.Summary();
            TxtTime.Text = "Thời gian: " + sum.ThoiGian;
            TxtTotal.Text = "Đã tải: " + sum.TongDaTai;
            TxtRate.Text = "Tốc độ tổng: " + sum.TocDoTong;
            TxtJobs.Text = sum.SoJob + " · mảnh: " + sum.ChunksTotal.ToString("N0");

            TxtInvariant.Text = "Bất biến: " + sum.BatBien;
            BorInvariant.Background = sum.BatBienOk ? _okBrush : _badBrush;
            TxtInvariant.Foreground = Brushes.White;
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            if (_engine.IsRunning) return;
            _engine.AddSamples(6);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_engine.IsRunning) return;
            string url = TxtUrl.Text.Trim();
            if (url.Length == 0) return;
            double.TryParse(TxtSizeMb.Text, out double mb);
            if (mb <= 0) mb = 2;

            string host = "maychu-A";
            try
            {
                if (url.StartsWith("http")) host = new Uri(url).Host;
            }
            catch { }

            string name = url;
            int slash = url.LastIndexOf('/');
            if (slash >= 0 && slash + 1 < url.Length) name = url.Substring(slash + 1);

            _engine.Add(name, (long)(mb * 1024 * 1024), host, url);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (_engine.IsRunning) return;
            _engine.ClearAll();
            RefreshUi();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_engine.IsRunning) return;
            _engine.WorkerCount = (int)SldWorkers.Value;
            _engine.UseLock = ChkLock.IsChecked == true;
            _engine.WidenRace = ChkWiden.IsChecked == true;
            _engine.Start();
            BtnStart.IsEnabled = false;
            BtnCancel.IsEnabled = true;
            BtnPause.IsEnabled = true;
            BtnPause.Content = "Tạm dừng";
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (!_engine.IsRunning) return;
            if (_engine.IsPaused)
            {
                _engine.Resume();
                BtnPause.Content = "Tạm dừng";
            }
            else
            {
                _engine.Pause();
                BtnPause.Content = "Tiếp tục";
            }
        }

        private async void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            BtnCancel.IsEnabled = false;
            BtnPause.IsEnabled = false;
            DownloadEngine engine = _engine;

            await System.Threading.Tasks.Task.Run(() => engine.CancelAll());

            BtnStart.IsEnabled = true;
            RefreshUi();
        }

        private void SldWorkers_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtWorkers != null) TxtWorkers.Text = ((int)e.NewValue).ToString();
        }

        private void ChkLock_Click(object sender, RoutedEventArgs e)
        {
            _engine.UseLock = ChkLock.IsChecked == true;
        }

        private void ChkWiden_Click(object sender, RoutedEventArgs e)
        {
            _engine.WidenRace = ChkWiden.IsChecked == true;
        }

        private void CmbSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSource == null || _engine.IsRunning) return;
            bool http = CmbSource.SelectedIndex == 1;
            _engine.Source = http ? (IDownloadSource)new HttpSource() : new SimulatedSource();
        }
    }
}
using System.Windows;
namespace SecurityInspector;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        FirewallGrid.ItemsSource = _firewall; IntegrityGrid.ItemsSource = _integrity;
        ExeGrid.ItemsSource = _exe; LogGrid.ItemsSource = _logs; ReportGrid.ItemsSource = _all;
        CancelButton.Click += (_, _) => _operation?.Cancel();
        RootText.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SecurityInspectorLab", "Files");
        BaselineText.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SecurityInspectorLab", "baseline.json");
        _ = Task.Run(() => Record(Finding.New("App", "Thử Dispatcher", "", "Dòng này xuất phát từ worker; Record chuyển việc thêm hàng về UI.", "Mẫu kiểm tra WPF")));
        RefreshSystemButton.Click += RefreshSystem_Click;
        // [INIT]
    }

    readonly System.Collections.ObjectModel.ObservableCollection<Finding> _firewall = new();
    readonly System.Collections.ObjectModel.ObservableCollection<Finding> _integrity = new();
    readonly System.Collections.ObjectModel.ObservableCollection<Finding> _exe = new();
    readonly System.Collections.ObjectModel.ObservableCollection<Finding> _logs = new();
    readonly System.Collections.ObjectModel.ObservableCollection<Finding> _all = new();
    CancellationTokenSource? _operation;
    bool _busy = false, _closing = false;
    const int MaxHistory = 5000;

    void Record(Finding item)
    {
        void Add()
        {
            if (_closing) return;
            if (_all.Count >= MaxHistory) _all.RemoveAt(0);
            _all.Add(item);
            var list = item.Area switch
            {
                "Firewall" => _firewall,
                "Integrity" => _integrity,
                "EXE" => _exe,
                "Log" => _logs,
                _ => null
            };
            list?.Add(item);
        }
        if (_closing || Dispatcher.HasShutdownStarted) return;
        if (Dispatcher.CheckAccess()) Add();
        else Dispatcher.BeginInvoke((Action)Add);
    }

    async Task WorkAsync(Func<CancellationToken, Task> work)
    {
        if (_busy) { StatusText.Text = "Đang có tác vụ khác; chờ hoặc bấm Hủy."; return; }
        _busy = true; _operation = new CancellationTokenSource(); StatusText.Text = "Đang xử lý...";
        try { await work(_operation.Token); if (!_closing) StatusText.Text = "Hoàn tất lượt xử lý; xem kết quả và lỗi."; }
        catch (OperationCanceledException) { Record(Finding.New("App", "Đã hủy", "", "Lượt dở dang không được coi là đầy đủ.", "Thao tác người dùng")); if (!_closing) StatusText.Text = "Đã hủy; không coi kết quả dở dang là đầy đủ."; }
        catch (Exception ex) { Record(Finding.New("App", "Lỗi", "", ex.Message, "Ứng dụng / dữ liệu đầu vào")); if (!_closing) StatusText.Text = ex.Message; }
        finally { _operation.Dispose(); _operation = null; _busy = false; }
    }

    async void RefreshSystem_Click(object sender, RoutedEventArgs e) => await WorkAsync(async ct =>
    {
        SystemText.Text = SystemService.Describe();
        _firewall.Clear();
        foreach (var item in await SystemService.FirewallAsync(ct)) Record(item);
    });
    // [MEMBERS]
}
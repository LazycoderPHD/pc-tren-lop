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
        ChooseRootButton.Click += ChooseRoot_Click;
        SaveBaselineButton.Click += SaveBaseline_Click;
        CompareButton.Click += Compare_Click;
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
            var list = item.Area switch { "Firewall" => _firewall, "Integrity" => _integrity,
                "EXE" => _exe, "Log" => _logs, _ => null };
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

    string _integrityRoot = "Chưa quét";
    void ChooseRoot_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog { Title = "Chọn thư mục thực hành" };
        if (dialog.ShowDialog(this) == true) RootText.Text = dialog.FolderName;
    }

    async void SaveBaseline_Click(object sender, RoutedEventArgs e) => await WorkAsync(async ct =>
    {
        string root = RootText.Text, destination = BaselineText.Text;
        var data = await Task.Run(() => BaselineService.BuildAsync(root, ct), ct);
        await BaselineService.SaveAsync(data, destination, ct);
        _integrityRoot = data.Root;
        _integrity.Clear();
        Record(Finding.New("Integrity", "Baseline", data.Root, $"Đã lưu {data.Files.Count} tệp; {data.Issues.Count} phần lỗi/bỏ qua.", "Thư mục thực hành"));
        foreach (string issue in data.Issues) Record(Finding.New("Integrity", "Chưa đọc đủ", root, issue, "Thư mục thực hành"));
    });

    async void Compare_Click(object sender, RoutedEventArgs e) => await WorkAsync(async ct =>
    {
        string root = RootText.Text, path = BaselineText.Text;
        var baseline = await BaselineService.LoadAsync(path, ct);
        var current = await Task.Run(() => BaselineService.BuildAsync(root, ct), ct);
        var results = BaselineService.Compare(baseline, current);
        _integrityRoot = current.Root;
        _integrity.Clear(); foreach (var item in results) Record(item);
    });
    // [MEMBERS]
}
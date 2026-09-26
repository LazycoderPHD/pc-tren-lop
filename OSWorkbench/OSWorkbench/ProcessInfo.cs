namespace OSWorkbench
{
    // Lớp trung gian: mỗi đối tượng = 1 dòng trên bảng tiến trình
    public class ProcessInfo
    {
        public int Id { get; set; }                          // PID
        public string ProcessName { get; set; } = "";        // tên tiến trình
        public double RamMB { get; set; }                    // RAM đang dùng (MB)
        public int ThreadCount { get; set; }                 // số thread
        public string PriorityClassText { get; set; } = "";  // độ ưu tiên
    }
}
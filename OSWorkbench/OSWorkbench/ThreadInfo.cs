namespace OSWorkbench
{
    // Lớp trung gian: mỗi đối tượng = 1 dòng trên bảng Thread
    public class ThreadInfo
    {
        public int ThreadId { get; set; }
        public string StateText { get; set; } = "";
        public string PriorityText { get; set; } = "";
        public string StartTimeText { get; set; } = "";
    }
}
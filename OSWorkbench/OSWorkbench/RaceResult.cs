namespace OSWorkbench
{
    public class RaceResult
    {
        public string Method { get; set; } = "";   // Cách cộng
        public int Threads { get; set; }           // Số thread
        public int Increments { get; set; }        // Số lần cộng mỗi thread
        public long Expected { get; set; }         // Kết quả đúng
        public long Actual { get; set; }           // Kết quả thực tế
        public long Lost { get; set; }             // Số lần cộng bị mất
        public long TimeMs { get; set; }           // Thời gian chạy (ms)
        public long CpuMs { get; set; }            // Thời gian CPU đã tiêu (ms)
    }
}
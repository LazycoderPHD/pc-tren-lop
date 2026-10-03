namespace OSWorkbench
{
    public class ProcessEvent
    {
        public string Time { get; set; } = "";   // Giờ phát hiện (HH:mm:ss)
        public string Kind { get; set; } = "";   // "Tạo mới" hoặc "Kết thúc"
        public string Name { get; set; } = "";   // Tên tiến trình
        public int Pid { get; set; }             // PID
    }
}
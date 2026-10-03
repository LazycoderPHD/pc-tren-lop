namespace OSWorkbench
{
    public class CpuInfo
    {
        public int Pid { get; set; }
        public string Name { get; set; } = "";
        public double CpuPercent { get; set; }     // %CPU trong lần đo gần nhất
        public int HighCount { get; set; }         // Số lần đo liên tiếp vượt ngưỡng
        public string Warning { get; set; } = "";  // Cảnh báo (nếu có)
    }
}
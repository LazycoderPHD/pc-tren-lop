using System;

namespace OSWorkbench
{
    // Trạng thái của một file đang tải
    public enum JobState
    {
        Cho,        // Đang nằm trong hàng đợi
        DangTai,    // Một worker đang tải
        Xong,
        Loi,
        DaHuy
    }

    // Một file cần tải — dữ liệu DÙNG CHUNG giữa worker và UI
    public class DownloadJob
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Source { get; set; } = "";        // URL hoặc tên máy chủ giả lập
        public string Host { get; set; } = "";          // "máy chủ" để thí nghiệm giới hạn băng thông
        public long TotalBytes { get; set; }

        // Hai trường dưới đây worker GHI liên tục, UI ĐỌC để hiện tiến độ
        public long DoneBytes;
        public JobState State = JobState.Cho;

        public int Retry { get; set; }                  // Số lần đã thử lại
        public string Note { get; set; } = "";
        public DateTime StartTime = DateTime.Now;
        public DateTime LastTick = DateTime.Now;
        public long LastBytes;

        public double Percent()
        {
            if (TotalBytes <= 0) return 0;
            return Math.Round(DoneBytes * 100.0 / TotalBytes, 1);
        }
    }

    // Dòng bất biến để UI bind vào DataGrid (chụp trong lúc khóa)
    public class DownloadRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Source { get; set; } = "";
        public string TienDo { get; set; } = "";
        public double PhanTram { get; set; }
        public string TocDo { get; set; } = "";
        public string TrangThai { get; set; } = "";
        public string GhiChu { get; set; } = "";
    }

    // Số liệu tổng hợp của cả phiên tải
    public class DownloadSummary
    {
        public string ThoiGian { get; set; } = "";
        public string TongDaTai { get; set; } = "";
        public string TocDoTong { get; set; } = "";
        public string SoJob { get; set; } = "";
        public string BatBien { get; set; } = "";
        public bool BatBienOk { get; set; }
        public long ChunksTotal { get; set; }
    }
}
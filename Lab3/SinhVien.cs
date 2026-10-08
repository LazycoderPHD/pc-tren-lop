using System;

namespace Lab3
{
    public class SinhVien
    {
        // Các thuộc tính cơ bản của sinh viên
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public double DiemTB { get; set; }

        // Constructor mặc định
        public SinhVien()
        {
            MaSV = "";
            HoTen = "";
            DiemTB = 0.0;
        }

        // Constructor có tham số
        public SinhVien(string maSV, string hoTen, double diemTB)
        {
            MaSV = maSV;
            HoTen = hoTen;
            DiemTB = diemTB;
        }

        // Phương thức nhập thông tin sinh viên
        public void Nhap()
        {
            Console.Write("Nhập mã sinh viên: ");
            MaSV = Console.ReadLine()!;
            Console.Write("Nhập họ tên sinh viên: ");
            HoTen = Console.ReadLine()!;
            Console.Write("Nhập điểm trung bình: ");
            DiemTB = double.Parse(Console.ReadLine()!);
        }

        // Phương thức xuất thông tin sinh viên
        public void Xuat()
        {
            Console.WriteLine($"Mã SV: {MaSV} | Họ tên: {HoTen} | Điểm TB: {DiemTB}");
        }
    }
}
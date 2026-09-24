using OOP_buoi1;
using System;

namespace BaiTap1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            // Kiểm tra quản lý mảng sinh viên (Question 3)
            Console.WriteLine("=== MANAGE AN ARRAY OF STUDENTS (MangSinhVien) ===");
            MangSinhVien qlsv = new MangSinhVien();

            qlsv.NhapDanhSach();
            qlsv.HienThiDanhSach();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
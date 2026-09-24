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

            SinhVien sv = new SinhVien();

            Console.WriteLine("=== ENTER STUDENT INFORMATION ===");
            sv.NhapThongTin();

            Console.WriteLine("\n=== STUDENT INFORMATION DISPLAY ===");
            sv.HienThiThongTin();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
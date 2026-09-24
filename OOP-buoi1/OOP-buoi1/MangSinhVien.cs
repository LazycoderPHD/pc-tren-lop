using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_buoi1
{
    public class MangSinhVien
    {
        private SinhVien[] danhSach;
        private int n;

        // Constructor mặc định
        public MangSinhVien()
        {
            danhSach = new SinhVien[0];
            n = 0;
        }

        // Phương thức nhập danh sách n sinh viên
        public void NhapDanhSach()
        {
            while (true)
            {
                Console.Write("Enter the number of students (n, 0 < n <= 1,000,000): ");
                if (int.TryParse(Console.ReadLine(), out n) && n > 0 && n <= 1000000)
                {
                    break;
                }
                Console.WriteLine("-> Invalid number! Please enter n such that 0 < n <= 1,000,000.");
            }

            danhSach = new SinhVien[n];
            Console.WriteLine($"\n=== ENTER INFORMATION FOR {n} STUDENTS ===");
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"\n--- Student [{i + 1}] ---");
                danhSach[i] = new SinhVien();
                danhSach[i].NhapThongTin();
            }
        }

        // Phương thức hiển thị danh sách sinh viên
        public void HienThiDanhSach()
        {
            Console.WriteLine($"\n=== LIST OF STUDENTS (Total: {n}) ===");
            if (n == 0)
            {
                Console.WriteLine("The list is empty.");
                return;
            }

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"\n--- Student [{i + 1}] ---");
                danhSach[i].HienThiThongTin();
            }
        }
    }
}

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

        public MangSinhVien()
        {
            danhSach = new SinhVien[0];
            n = 0;
        }

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

        // ==========================================
        // CÁC CHỨC NĂNG MỚI CHO QUESTION 4
        // ==========================================

        // 1. Sắp xếp giảm dần theo điểm trung bình (Dùng thuật toán Bubble Sort thủ công)
        public void SapXepGiamTheoDiemTB()
        {
            if (n <= 1) return;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (danhSach[j].DiemTB < danhSach[j + 1].DiemTB)
                    {
                        // Hoán đổi vị trí hai sinh viên
                        SinhVien temp = danhSach[j];
                        danhSach[j] = danhSach[j + 1];
                        danhSach[j + 1] = temp;
                    }
                }
            }
        }

        // 2. Tìm kiếm sinh viên theo mã số (Trả về đối tượng SinhVien hoặc null)
        public SinhVien TimKiemTheoMaSo(string maSo)
        {
            for (int i = 0; i < n; i++)
            {
                // So sánh chuỗi không phân biệt hoa thường
                if (string.Equals(danhSach[i].MaSo, maSo, StringComparison.OrdinalIgnoreCase))
                {
                    return danhSach[i];
                }
            }
            return null!;
        }

        // 3. Thống kê và hiển thị số lượng sinh viên theo từng nhóm xếp loại
        public void ThongKeXepLoai()
        {
            int weak = 0, average = 0, good = 0, excellent = 0;

            for (int i = 0; i < n; i++)
            {
                string loai = danhSach[i].Loai;
                if (loai == "Weak") weak++;
                else if (loai == "Average") average++;
                else if (loai == "Good") good++;
                else if (loai == "Excellent") excellent++;
            }

            Console.WriteLine("\n=== STATISTICAL CLASSIFICATION ===");
            Console.WriteLine($"- Weak (< 5)          : {weak} students");
            Console.WriteLine($"- Average (5 - <7)   : {average} students");
            Console.WriteLine($"- Good (7 - <8)      : {good} students");
            Console.WriteLine($"- Excellent (>= 8)   : {excellent} students");
        }

        // 4. Tìm sinh viên có điểm trung bình cao nhất
        public void TimSinhVienDiemTBCaoNhat()
        {
            if (n == 0)
            {
                Console.WriteLine("The list is empty.");
                return;
            }

            float maxDiem = danhSach[0].DiemTB;
            // Tìm giá trị điểm cao nhất
            for (int i = 1; i < n; i++)
            {
                if (danhSach[i].DiemTB > maxDiem)
                {
                    maxDiem = danhSach[i].DiemTB;
                }
            }

            Console.WriteLine($"\n=== STUDENT(S) WITH THE HIGHEST GPA ({maxDiem}) ===");
            for (int i = 0; i < n; i++)
            {
                if (danhSach[i].DiemTB == maxDiem)
                {
                    danhSach[i].HienThiThongTin();
                }
            }
        }
    }
}

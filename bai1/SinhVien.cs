using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_buoi1
{
    public class SinhVien
    {
        public string MaSo { get; set; }
        public string HoTen { get; set; }
        public string ChuyenNganh { get; set; }
        public int NamSinh { get; set; }
        public float DiemTB { get; set; }

        public string Loai => XepLoai();

        // 1. Constructor mặc định (dùng cho svA)
        public SinhVien() { }

        // 2. Constructor có tham số (dùng cho svB)
        public SinhVien(string maSo, string hoTen, string chuyenNganh, int namSinh, float diemTB)
        {
            MaSo = maSo;
            HoTen = hoTen;
            ChuyenNganh = chuyenNganh;
            NamSinh = namSinh;
            DiemTB = diemTB;
        }

        // 3. Copy Constructor (dùng cho svC sao chép từ svB)
        public SinhVien(SinhVien other)
        {
            if (other != null)
            {
                MaSo = other.MaSo;
                HoTen = other.HoTen;
                ChuyenNganh = other.ChuyenNganh;
                NamSinh = other.NamSinh;
                DiemTB = other.DiemTB;
            }
        }

        public string XepLoai()
        {
            if (DiemTB < 5) return "Weak";
            if (DiemTB < 7) return "Average";
            if (DiemTB < 8) return "Good";
            return "Excellent";
        }

        public void NhapThongTin()
        {
            Console.Write("Enter Student ID (maSo): ");
            MaSo = Console.ReadLine()!;

            Console.Write("Enter Full Name (hoTen): ");
            HoTen = Console.ReadLine()!;

            Console.Write("Enter Major (chuyenNganh): ");
            ChuyenNganh = Console.ReadLine()!;

            int currentYear = DateTime.Now.Year;
            while (true)
            {
                Console.Write($"Enter Year of Birth (namSinh) [Age 17 - 70, i.e., {currentYear - 70} - {currentYear - 17}]: ");
                if (int.TryParse(Console.ReadLine(), out int ns))
                {
                    int age = currentYear - ns;
                    if (age >= 17 && age <= 70)
                    {
                        NamSinh = ns;
                        break;
                    }
                }
                Console.WriteLine("-> Invalid age! Age must be between 17 and 70. Please try again.");
            }

            while (true)
            {
                Console.Write("Enter Cumulative GPA (diemTB) [0 - 10]: ");
                if (float.TryParse(Console.ReadLine(), out float dtb) && dtb >= 0 && dtb <= 10)
                {
                    DiemTB = dtb;
                    break;
                }
                Console.WriteLine("-> Invalid GPA! GPA must be between 0 and 10. Please try again.");
            }
        }

        public void HienThiThongTin()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Student ID     : {MaSo}");
            Console.WriteLine($"Full Name      : {HoTen}");
            Console.WriteLine($"Major          : {ChuyenNganh}");
            Console.WriteLine($"Year of Birth  : {NamSinh} (Age: {DateTime.Now.Year - NamSinh})");
            Console.WriteLine($"Cumulative GPA : {DiemTB}");
            Console.WriteLine($"Classification : {Loai}");
            Console.WriteLine("----------------------------------------");
        }

    }
}

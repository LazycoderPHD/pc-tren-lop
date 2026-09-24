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

            // a. Create an empty object svA and input/display info
            Console.WriteLine("=== 1. svA Object ===");
            SinhVien svA = new SinhVien();
            svA.NhapThongTin();
            Console.WriteLine("\n-- Information of svA --");
            svA.HienThiThongTin();

            // b. Create an object svB with fixed data and display
            Console.WriteLine("\n=== 2. svB Object ===");
            SinhVien svB = new SinhVien("18DH001", "Lam Thanh Ngoc", "CNPM", 2000, 7.6f);
            Console.WriteLine("-- Information of svB --");
            svB.HienThiThongTin();

            // c. Create an object svC copied from svB
            Console.WriteLine("\n=== 3. svC Object (Copied from svB) ===");
            SinhVien svC = new SinhVien(svB);

            // Input a new full name and GPA to update svC
            Console.WriteLine("Enter new information to update svC:");
            Console.Write("Enter new Full Name: ");
            svC.HoTen = Console.ReadLine()!;

            while (true)
            {
                Console.Write("Enter new Cumulative GPA [0 - 10]: ");
                if (float.TryParse(Console.ReadLine(), out float newDtb) && newDtb >= 0 && newDtb <= 10)
                {
                    svC.DiemTB = newDtb;
                    break;
                }
                Console.WriteLine("-> Invalid GPA! Please try again.");
            }

            Console.WriteLine("\n-- Information of svC (After updating) --");
            svC.HienThiThongTin();

            // d. Display svB again to check if it changed
            Console.WriteLine("\n=== 4. Check svB Information Again ===");
            Console.WriteLine("-- Information of svB (Should remain unchanged) --");
            svB.HienThiThongTin();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
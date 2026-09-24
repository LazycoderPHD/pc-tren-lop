using OOP_buoi1;
using System;

namespace BaiTap1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Thiết lập mã hóa tiếng Việt cho console
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            MangSinhVien qlsv = new MangSinhVien();
            bool hasData = false;

            while (true)
            {
                Console.WriteLine("\n================ QUESTION 4 MENU ================");
                Console.WriteLine("1. Input and display student list (MangSinhVien)");
                Console.WriteLine("2. Sort students by GPA in descending order");
                Console.WriteLine("3. Search student by Student ID (maSo)");
                Console.WriteLine("4. Count students by classification group");
                Console.WriteLine("5. Find student(s) with the highest GPA");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option (0-5): ");

                string choice = Console.ReadLine()!;
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("=== 1. INPUT & DISPLAY STUDENT LIST ===");
                        qlsv.NhapDanhSach();
                        qlsv.HienThiDanhSach();
                        hasData = true;
                        break;

                    case "2":
                        if (!CheckData(hasData)) break;
                        Console.WriteLine("=== 2. SORT STUDENTS BY GPA (DESCENDING) ===");
                        qlsv.SapXepGiamTheoDiemTB();
                        qlsv.HienThiDanhSach();
                        break;

                    case "3":
                        if (!CheckData(hasData)) break;
                        Console.WriteLine("=== 3. SEARCH STUDENT BY ID ===");
                        Console.Write("Enter Student ID to search: ");
                        string searchId = Console.ReadLine()!;
                        SinhVien foundSv = qlsv.TimKiemTheoMaSo(searchId);
                        if (foundSv != null)
                        {
                            Console.WriteLine("-> Found student:");
                            foundSv.HienThiThongTin();
                        }
                        else
                        {
                            Console.WriteLine($"-> No student found with ID: {searchId}");
                        }
                        break;

                    case "4":
                        if (!CheckData(hasData)) break;
                        qlsv.ThongKeXepLoai();
                        break;

                    case "5":
                        if (!CheckData(hasData)) break;
                        qlsv.TimSinhVienDiemTBCaoNhat();
                        break;

                    case "0":
                        Console.WriteLine("Exiting program. Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid choice! Please select from 0 to 5.");
                        break;
                }

                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static bool CheckData(bool hasData)
        {
            if (!hasData)
            {
                Console.WriteLine("-> Please enter the student list first (Select option 1)!");
                return false;
            }
            return true;
        }
    }
}
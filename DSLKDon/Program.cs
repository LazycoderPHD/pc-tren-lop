using System;

namespace DSLKDon
{
    class Program
    {
        static void Main(string[] args)
        {
            MyList list = new MyList();
            
            Console.WriteLine("=== NHAP DANH SACH ===");
            list.InputWithoutDuplicates();
            Console.Write("Danh sach vua nhap: ");
            list.ShowList();

            Console.WriteLine("\n=== KIEM TRA SHIFT LEFT ===");
            Console.Write("Truoc khi ShiftLeft: ");
            list.ShowList();
            list.ShiftLeft();
            Console.Write("Sau khi ShiftLeft: ");
            list.ShowList();

            Console.WriteLine("\n=== KIEM TRA RSHIFT RIGHT ===");
            Console.Write("Truoc khi RShiftRight: ");
            list.ShowList();
            list.RShiftRight();
            Console.Write("Sau khi RShiftRight: ");
            list.ShowList();

            Console.WriteLine("\n=== KIEM TRA SAP XEP ===");
            list.InterchangeSort();
            Console.Write("Sau InterchangeSort: ");
            list.ShowList();

            Console.WriteLine("\nNhan phim bat ky de thoat...");
            Console.ReadKey();
        }
    }
}
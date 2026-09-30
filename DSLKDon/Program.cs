using System;

namespace DSLKDon
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== PHAN A & B: NHAP DANH SACH VA TACH CHAN/LE ===");
            MyList list1 = new MyList();
            list1.Input();

            Console.WriteLine("\nDanh sach goc (Original list):");
            list1.ShowList();

            // Test Câu 2: Tìm kiếm, Max, Min
            Console.WriteLine($"\n--- Thong tin co bản ---");
            Console.WriteLine($"Tong so phan tury (Count): {list1.Count}");

            if (!list1.IsEmpty())
            {
                Console.WriteLine($"Max value: {list1.GetMax()?.Data}");
                Console.WriteLine($"Min value: {list1.GetMin()?.Data}");
            }

            // Test Phần B - Câu 4: Tách chẵn / lẻ[cite: 10, 11]
            Console.WriteLine("\n--- Test Tach Chan / Le (Question 4) ---");
            MyList evenList = list1.GetEvenList();
            MyList oddList = list1.GetOddList();

            Console.Write("Even numbers list: ");
            evenList.ShowList();

            Console.Write("Odd numbers list: ");
            oddList.ShowList();

            // Test Phần C - Câu 5: Nối hai danh sách[cite: 11]
            Console.WriteLine("\n=== PHAN C: NOI HAI DANH SACH (Question 5) ===");
            Console.WriteLine("Nhap danh sach thu hai (list2):");
            MyList list2 = new MyList();
            list2.Input();

            Console.Write("Danh sach 1: ");
            list1.ShowList();
            Console.Write("Danh sach 2: ");
            list2.ShowList();

            // Thực hiện nối list1 và list2 thành list3[cite: 11]
            MyList list3 = MyList.JoinList(list1, list2);
            Console.Write("Danh sach sau khi noi (list3): ");
            list3.ShowList();

            Console.WriteLine("\nHoan tat kiem thu tat ca cac chuc nang!");
        }
    }
}
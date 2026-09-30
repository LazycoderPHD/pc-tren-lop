using System;

namespace DSLKDon
{
    class Program
    {
        static void TestInput()
        {
            MyList list = new MyList();
            Console.WriteLine("=== NHAP DANH SACH ===");
            list.Input();

            Console.WriteLine("\nInteger linked list:");
            list.ShowList();
            Console.WriteLine($"Total elements (Count): {list.Count}");

            // Test SearchX
            Console.Write("\nEnter value to search (x): ");
            if (int.TryParse(Console.ReadLine(), out int x))
            {
                IntNode found = list.SearchX(x);
                if (found != null)
                    Console.WriteLine($"-> Found {x} in the list!");
                else
                    Console.WriteLine($"-> {x} not found in the list.");
            }

            // Test GetMax & GetMin
            IntNode maxNode = list.GetMax();
            IntNode minNode = list.GetMin();

            if (maxNode != null && minNode != null)
            {
                Console.WriteLine($"-> Max value: {maxNode.Data}");
                Console.WriteLine($"-> Min value: {minNode.Data}");
            }
            else
            {
                Console.WriteLine("-> The list is empty.");
            }
        }

        static void Main(string[] args)
        {
            TestInput();
        }
    }
}
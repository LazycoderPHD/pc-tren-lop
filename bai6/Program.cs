using System;

namespace IntStack
{
    internal class Program
    {
        // --- Test cho ArrayStack (Part A) ---
        static void Input(ArrayStack arrStack)
        {
            int x;
            do
            {
                Console.Write("Enter value (negative to stop): ");
                if (!int.TryParse(Console.ReadLine(), out x)) continue;
                if (x < 0) break;

                if (!arrStack.Push(x))
                {
                    Console.WriteLine("Stack is full!");
                    break;
                }
            } while (true);
        }

        static void Output(ArrayStack arrStack)
        {
            while (!arrStack.IsEmpty)
            {
                int val;
                arrStack.Pop(out val);
                Console.Write($"{val} ");
            }
            Console.WriteLine();
        }

        static void TestArrayStack()
        {
            ArrayStack arrStack = new ArrayStack(1000);
            Input(arrStack);
            
            if (arrStack.GetTop(out int topVal))
            {
                Console.WriteLine($"Value currently at the top of ArrayStack: {topVal}");
            }

            Console.WriteLine("Values removed from ArrayStack:");
            Output(arrStack);
        }

        // --- Test cho ListStack (Part B) ---
        static void InputList(ListStack listStack)
        {
            int x;
            do
            {
                Console.Write("Enter value for ListStack (negative to stop): ");
                if (!int.TryParse(Console.ReadLine(), out x)) continue;
                if (x < 0) break;

                listStack.Push(x);
            } while (true);
        }

        static void OutputList(ListStack listStack)
        {
            while (!listStack.IsEmpty)
            {
                int val;
                listStack.Pop(out val);
                Console.Write($"{val} ");
            }
            Console.WriteLine();
        }

        static void TestListStack()
        {
            ListStack listStack = new ListStack();
            InputList(listStack);

            if (listStack.GetTop(out int topVal))
            {
                Console.WriteLine($"Value currently at the top of ListStack: {topVal}");
            }

            Console.WriteLine("Values removed from ListStack:");
            OutputList(listStack);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("=== TESTING ARRAY STACK ===");
            TestArrayStack();

            Console.WriteLine("\n=== TESTING LIST STACK ===");
            TestListStack();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
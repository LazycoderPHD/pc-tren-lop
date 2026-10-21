using System;

namespace IntQueue
{
    internal class Program
    {
        static void Input(ArrayQueue arrQueue)
        {
            int x;
            do
            {
                Console.Write("Enter value for ArrayQueue (negative to stop): ");
                if (!int.TryParse(Console.ReadLine(), out x)) continue;
                if (x < 0) break;

                if (!arrQueue.EnQueue(x))
                {
                    Console.WriteLine("Queue is full!");
                    break;
                }
            } while (true);
        }

        static void Output(ArrayQueue arrQueue)
        {
            while (!arrQueue.IsEmpty())
            {
                int val;
                arrQueue.DeQueue(out val);
                Console.Write($"{val} ");
            }
            Console.WriteLine();
        }

        static void TestArrayQueue()
        {
            ArrayQueue arrQueue = new ArrayQueue(1000);
            Input(arrQueue);
            if (arrQueue.GetFront(out int frontVal) && arrQueue.GetRear(out int rearVal))
            {
                Console.WriteLine($"Front: {frontVal}, Rear: {rearVal}");
            }
            Console.WriteLine("Values in the ArrayQueue:");
            Output(arrQueue);
        }

        static void InputList(ListQueue listQueue)
        {
            int x;
            do
            {
                Console.Write("Enter value for ListQueue (negative to stop): ");
                if (!int.TryParse(Console.ReadLine(), out x)) continue;
                if (x < 0) break;

                listQueue.EnQueue(x);
            } while (true);
        }

        static void OutputList(ListQueue listQueue)
        {
            while (!listQueue.IsEmpty())
            {
                int val;
                listQueue.DeQueue(out val);
                Console.Write($"{val} ");
            }
            Console.WriteLine();
        }

        static void TestListQueue()
        {
            ListQueue listQueue = new ListQueue();
            InputList(listQueue);
            if (listQueue.GetFront(out int frontVal) && listQueue.GetRear(out int rearVal))
            {
                Console.WriteLine($"Front: {frontVal}, Rear: {rearVal}");
            }
            Console.WriteLine("Values in the ListQueue:");
            OutputList(listQueue);
        }

        static void TestStackQueue()
        {
            StackQueue sq = new StackQueue();
            sq.EnQueue(10);
            sq.EnQueue(20);
            sq.EnQueue(30);

            if (sq.GetFront(out int front))
            {
                Console.WriteLine($"StackQueue Front: {front}");
            }

            Console.WriteLine("Values removed from StackQueue:");
            while (!sq.IsEmpty())
            {
                sq.DeQueue(out int val);
                Console.Write($"{val} ");
            }
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("=== 1. TEST ARRAY QUEUE ===");
            TestArrayQueue();

            Console.WriteLine("\n=== 2. TEST LIST QUEUE ===");
            TestListQueue();

            Console.WriteLine("\n=== 3. TEST STACK QUEUE (PART C) ===");
            TestStackQueue();
        }
    }
}
// MSSV: 25DH110167
// Họ tên: Đào Nguyên Phát
using System;

namespace IntAVL
{
    class Program
    {
        static MyAVLTree TestInput()
        {
            MyAVLTree tree = new MyAVLTree();
            tree.Input();

            Console.WriteLine("Pre-order (NLR) traversal:");
            tree.NLR();
            Console.WriteLine();

            Console.WriteLine("In-order (LNR) traversal:");
            tree.LNR();
            Console.WriteLine();

            Console.WriteLine("Post-order (LRN) traversal:");
            tree.LRN();
            Console.WriteLine();

            return tree;
        }

        static void TestSearchAndMetrics(MyAVLTree tree)
        {
            Console.WriteLine("Count (number of elements): " + tree.Count);
            Console.WriteLine("Height of tree: " + tree.Height);

            CMyAVLIntNode minNode = tree.FindMin();
            if (minNode != null)
                Console.WriteLine("Smallest value (Min): " + minNode.DataValue);

            CMyAVLIntNode maxNode = tree.FindMax();
            if (maxNode != null)
                Console.WriteLine("Largest value (Max): " + maxNode.DataValue);

            Console.Write("Enter value x to search: ");
            int x;
            if (int.TryParse(Console.ReadLine(), out x))
            {
                CMyAVLIntNode found = tree.FindX(x);
                if (found != null)
                    Console.WriteLine("Found node with value: " + found.DataValue);
                else
                    Console.WriteLine("Value " + x + " not found in the tree.");
            }
        }

        static void TestIsBalancedSeries()
        {
            Console.WriteLine("Testing IsBalanced() after a series of insertions:");
            MyAVLTree testTree = new MyAVLTree();
            int[] values = { 10, 20, 30, 40, 50, 25 };
            foreach (int val in values)
            {
                testTree.Insert(val);
                Console.Write("Inserted " + val + " -> IsBalanced: " + testTree.IsBalanced() + " | NLR: ");
                testTree.NLR();
                Console.WriteLine();
            }
        }

        static void TestRemove(MyAVLTree tree)
        {
            Console.Write("Enter number of values to remove: ");
            int n;
            int.TryParse(Console.ReadLine(), out n);
            for (int i = 0; i < n; i++)
            {
                Console.Write("Enter value x to remove: ");
                int x;
                int.TryParse(Console.ReadLine(), out x);

                Console.WriteLine("In-order (LNR) before removing " + x + ":");
                tree.LNR();
                Console.WriteLine();

                if (tree.RemoveX(x))
                    Console.WriteLine("Successfully removed " + x);
                else
                    Console.WriteLine("Value " + x + " does not exist in the tree");

                Console.WriteLine("In-order (LNR) after removing " + x + ":");
                tree.LNR();
                Console.WriteLine();

                Console.WriteLine("Pre-order (NLR) after removing " + x + ":");
                tree.NLR();
                Console.WriteLine();

                Console.WriteLine("IsBalanced after removal: " + tree.IsBalanced());
            }
        }

        static void Main(string[] args)
        {
            MyAVLTree tree = TestInput();
            TestSearchAndMetrics(tree);
            TestIsBalancedSeries();
            TestRemove(tree);
        }
    }
}
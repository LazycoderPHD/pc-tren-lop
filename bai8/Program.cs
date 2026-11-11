// MSSV: 25DH110167
// Họ tên: Đào Nguyên Phát
using System;

namespace CTDLGT_BT5
{
    class Program
    {
        static MyBinaryTree TestInput()
        {
            MyBinaryTree root = new MyBinaryTree();
            root.Input();
            Console.WriteLine("Pre-order (NLR) traversal:");
            root.PreOrder();
            Console.WriteLine("In-order (LNR) traversal:");
            root.LNR();
            Console.WriteLine("Post-order (LRN) traversal:");
            root.LRN();
            return root;
        }

        static void TestSearchAndCount(MyBinaryTree root)
        {
            Console.WriteLine("Count (number of elements): " + root.Count);
            Console.WriteLine("Height of tree: " + root.Height);
            Console.WriteLine("Number of leaf nodes: " + root.CountLeaf());

            IntTNode minNode = root.FindMin();
            if (minNode != null)
                Console.WriteLine("Smallest value (Min): " + minNode.Data);

            IntTNode maxNode = root.FindMax();
            if (maxNode != null)
                Console.WriteLine("Largest value (Max): " + maxNode.Data);

            Console.Write("Enter value x to search: ");
            int x;
            int.TryParse(Console.ReadLine(), out x);
            IntTNode found = root.FindX(x);
            if (found != null)
                Console.WriteLine("Found node with value: " + found.Data);
            else
                Console.WriteLine("Value " + x + " not found in the tree.");
        }

        static void TestRemove(MyBinaryTree root)
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
                root.LNR();
                if (root.RemoveX(x))
                    Console.WriteLine("Successfully removed " + x);
                else
                    Console.WriteLine("Value " + x + " does not exist in the tree");
                Console.WriteLine("In-order (LNR) after removing " + x + ":");
                root.LNR();
            }
        }

        static void TestLevelTraversal(MyBinaryTree root)
        {
            Console.WriteLine("Level-order traversal (ListByLevel):");
            root.ListByLevel();
            Console.WriteLine("Last level nodes (ListLastLevel):");
            root.ListLastLevel();
        }

        static void Main(string[] args)
        {
            MyBinaryTree tree = TestInput();
            TestSearchAndCount(tree);
            TestRemove(tree);
            TestLevelTraversal(tree);
        }
    }
}
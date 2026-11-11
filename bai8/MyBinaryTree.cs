using System;
using System.Collections.Generic;

namespace CTDLGT_BT5
{
    class MyBinaryTree
    {
        private IntTNode root;

        public IntTNode Root
        {
            get { return root; }
            set { root = value; }
        }

        public int Count
        {
            get
            {
                if (Root == null)
                    return 0;
                return Root.Count;
            }
        }

        public int Height
        {
            get
            {
                if (Root == null)
                    return 0;
                return Root.Height;
            }
        }

        public MyBinaryTree()
        {
            Root = null;
        }

        public bool Insert(int x)
        {
            if (Root == null)
            {
                Root = new IntTNode(x);
                return true;
            }
            return Root.InsertNode(x);
        }

        public void Input()
        {
            do
            {
                int x;
                Console.Write("Enter a value (a duplicate value ends input): ");
                int.TryParse(Console.ReadLine(), out x);
                if (Insert(x) == true)
                    Console.WriteLine("Value added to the tree");
                else
                {
                    Console.WriteLine("Duplicate value, stopping");
                    return;
                }
            } while (true);
        }

        public void PreOrder()
        {
            if (Root == null)
                return;
            Root.NLR();
            Console.WriteLine();
        }

        public void NLR()
        {
            PreOrder();
        }

        public void LNR()
        {
            if (Root == null)
                return;
            Root.LNR();
            Console.WriteLine();
        }

        public void LRN()
        {
            if (Root == null)
                return;
            Root.LRN();
            Console.WriteLine();
        }

        public int CountLeaf()
        {
            if (Root == null)
                return 0;
            return Root.CountLeaf();
        }

        public IntTNode FindX(int x)
        {
            if (Root == null)
                return null;
            return Root.FindX(x);
        }

        public IntTNode FindMin()
        {
            if (Root == null)
                return null;
            return Root.FindMin();
        }

        public IntTNode FindMax()
        {
            if (Root == null)
                return null;
            return Root.FindMax();
        }

        public bool RemoveX(int x)
        {
            bool removed;
            Root = RemoveNode(Root, x, out removed);
            return removed;
        }

        private IntTNode RemoveNode(IntTNode p, int x, out bool removed)
        {
            if (p == null)
            {
                removed = false;
                return null;
            }
            if (x < p.Data)
            {
                p.Left = RemoveNode(p.Left, x, out removed);
                return p;
            }
            if (x > p.Data)
            {
                p.Right = RemoveNode(p.Right, x, out removed);
                return p;
            }

            removed = true;
            if (p.Left == null && p.Right == null)
                return null;
            if (p.Left == null)
                return p.Right;
            if (p.Right == null)
                return p.Left;

            IntTNode successor = p.Right.FindMin();
            p.Data = successor.Data;
            bool temp;
            p.Right = RemoveNode(p.Right, successor.Data, out temp);
            return p;
        }

        public void ListByLevel()
        {
            if (Root == null)
                return;
            Queue<IntTNode> q = new Queue<IntTNode>();
            q.Enqueue(Root);
            while (q.Count > 0)
            {
                IntTNode current = q.Dequeue();
                Console.Write(current.Data + "; ");
                if (current.Left != null)
                    q.Enqueue(current.Left);
                if (current.Right != null)
                    q.Enqueue(current.Right);
            }
            Console.WriteLine();
        }

        public void ListLastLevel()
        {
            if (Root == null)
                return;
            int treeHeight = Height;
            int currentLevel = 1;
            Queue<IntTNode> q = new Queue<IntTNode>();
            q.Enqueue(Root);
            while (q.Count > 0)
            {
                int levelSize = q.Count;
                for (int i = 0; i < levelSize; i++)
                {
                    IntTNode current = q.Dequeue();
                    if (currentLevel == treeHeight)
                        Console.Write(current.Data + "; ");
                    if (current.Left != null)
                        q.Enqueue(current.Left);
                    if (current.Right != null)
                        q.Enqueue(current.Right);
                }
                currentLevel++;
            }
            Console.WriteLine();
        }
    }
}
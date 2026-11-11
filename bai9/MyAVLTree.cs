using System;

namespace IntAVL
{
    public class MyAVLTree
    {
        private CMyAVLIntNode root;

        public CMyAVLIntNode Root
        {
            get { return root; }
            set { root = value; }
        }

        public MyAVLTree()
        {
            root = null;
        }

        public int Height
        {
            get { return root == null ? 0 : root.HeightValue; }
        }

        public int Count
        {
            get { return root == null ? 0 : root.CountNodes(); }
        }

        public void Insert(int x)
        {
            if (root == null)
                root = new CMyAVLIntNode(x);
            else
                root = root.InsertNode(root, x);
        }

        public void Input()
        {
            do
            {
                int x;
                Console.Write("Enter a value (a duplicate value ends input): ");
                if (!int.TryParse(Console.ReadLine(), out x))
                    return;
                if (FindX(x) == null)
                {
                    Insert(x);
                    Console.WriteLine("Value added to the tree");
                }
                else
                {
                    Console.WriteLine("Duplicate value, stopping");
                    return;
                }
            } while (true);
        }

        public void NLR()
        {
            if (root == null)
                return;
            root.NLR();
        }

        public void LNR()
        {
            if (root == null)
                return;
            root.LNR();
        }

        public void LRN()
        {
            if (root == null)
                return;
            root.LRN();
        }

        public CMyAVLIntNode FindX(int x)
        {
            if (root == null)
                return null;
            return root.FindX(x);
        }

        public CMyAVLIntNode FindMin()
        {
            if (root == null)
                return null;
            return root.FindMin();
        }

        public CMyAVLIntNode FindMax()
        {
            if (root == null)
                return null;
            return root.FindMax();
        }

        public bool IsBalanced()
        {
            if (root == null)
                return true;
            return root.IsBalancedNode();
        }

        public bool RemoveX(int x)
        {
            if (FindX(x) == null)
                return false;
            root = root.RemoveNode(root, x);
            return true;
        }
    }
}
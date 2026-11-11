using System;

namespace CTDLGT_BT5
{
    class IntTNode
    {
        private int data;
        private IntTNode left;
        private IntTNode right;

        public int Data
        {
            get { return data; }
            set { data = value; }
        }

        public IntTNode Left
        {
            get { return left; }
            set { left = value; }
        }

        public IntTNode Right
        {
            get { return right; }
            set { right = value; }
        }

        public int Count
        {
            get
            {
                int l = (left == null) ? 0 : left.Count;
                int r = (right == null) ? 0 : right.Count;
                return 1 + l + r;
            }
        }

        public int Height
        {
            get
            {
                int l = (left == null) ? 0 : left.Height;
                int r = (right == null) ? 0 : right.Height;
                return 1 + Math.Max(l, r);
            }
        }

        public IntTNode(int x = 0)
        {
            data = x;
            left = null;
            right = null;
        }

        public bool InsertNode(int x)
        {
            if (data == x)
                return false;
            if (data > x)
            {
                if (left == null)
                    left = new IntTNode(x);
                else
                    return left.InsertNode(x);
            }
            else
            {
                if (right == null)
                    right = new IntTNode(x);
                else
                    return right.InsertNode(x);
            }
            return true;
        }

        public void NLR()
        {
            Console.Write(Data + "; ");
            if (left != null)
                left.NLR();
            if (right != null)
                right.NLR();
        }

        public void LNR()
        {
            if (left != null)
                left.LNR();
            Console.Write(Data + "; ");
            if (right != null)
                right.LNR();
        }

        public void LRN()
        {
            if (left != null)
                left.LRN();
            if (right != null)
                right.LRN();
            Console.Write(Data + "; ");
        }

        public int CountLeaf()
        {
            if (left == null && right == null)
                return 1;
            int l = (left == null) ? 0 : left.CountLeaf();
            int r = (right == null) ? 0 : right.CountLeaf();
            return l + r;
        }

        public IntTNode FindX(int x)
        {
            if (data == x)
                return this;
            if (data > x)
            {
                if (left == null)
                    return null;
                return left.FindX(x);
            }
            else
            {
                if (right == null)
                    return null;
                return right.FindX(x);
            }
        }

        public IntTNode FindMin()
        {
            if (left == null)
                return this;
            return left.FindMin();
        }

        public IntTNode FindMax()
        {
            if (right == null)
                return this;
            return right.FindMax();
        }
    }
}
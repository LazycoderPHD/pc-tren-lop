using System;

namespace IntAVL
{
    public class CMyAVLIntNode
    {
        private int Data { get; set; }
        private int Height { get; set; }
        private CMyAVLIntNode Left { get; set; }
        private CMyAVLIntNode Right { get; set; }

        public CMyAVLIntNode(int data)
        {
            Data = data;
            Height = 1;
            Left = null;
            Right = null;
        }

        public int DataValue
        {
            get { return Data; }
            set { Data = value; }
        }

        public int HeightValue
        {
            get { return Height; }
            set { Height = value; }
        }

        public CMyAVLIntNode LeftNode
        {
            get { return Left; }
            set { Left = value; }
        }

        public CMyAVLIntNode RightNode
        {
            get { return Right; }
            set { Right = value; }
        }

        public CMyAVLIntNode InsertNode(CMyAVLIntNode cur, int x)
        {
            if (cur == null) return new CMyAVLIntNode(x);

            if (x < cur.DataValue)
            {
                cur.LeftNode = InsertNode(cur.LeftNode, x);
            }
            else if (x > cur.DataValue)
            {
                cur.RightNode = InsertNode(cur.RightNode, x);
            }
            else
            {
                return cur;
            }

            cur.HeightValue = 1 + Math.Max(GetHeight(cur.LeftNode), GetHeight(cur.RightNode));
            int balance = cur.GetBalance();

            if (balance > 1 && x < cur.LeftNode.DataValue)
            {
                return cur.RightRotate();
            }

            if (balance < -1 && x > cur.RightNode.DataValue)
            {
                return cur.LeftRotate();
            }

            if (balance > 1 && x > cur.LeftNode.DataValue)
            {
                cur.LeftNode = cur.LeftNode.LeftRotate();
                return cur.RightRotate();
            }

            if (balance < -1 && x < cur.RightNode.DataValue)
            {
                cur.RightNode = cur.RightNode.RightRotate();
                return cur.LeftRotate();
            }

            return cur;
        }

        private int GetHeight(CMyAVLIntNode node)
        {
            return node != null ? node.HeightValue : 0;
        }

        public int GetBalance()
        {
            int hL = GetHeight(LeftNode);
            int hR = GetHeight(RightNode);
            return hL - hR;
        }

        public CMyAVLIntNode RightRotate()
        {
            CMyAVLIntNode T1 = LeftNode;
            CMyAVLIntNode RT1 = T1.RightNode;
            T1.RightNode = this;
            LeftNode = RT1;
            HeightValue = 1 + Math.Max(GetHeight(LeftNode), GetHeight(RightNode));
            T1.HeightValue = 1 + Math.Max(GetHeight(T1.LeftNode), GetHeight(T1.RightNode));
            return T1;
        }

        public CMyAVLIntNode LeftRotate()
        {
            CMyAVLIntNode T1 = RightNode;
            CMyAVLIntNode LT1 = T1.LeftNode;
            T1.LeftNode = this;
            RightNode = LT1;
            HeightValue = 1 + Math.Max(GetHeight(LeftNode), GetHeight(RightNode));
            T1.HeightValue = 1 + Math.Max(GetHeight(T1.LeftNode), GetHeight(T1.RightNode));
            return T1;
        }

        public void NLR()
        {
            Console.Write(DataValue + " ");
            LeftNode?.NLR();
            RightNode?.NLR();
        }

        public void LNR()
        {
            LeftNode?.LNR();
            Console.Write(DataValue + " ");
            RightNode?.LNR();
        }

        public void LRN()
        {
            LeftNode?.LRN();
            RightNode?.LRN();
            Console.Write(DataValue + " ");
        }

        public int CountNodes()
        {
            int l = LeftNode != null ? LeftNode.CountNodes() : 0;
            int r = RightNode != null ? RightNode.CountNodes() : 0;
            return 1 + l + r;
        }

        public CMyAVLIntNode FindX(int x)
        {
            if (DataValue == x)
                return this;
            if (x < DataValue)
                return LeftNode != null ? LeftNode.FindX(x) : null;
            return RightNode != null ? RightNode.FindX(x) : null;
        }

        public CMyAVLIntNode FindMin()
        {
            if (LeftNode == null)
                return this;
            return LeftNode.FindMin();
        }

        public CMyAVLIntNode FindMax()
        {
            if (RightNode == null)
                return this;
            return RightNode.FindMax();
        }

        public bool IsBalancedNode()
        {
            int balance = GetBalance();
            if (balance < -1 || balance > 1)
                return false;
            if (LeftNode != null && !LeftNode.IsBalancedNode())
                return false;
            if (RightNode != null && !RightNode.IsBalancedNode())
                return false;
            return true;
        }

        public CMyAVLIntNode RemoveNode(CMyAVLIntNode cur, int x)
        {
            if (cur == null)
                return null;

            if (x < cur.DataValue)
            {
                cur.LeftNode = RemoveNode(cur.LeftNode, x);
            }
            else if (x > cur.DataValue)
            {
                cur.RightNode = RemoveNode(cur.RightNode, x);
            }
            else
            {
                if (cur.LeftNode == null || cur.RightNode == null)
                {
                    CMyAVLIntNode temp = cur.LeftNode != null ? cur.LeftNode : cur.RightNode;
                    if (temp == null)
                        cur = null;
                    else
                        cur = temp;
                }
                else
                {
                    CMyAVLIntNode successor = cur.RightNode.FindMin();
                    cur.DataValue = successor.DataValue;
                    cur.RightNode = RemoveNode(cur.RightNode, successor.DataValue);
                }
            }

            if (cur == null)
                return null;

            cur.HeightValue = 1 + Math.Max(GetHeight(cur.LeftNode), GetHeight(cur.RightNode));
            int balance = cur.GetBalance();

            if (balance > 1 && cur.LeftNode.GetBalance() >= 0)
            {
                return cur.RightRotate();
            }

            if (balance > 1 && cur.LeftNode.GetBalance() < 0)
            {
                cur.LeftNode = cur.LeftNode.LeftRotate();
                return cur.RightRotate();
            }

            if (balance < -1 && cur.RightNode.GetBalance() <= 0)
            {
                return cur.LeftRotate();
            }

            if (balance < -1 && cur.RightNode.GetBalance() > 0)
            {
                cur.RightNode = cur.RightNode.RightRotate();
                return cur.LeftRotate();
            }

            return cur;
        }
    }
}
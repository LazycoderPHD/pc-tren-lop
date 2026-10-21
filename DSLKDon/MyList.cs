using System;

namespace DSLKDon
{
    class MyList
    {
        private IntNode first;
        private IntNode last;

        public IntNode First
        {
            get { return first; }
            set { first = value; }
        }

        public IntNode Last
        {
            get { return last; }
            set { last = value; }
        }

        public MyList()
        {
            first = null;
            last = null;
        }

        public bool IsEmpty()
        {
            return first == null;
        }

        public void AddFirst(IntNode newNode)
        {
            if (IsEmpty())
            {
                first = last = newNode;
            }
            else
            {
                newNode.Next = first;
                first = newNode;
            }
        }

        public void AddLast(IntNode newNode)
        {
            if (IsEmpty())
            {
                first = last = newNode;
            }
            else
            {
                last.Next = newNode;
                last = newNode;
            }
        }

        public int Count
        {
            get
            {
                int count = 0;
                IntNode p = first;
                while (p != null)
                {
                    count++;
                    p = p.Next;
                }
                return count;
            }
        }

        public void Input()
        {
            int x;
            do
            {
                Console.Write("Value (0 to stop): ");
                int.TryParse(Console.ReadLine(), out x);
                if (x == 0)
                    return;
                IntNode newNode = new IntNode(x);
                AddFirst(newNode);
            } while (true);
        }

        public void ShowList()
        {
            IntNode p = first;
            while (p != null)
            {
                Console.Write("{0} -> ", p.Data);
                p = p.Next;
            }
            Console.WriteLine("null");
        }

        public IntNode SearchX(int x)
        {
            IntNode p = first;
            while (p != null)
            {
                if (p.Data == x) return p;
                p = p.Next;
            }
            return null;
        }

        public IntNode GetMax()
        {
            if (IsEmpty()) return null;
            IntNode maxNode = first;
            IntNode p = first.Next;
            while (p != null)
            {
                if (p.Data > maxNode.Data) maxNode = p;
                p = p.Next;
            }
            return maxNode;
        }

        public IntNode GetMin()
        {
            if (IsEmpty()) return null;
            IntNode minNode = first;
            IntNode p = first.Next;
            while (p != null)
            {
                if (p.Data < minNode.Data) minNode = p;
                p = p.Next;
            }
            return minNode;
        }

        public void InputWithoutDuplicates()
        {
            int x;
            do
            {
                Console.Write("Value (0 to stop): ");
                if (!int.TryParse(Console.ReadLine(), out x)) continue;
                if (x == 0) return;

                if (SearchX(x) != null)
                {
                    Console.WriteLine($"Gia tri {x} da ton tai! Vui long nhap gia tri khac.");
                    continue;
                }

                IntNode newNode = new IntNode(x);
                AddLast(newNode);
            } while (true);
        }

        public void RemoveAt(int i)
        {
            if (IsEmpty() || i < 0 || i >= Count) return;

            if (i == 0)
            {
                first = first.Next;
                if (first == null) last = null;
                return;
            }

            IntNode p = first;
            for (int idx = 0; idx < i - 1; idx++)
            {
                p = p.Next;
            }
            p.Next = p.Next.Next;
            if (p.Next == null)
            {
                last = p;
            }
        }

        public void RemoveX(int x)
        {
            if (IsEmpty()) return;

            if (first.Data == x)
            {
                first = first.Next;
                if (first == null) last = null;
                return;
            }

            IntNode p = first;
            while (p.Next != null && p.Next.Data != x)
            {
                p = p.Next;
            }

            if (p.Next != null)
            {
                if (p.Next == last)
                {
                    last = p;
                }
                p.Next = p.Next.Next;
            }
        }

        public void InsertAt(int x, int i)
        {
            if (i < 0 || i > Count) return;

            if (i == 0)
            {
                AddFirst(new IntNode(x));
                return;
            }
            if (i == Count)
            {
                AddLast(new IntNode(x));
                return;
            }

            IntNode p = first;
            for (int idx = 0; idx < i - 1; idx++)
            {
                p = p.Next;
            }
            IntNode newNode = new IntNode(x);
            newNode.Next = p.Next;
            p.Next = newNode;
        }

        public void InsertXAfterMin(int x)
        {
            IntNode minNode = GetMin();
            if (minNode == null) return;

            IntNode newNode = new IntNode(x);
            newNode.Next = minNode.Next;
            minNode.Next = newNode;
            if (minNode == last)
            {
                last = newNode;
            }
        }

        public void InsertXAfterY(int x, int y)
        {
            IntNode yNode = SearchX(y);
            if (yNode == null) return;

            IntNode newNode = new IntNode(x);
            newNode.Next = yNode.Next;
            yNode.Next = newNode;
            if (yNode == last)
            {
                last = newNode;
            }
        }

        public void ShiftLeft()
        {
            if (IsEmpty()) return;
            first = first.Next;
            if (first == null) last = null;
        }

        public void InsertXBeforeMax(int x)
        {
            IntNode maxNode = GetMax();
            if (maxNode == null) return;

            if (maxNode == first)
            {
                AddFirst(new IntNode(x));
                return;
            }

            IntNode p = first;
            while (p != null && p.Next != maxNode)
            {
                p = p.Next;
            }

            if (p != null)
            {
                IntNode newNode = new IntNode(x);
                newNode.Next = maxNode;
                p.Next = newNode;
            }
        }

        public void InsertXBeforeY(int x, int y)
        {
            IntNode yNode = SearchX(y);
            if (yNode == null) return;

            if (yNode == first)
            {
                AddFirst(new IntNode(x));
                return;
            }

            IntNode p = first;
            while (p != null && p.Next != yNode)
            {
                p = p.Next;
            }

            if (p != null)
            {
                IntNode newNode = new IntNode(x);
                newNode.Next = yNode;
                p.Next = newNode;
            }
        }

        public void RShiftRight()
        {
            if (IsEmpty() || first == last) return;

            IntNode p = first;
            while (p.Next != last)
            {
                p = p.Next;
            }

            last.Next = first;
            first = last;
            last = p;
            last.Next = null;
        }

        public void InterchangeSort()
        {
            for (IntNode p = first; p != null; p = p.Next)
            {
                for (IntNode q = p.Next; q != null; q = q.Next)
                {
                    if (p.Data > q.Data)
                    {
                        int temp = p.Data;
                        p.Data = q.Data;
                        q.Data = temp;
                    }
                }
            }
        }

        public void SelectionSort()
        {
            for (IntNode p = first; p != null; p = p.Next)
            {
                IntNode maxNode = p;
                for (IntNode q = p.Next; q != null; q = q.Next)
                {
                    if (q.Data > maxNode.Data)
                    {
                        maxNode = q;
                    }
                }
                if (maxNode != p)
                {
                    int temp = p.Data;
                    p.Data = maxNode.Data;
                    maxNode.Data = temp;
                }
            }
        }

        public static MyList MergeList(MyList list1, MyList list2)
        {
            MyList list3 = new MyList();
            IntNode p1 = list1.First;
            IntNode p2 = list2.First;

            while (p1 != null && p2 != null)
            {
                if (p1.Data <= p2.Data)
                {
                    list3.AddLast(new IntNode(p1.Data));
                    p1 = p1.Next;
                }
                else
                {
                    list3.AddLast(new IntNode(p2.Data));
                    p2 = p2.Next;
                }
            }

            while (p1 != null)
            {
                list3.AddLast(new IntNode(p1.Data));
                p1 = p1.Next;
            }

            while (p2 != null)
            {
                list3.AddLast(new IntNode(p2.Data));
                p2 = p2.Next;
            }

            return list3;
        }
    }
}
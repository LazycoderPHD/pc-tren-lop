using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Thuộc tính Count: đếm số phần tử
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

        public void AddFirst(IntNode newNode)
        {
            if (IsEmpty())
                first = last = newNode;
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
                AddLast(newNode); // Thêm vào cuối để giữ đúng thứ tự nhập
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

        // ==================== CÂU 2: TÌM KIẾM VÀ CỰC TRỊ ====================

        // 1. Tìm kiếm node có giá trị x
        public IntNode SearchX(int x)
        {
            IntNode p = first;
            while (p != null)
            {
                if (p.Data == x)
                    return p;
                p = p.Next;
            }
            return null;
        }

        // 2. Lấy node có giá trị lớn nhất
        public IntNode GetMax()
        {
            if (IsEmpty()) return null;

            IntNode maxNode = first;
            IntNode p = first.Next;
            while (p != null)
            {
                if (p.Data > maxNode.Data)
                {
                    maxNode = p;
                }
                p = p.Next;
            }
            return maxNode;
        }

        // 3. Lấy node có giá trị nhỏ nhất
        public IntNode GetMin()
        {
            if (IsEmpty()) return null;

            IntNode minNode = first;
            IntNode p = first.Next;
            while (p != null)
            {
                if (p.Data < minNode.Data)
                {
                    minNode = p;
                }
                p = p.Next;
            }
            return minNode;
        }

        // ==================== PHẦN B: CÂU 4 (Tách danh sách chẵn/lẻ) ====================

        // Trả về danh sách gồm các số chẵn
        public MyList GetEvenList()
        {
            MyList evenList = new MyList();
            IntNode p = first;
            while (p != null)
            {
                if (p.Data % 2 == 0)
                {
                    evenList.AddLast(new IntNode(p.Data));
                }
                p = p.Next;
            }
            return evenList;
        }

        // Trả về danh sách gồm các số lẻ
        public MyList GetOddList()
        {
            MyList oddList = new MyList();
            IntNode p = first;
            while (p != null)
            {
                if (p.Data % 2 != 0)
                {
                    oddList.AddLast(new IntNode(p.Data));
                }
                p = p.Next;
            }
            return oddList;
        }

        // ==================== PHẦN C: CÂU 5 (Nối hai danh sách) ====================

        // Tạo list3 bằng cách nối list2 vào sau list1 (sử dụng node mới hoàn toàn)[cite: 11]
        public static MyList JoinList(MyList list1, MyList list2)
        {
            MyList list3 = new MyList();

            // Sao chép các phần tử của list1 sang list3
            IntNode p = list1.First;
            while (p != null)
            {
                list3.AddLast(new IntNode(p.Data));
                p = p.Next;
            }

            // Sao chép các phần tử của list2 sang list3
            p = list2.First;
            while (p != null)
            {
                list3.AddLast(new IntNode(p.Data));
                p = p.Next;
            }

            return list3;
        }
    }
}

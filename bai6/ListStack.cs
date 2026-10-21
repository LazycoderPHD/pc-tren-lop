namespace IntStack
{
    class ListStack
    {
        Node top;

        internal Node Top { get => top; set => top = value; }

        public ListStack()
        {
            top = null;
        }

        public bool IsEmpty => top == null;

        public bool Push(int x)
        {
            Node newNode = new Node(x);
            newNode.Next = top;
            top = newNode;
            return true;
        }

        public bool Pop(out int outItem)
        {
            if (IsEmpty)
            {
                outItem = 0;
                return false;
            }
            outItem = top.Data;
            top = top.Next;
            return true;
        }

        public bool GetTop(out int outItem)
        {
            if (IsEmpty)
            {
                outItem = 0;
                return false;
            }
            outItem = top.Data;
            return true;
        }
    }
}
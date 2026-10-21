namespace IntQueue
{
    class ListQueue
    {
        Node front;
        Node rear;

        public ListQueue()
        {
            front = null;
            rear = null;
        }

        public bool IsEmpty()
        {
            // throw new NotImplementedException();
            return front == null;
        }

        public bool EnQueue(int x)
        {
            // throw new NotImplementedException();
            Node newNode = new Node(x);
            if (IsEmpty())
            {
                front = rear = newNode;
            }
            else
            {
                rear.Next = newNode;
                rear = newNode;
            }
            return true;
        }

        public bool DeQueue(out int outItem)
        {
            // throw new NotImplementedException();
            if (IsEmpty())
            {
                outItem = 0;
                return false;
            }
            outItem = front.Data;
            front = front.Next;
            if (front == null)
            {
                rear = null;
            }
            return true;
        }

        public bool GetFront(out int frontItem)
        {
            // throw new NotImplementedException();
            if (IsEmpty())
            {
                frontItem = 0;
                return false;
            }
            frontItem = front.Data;
            return true;
        }

        public bool GetRear(out int rearItem)
        {
            // throw new NotImplementedException();
            if (IsEmpty())
            {
                rearItem = 0;
                return false;
            }
            rearItem = rear.Data;
            return true;
        }
    }
}
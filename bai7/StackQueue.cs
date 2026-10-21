using System.Collections.Generic;

namespace IntQueue
{
    class StackQueue
    {
        private Stack<int> inStack;
        private Stack<int> outStack;

        public StackQueue()
        {
            inStack = new Stack<int>();
            outStack = new Stack<int>();
        }

        public bool IsEmpty()
        {
            return inStack.Count == 0 && outStack.Count == 0;
        }

        public void EnQueue(int x)
        {
            inStack.Push(x);
        }

        public bool DeQueue(out int outItem)
        {
            if (IsEmpty())
            {
                outItem = 0;
                return false;
            }

            MoveElements();
            outItem = outStack.Pop();
            return true;
        }

        public bool GetFront(out int frontItem)
        {
            if (IsEmpty())
            {
                frontItem = 0;
                return false;
            }

            MoveElements();
            frontItem = outStack.Peek();
            return true;
        }

        private void MoveElements()
        {
            if (outStack.Count == 0)
            {
                while (inStack.Count > 0)
                {
                    outStack.Push(inStack.Pop());
                }
            }
        }
    }
}
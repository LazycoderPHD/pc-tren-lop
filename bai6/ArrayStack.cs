namespace IntStack
{
    class ArrayStack
    {
        private int[] stkArray;
        private int stkMax;
        private int stkTop;

        public int[] StkArray { get => stkArray; set => stkArray = value; }
        public int StkMax { get => stkMax; set => stkMax = value; }
        public int StkTop { get => stkTop; set => stkTop = value; }

        public ArrayStack(int max = 0)
        {
            stkMax = max;
            stkArray = new int[max];
            stkTop = -1;
        }

        public bool IsEmpty => stkTop == -1;

        public bool IsFull => stkTop == stkMax - 1;

        public bool Push(int x)
        {
            if (IsFull) return false;
            stkTop++;
            stkArray[stkTop] = x;
            return true;
        }

        public bool Pop(out int outItem)
        {
            if (IsEmpty)
            {
                outItem = 0;
                return false;
            }
            outItem = stkArray[stkTop];
            stkTop--;
            return true;
        }

        public bool GetTop(out int outItem)
        {
            if (IsEmpty)
            {
                outItem = 0;
                return false;
            }
            outItem = stkArray[stkTop];
            return true;
        }
    }
}
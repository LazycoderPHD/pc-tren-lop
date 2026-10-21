using System;

namespace IntQueue
{
    class ArrayQueue
    {
        private int[] quArray; // array holding the elements
        private int quMax; // maximum number of elements
        private int quFront; // position of the front element
        private int quRear; // position of the rear element
        private int quCount; // current number of elements

        public int[] QuArray { get => quArray; set => quArray = value; }
        public int QuMax { get => quMax; set => quMax = value; }
        public int QuFront { get => quFront; set => quFront = value; }
        public int QuRear { get => quRear; set => quRear = value; }

        public ArrayQueue(int max = 0)
        {
            quMax = max;
            quArray = new int[max];
            quFront = 0;
            quRear = -1;
            quCount = 0;
        }

        public bool IsEmpty()
        {
            // throw new NotImplementedException();
            return quCount == 0;
        }

        public bool IsFull()
        {
            // throw new NotImplementedException();
            return quCount == quMax;
        }

        public bool EnQueue(int x)
        {
            // Remember: quRear = (quRear + 1) % quMax;
            // throw new NotImplementedException();
            if (IsFull()) return false;
            quRear = (quRear + 1) % quMax;
            quArray[quRear] = x;
            quCount++;
            return true;
        }

        public bool DeQueue(out int outItem)
        {
            // Remember: quFront = (quFront + 1) % quMax;
            // throw new NotImplementedException();
            if (IsEmpty())
            {
                outItem = 0;
                return false;
            }
            outItem = quArray[quFront];
            quFront = (quFront + 1) % quMax;
            quCount--;
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
            frontItem = quArray[quFront];
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
            rearItem = quArray[quRear];
            return true;
        }
    }
}
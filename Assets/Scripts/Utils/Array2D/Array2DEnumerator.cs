using System;
using System.Collections;

namespace Utils.Array2D
{
    public class Array2DEnumerator<TRealEnum> : I2DEnumerator<TRealEnum>
    {
        protected readonly TRealEnum[,] Array;
        protected int CountX = -1;
        protected int CountY;
        
        public Array2DEnumerator(TRealEnum[,] array)
        {
            Array = array;
        }

        public void Reset()
        {
            CountX = -1;
            CountY = 0;
        }

        object IEnumerator.Current => Current;
        public virtual TRealEnum Current
        {
            get
            {
                try
                {
                    return Array[CountX, CountY];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException($"InvalidOperationException: {GetType()} - Enumerator has ended!");
                }
            }
        }

        public void Dispose()
        {
        }

        public EnumerationStatus2D MoveNext2D()
        {
            if (CountX < Array.GetLength(0) - 1)
            {
                CountX++;
                return EnumerationStatus2D.Next;
            }

            if (CountY < Array.GetLength(1) - 1)
            {
                CountX = 0;
                CountY++;
                return EnumerationStatus2D.NextRow;
            }

            return EnumerationStatus2D.End;
        }
        
        public bool MoveNext()
        {
            return MoveNext2D() > 0;
        }
    }
}
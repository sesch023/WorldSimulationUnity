using System.Collections;
using System.Collections.Generic;

namespace Utils.Array2D
{
    public enum EnumerationStatus2D
    {
        End = 0,
        Next = 1,
        NextRow = 2
    }
    
    public interface I2DEnumerator<out T> : IEnumerator<T>
    {
        bool IEnumerator.MoveNext()
        {
            return MoveNext();
        }

        public new bool MoveNext();

        EnumerationStatus2D MoveNext2D();
    }
}
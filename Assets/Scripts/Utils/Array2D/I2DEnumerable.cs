using System.Collections;
using System.Collections.Generic;

namespace Utils.Array2D
{
    public interface I2DEnumerable<out T> : IEnumerable<T>
    {
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Get2DEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Get2DEnumerator();
        }

        I2DEnumerator<T> Get2DEnumerator();
    }
}
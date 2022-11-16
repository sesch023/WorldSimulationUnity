using System.Collections.Generic;

namespace Utils.BaseUtils
{
    public class CircularList<T> : List<T>
    {
        public T Last
        {
            get { return this[this.Count - 1]; }
            set { this[this.Count - 1] = value; }
        }

        public T First
        {
            get { return this[0]; }
            set { this[0] = value; }
        }

        public void PushLast(T obj)
        {
            this.Add(obj);
        }

        public T PopLast()
        {
            T retVal = this[this.Count - 1];
            this.RemoveAt(this.Count - 1);
            return retVal;
        }

        public void PushFirst(T obj)
        {
            this.Insert(0, obj);
        }

        public T PopFirst()
        {
            T retVal = this[0];
            this.RemoveAt(0);
            return retVal;
        }
    }
}
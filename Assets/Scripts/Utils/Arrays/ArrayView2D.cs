using System;
using System.Linq;
using Unity.VisualScripting;

namespace Utils.Arrays
{
    public class ArrayView2D<T> : I2DArray<T>
    {
        private readonly Array2D<T> _array;
        private readonly int _viewStartX;
        private readonly int _viewStartY;
        private readonly int _viewEndX;
        private readonly int _viewEndY;

        public ArrayView2D(Array2D<T> array) 
            : this(array, 0, 0, 
                array.GetLength(0), array.GetLength(1))
        {
            
        } 
        
        public ArrayView2D(T[,] array) : 
            this(new Array2D<T>(array), 0, 0, 
                array.GetLength(0), array.GetLength(1))
        {
            
        } 
        
        public ArrayView2D(Array2D<T> array, int viewStartX, int viewStartY, int viewEndX, int viewEndY)
        {
            if (viewStartX >= viewEndX || viewStartX < 0)
            {
                throw new ArgumentException($"ArgumentException: {GetType()} - Illegal value for viewStartX!");
            }
            
            if (viewStartY >= viewEndY || viewStartY < 0)
            {
                throw new ArgumentException($"ArgumentException: {GetType()} - Illegal value for viewStartY!");
            }
            
            if (viewEndX > array.GetLength(0) || viewEndX < 0)
            {
                throw new ArgumentException($"ArgumentException: {GetType()} - Illegal value for viewEndX!");
            }
            
            if (viewEndY > array.GetLength(1) || viewEndY < 0)
            {
                throw new ArgumentException($"ArgumentException: {GetType()} - Illegal value for viewEndY!");
            }
            
            _array = array;
            this._viewStartX = viewStartX;
            this._viewStartY = viewStartY;
            this._viewEndX = viewEndX;
            this._viewEndY = viewEndY;
        } 
        
        public ArrayView2D(T[,] array, int viewStartX, int viewStartY, int viewEndX, int viewEndY) 
            : this(new Array2D<T>(array), viewStartX, viewStartY, viewEndX, viewEndY)
        {
            
        }

        public I2DEnumerator<T> Get2DEnumerator()
        {
            return new Array2DEnumerator<T>(this);
        }

        public int GetLength(int dimension)
        {
            if (dimension == 0)
                return _viewStartX;
            if (dimension == 1)
                return _viewStartY;
            
            throw new ArgumentException($"ArgumentException: {GetType()}::GetLength - Illegal value for dimension!");
        }

        public T this[int x, int y]
        {
            get
            {
                if(x >= _viewEndX)
                    throw new IndexOutOfRangeException($"IndexOutOfRangeException: {GetType()}::get - X is out of range!");
                
                if(y >= _viewEndY)
                    throw new IndexOutOfRangeException($"IndexOutOfRangeException: {GetType()}::get - Y is out of range!");
                
                return _array[_viewStartX + x, _viewStartY + y];
            }
            set
            {
                if(x >= _viewEndX)
                    throw new IndexOutOfRangeException($"IndexOutOfRangeException: {GetType()}::set - X is out of range!");
                
                if(y >= _viewEndY)
                    throw new IndexOutOfRangeException($"IndexOutOfRangeException: {GetType()}::set - Y is out of range!");
                
                _array[_viewStartX + x, _viewStartY + y] = value;
            }
        }
    }
}
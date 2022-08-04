using System;

namespace Utils.Array2D
{
    /// <summary>
    /// Creates a enumerator on a 2D array view.
    /// </summary>
    /// <typeparam name="TRealEnum">Type of the internal array.</typeparam>
    /// <typeparam name="TViewEnum">Type of the view.</typeparam>
    public class ArrayView2DEnumerator<TRealEnum, TViewEnum> : Array2DEnumerator<TRealEnum>, I2DEnumerator<TViewEnum>
    {
        /// Accessor to the internal array.
        private readonly Func<TRealEnum, TViewEnum> _accessor;

        /// <summary>
        /// Creates a new enumerator on a 2D array view.
        /// </summary>
        /// <param name="array">Internal array.</param>
        /// <param name="accessor">Accessor to the internal array.</param>
        public ArrayView2DEnumerator(TRealEnum[,] array, Func<TRealEnum, TViewEnum> accessor) : base(array)
        {
            _accessor = accessor;
        }
        
        /// <summary>
        /// Creates a new enumerator on a 2D array view.
        /// </summary>
        /// <param name="arrayView2D">ArrayView2D to create an enumerator on.</param>
        public ArrayView2DEnumerator(ArrayView2D<TRealEnum, TViewEnum> arrayView2D) : base(arrayView2D.GetRealArray())
        {
            _accessor = arrayView2D.Accessor;
        }

        /// <summary>
        /// Returns the current element.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the enumerator has ended.</exception>
        public new TViewEnum Current
        {
            get
            {
                try
                {
                    return _accessor(Array[CountX, CountY]);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException($"InvalidOperationException: {GetType()} - Enumerator has ended!");
                }
            }
        }
    }
}
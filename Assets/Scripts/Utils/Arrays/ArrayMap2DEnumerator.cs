using System;

namespace Utils.Arrays
{
    /// <summary>
    /// Creates a enumerator on a 2D array immutableMap.
    /// </summary>
    /// <typeparam name="TRealEnum">Type of the internal array.</typeparam>
    /// <typeparam name="TMappedEnum">Type of the immutableMap.</typeparam>
    public class ArrayMap2DEnumerator<TRealEnum, TMappedEnum> : Array2DEnumerator<TRealEnum>, I2DEnumerator<TMappedEnum>
    {
        /// Accessor to the internal array.
        private readonly Func<TRealEnum, TMappedEnum> _accessor;

        /// <summary>
        /// Creates a new enumerator on a 2D array immutableMap.
        /// </summary>
        /// <param name="array">Internal array.</param>
        /// <param name="accessor">Accessor to the internal array.</param>
        public ArrayMap2DEnumerator(I2DArrayImmutable<TRealEnum> array, Func<TRealEnum, TMappedEnum> accessor) : base(array)
        {
            _accessor = accessor;
        }
        
        /// <summary>
        /// Creates a new enumerator on a 2D array immutableMap.
        /// </summary>
        /// <param name="arrayImmutableMap2D">ArrayImmutableMap2D to create an enumerator on.</param>
        public ArrayMap2DEnumerator(ArrayImmutableMap2D<TRealEnum, TMappedEnum> arrayImmutableMap2D) : base(arrayImmutableMap2D.GetRealArray())
        {
            _accessor = arrayImmutableMap2D.Accessor;
        }

        /// <summary>
        /// Returns the current element.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the enumerator has ended.</exception>
        public new TMappedEnum Current
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
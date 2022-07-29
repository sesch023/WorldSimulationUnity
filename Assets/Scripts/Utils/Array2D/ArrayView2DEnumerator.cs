using System;

namespace Utils.Array2D
{
    public class ArrayView2DEnumerator<TRealEnum, TViewEnum> : Array2DEnumerator<TRealEnum>, I2DEnumerator<TViewEnum>
    {
        private readonly Func<TRealEnum, TViewEnum> _accessor;

        public ArrayView2DEnumerator(TRealEnum[,] array, Func<TRealEnum, TViewEnum> accessor) : base(array)
        {
            _accessor = accessor;
        }

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
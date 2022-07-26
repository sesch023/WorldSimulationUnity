using System.Text;
using UnityEngine;

namespace Utils.Array2D
{
    // ReSharper disable once TypeParameterCanBeVariant
    public interface I2DArray<TReal> : I2DEnumerable<TReal>, IFixedDimensional
    {
        TReal this[int x, int y] { get; }

        public static string I2DArrayToString(I2DArray<TReal> array)
        {
            I2DEnumerator<TReal> enumerator = array.Get2DEnumerator();
            StringBuilder builder = new StringBuilder("[\n[ ");
            EnumerationStatus2D status = enumerator.MoveNext2D();
            while (status != EnumerationStatus2D.End)
            {
                if (status == EnumerationStatus2D.NextRow)
                {
                    builder.Length -= 2;
                    builder.Append(" ],\n[ ");
                }

                builder.Append(enumerator.Current).Append(", ");
                
                status = enumerator.MoveNext2D();
            }
            builder.Length -= 2;
            builder.Append(" ]\n]");
            return builder.ToString();
        }
    }
}
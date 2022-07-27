using UnityEngine;
using Utils.Array2D;

namespace Model.Map
{
    public class Peak : Valley
    {
        protected override float ExitElevationStart => float.NegativeInfinity;

        public Peak(Vector2Int start, MapUnit[,] mapUnits) : base(start, mapUnits)
        {
        }

        public Peak(Vector2Int start, I2DArray<float> mapUnits) : base(start, mapUnits)
        {
        }

        public Peak(Vector2Int start, MapUnit[,] mapUnits, float elevation) : base(start, mapUnits, elevation)
        {
        }

        public Peak(Vector2Int start, I2DArray<float> mapUnits, float elevation) : base(start, mapUnits, elevation)
        {
        }

        protected override bool ElevationCondition(Vector2Int position, float elevation)
        {
            return MapElevations[position.x, position.y] >= elevation;
        }
    }
}
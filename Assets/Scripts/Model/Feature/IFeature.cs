using UnityEngine;

namespace Model.Map.Feature
{
    public interface IFeature
    {
        public Vector2Int[] GetFeaturePositions();
    }
}
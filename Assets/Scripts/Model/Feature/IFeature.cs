using UnityEngine;

namespace Model.Feature
{
    public interface IFeature
    {
        public Vector2Int[] GetFeaturePositions();
    }
}
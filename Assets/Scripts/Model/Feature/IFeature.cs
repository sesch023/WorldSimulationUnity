using UnityEngine;

namespace Model.Feature
{
    /// <summary>
    /// Interface for a feature that is represented by multiple points on a map.
    /// </summary>
    public interface IFeature
    {
        /// <summary>
        /// Collection of points that make up the feature.
        /// </summary>
        /// <returns>Collection of points that make up the feature.</returns>
        public Vector2Int[] GetFeaturePositions();
    }
}
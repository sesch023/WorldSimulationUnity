using UnityEngine;

namespace Model.Feature
{
    /// <summary>
    /// Interface for a feature that is represented by multiple points on a map and can contain elements by position.
    /// </summary>
    public interface IBody : IFeature
    {
        /// <summary>
        /// Checks if the given point is inside the body.
        /// </summary>
        /// <param name="position">Vector2 to check.</param>
        /// <returns>True, if the point is inside the body.</returns>
        public bool InBody(Vector2Int position);
    }
}
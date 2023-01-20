using UnityEngine;

namespace Model.Map.Feature
{
    public interface IBody : IFeature
    {
        public bool InBody(Vector2Int position);
    }
}
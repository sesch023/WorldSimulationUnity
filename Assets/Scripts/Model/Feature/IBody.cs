using UnityEngine;

namespace Model.Feature
{
    public interface IBody : IFeature
    {
        public bool InBody(Vector2Int position);
    }
}
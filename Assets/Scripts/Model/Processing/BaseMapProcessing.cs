using System;
using UnityEngine;

namespace Model.Processing
{
    /// <summary>
    /// Base class for processing a map.
    /// </summary>
    [Serializable]
    public abstract class BaseMapProcessing : ScriptableObject, IMapProcessing
    {
        /// <summary>
        /// Processes a given map.
        /// </summary>
        /// <param name="map">Map to Process.</param>
        public abstract void ProcessMap(Map.Map map);
    }
}
using System;
using UnityEngine;

namespace Model.Processing
{
    /// <summary>
    /// Base Class for Processing a Map or a Data Array.
    /// </summary>
    [Serializable]
    public abstract class BaseGeneralProcessing : ScriptableObject, IMapProcessing, IDataArrayProcessing
    {
        /// <summary>
        /// Processes a given map.
        /// </summary>
        /// <param name="map">Map to Process.</param>
        public abstract void ProcessMap(Map.Map map);
        
        /// <summary>
        /// Processes a given Map Array.
        /// </summary>
        /// <param name="map">Map Array to process.</param>
        public abstract void ProcessGeneratorData(float[,] map);
    }
}
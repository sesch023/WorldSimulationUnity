using System;
using UnityEngine;

namespace Model.Processing
{
    /// <summary>
    /// Base class for processing a data array.
    /// </summary>
    [Serializable]
    public abstract class BaseGeneratorProcessing : ScriptableObject, IDataArrayProcessing
    {
        /// <summary>
        /// Processes a given Map Array.
        /// </summary>
        /// <param name="map">Map Array to process.</param>
        public abstract void ProcessGeneratorData(float[,] map);
    }
}
using System;
using UnityEngine;
using Utils.BaseUtils;

namespace Model.Generators
{
    /// <summary>
    /// The FlatGenerator is a generator that generates a flat map.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "FlatGenerator", menuName = "ScriptableObjects/FlatGenerator", order = 1)]
    public class FlatGenerator : BaseGenerator
    {
        /// Target height of the map.
        [SerializeField]
        private float elevationLevel = 0;
        
        /// <summary>
        /// Returns a flat map.
        /// </summary>
        /// <param name="sizeX">Width of the Map.</param>
        /// <param name="sizeY">Height of the Map.</param>
        /// <returns>A flat 2D map of elevations.</returns>
        public override (float[,] elevation, float min, float max) GenerateElevation(int sizeX, int sizeY)
        {
            (int sizeX, int sizeY) newSizes = LimitMapSizes(sizeX, sizeY);
            return (Util.GetNew2DArray(newSizes.sizeX, newSizes.sizeY, elevationLevel), 0, elevationLevel);
        }
    }
}
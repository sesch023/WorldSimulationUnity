using Model.Map;
using UnityEngine;
using Utils.Arrays;

namespace Model.Processing
{
    /// <summary>
    /// Eroder Processor Scriptable Object for executing erosion on a map.
    /// 
    /// Based on:
    /// https://github.com/SebLague/Hydraulic-Erosion/blob/master/Assets/Scripts/Erosion.cs
    /// </summary>
    [CreateAssetMenu(fileName = "Hydraulic Eroder", menuName = "ScriptableObjects/Hydraulic Eroder", order = 1)]
    public class HydraulicEroder : BaseGeneralProcessing
    {
        /// Iterations of erosion to perform.
        [SerializeField]
        private int iterations = 100000;
        
        /// Eroder to use.
        [SerializeField]
        private HydraulicErosion erosion;

        /// <summary>
        /// Processes the map with the Hydraulic Eroder and the given numbers of iterations.
        /// </summary>
        /// <param name="map">Map to process.</param>
        public override void ProcessMap(Map.Map map)
        {
            ArrayMap2D<MapUnit, float> mappedMap = new ArrayMap2D<MapUnit, float>(map.MapUnits, unit => unit.Position.Elevation,
                (array, x, y, mapped) => array[x, y].ErodeElevation(mapped));
            I2DArray<float> data = new NormalizedFloatArray2D(mappedMap);
            
            erosion.Erode(data, iterations);
        }
        
        /// <summary>
        /// Processes the array with the Hydraulic Eroder and the given numbers of iterations.
        /// </summary>
        /// <param name="map">Array to process.</param>
        public override void ProcessGeneratorData(float[,] map)
        {
            erosion.Erode(new NormalizedFloatArray2D(map), iterations);
        }
    }
}
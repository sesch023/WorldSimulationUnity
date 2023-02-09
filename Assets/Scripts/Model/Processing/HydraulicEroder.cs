using UnityEngine;
using Utils.Arrays;

namespace Model.Processing
{
    /// <summary>
    /// Auf Basis von:
    /// https://github.com/SebLague/Hydraulic-Erosion/blob/master/Assets/Scripts/Erosion.cs
    /// </summary>
    [CreateAssetMenu(fileName = "Hydraulic Eroder", menuName = "ScriptableObjects/Hydraulic Eroder", order = 1)]
    public class HydraulicEroder : BaseGeneralProcessing
    {
        [SerializeField]
        private int iterations = 100000;
        
        [SerializeField]
        private HydraulicErosion erosion;

        public override void ProcessMap(Map map)
        {
            ArrayMap2D<MapUnit, float> mappedMap = new ArrayMap2D<MapUnit, float>(map.MapUnits, unit => unit.Position.Elevation,
                (array, x, y, mapped) => array[x, y].ErodeElevation(mapped));
            I2DArray<float> data = new NormalizedFloatArray2D(mappedMap);
            
            erosion.Erode(data, iterations);
        }
        
        public override void ProcessGeneratorData(float[,] map)
        {
            erosion.Erode(new NormalizedFloatArray2D(map), iterations);
        }
    }
}
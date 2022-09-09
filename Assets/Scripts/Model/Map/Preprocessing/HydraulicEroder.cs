using UnityEngine;
using Utils.Arrays;

namespace Model.Map.Preprocessing
{
    /// <summary>
    /// Auf Basis von:
    /// https://github.com/SebLague/Hydraulic-Erosion/blob/master/Assets/Scripts/Erosion.cs
    /// </summary>
    [CreateAssetMenu(fileName = "Hydraulic Eroder", menuName = "ScriptableObjects/Hydraulic Eroder", order = 1)]
    public class HydraulicEroder : BasePreprocessMapStep
    {
        [SerializeField]
        private int iterations = 100000;
        
        [SerializeField]
        private HydraulicErosion erosion;

        public override void Preprocess(Map map)
        {
            ArrayMap2D<MapUnit, float> mappedMap = new ArrayMap2D<MapUnit, float>(map.MapUnits, unit => unit.Position.Elevation,
                (array, x, y, mapped) => array[x, y].Position.Elevation = mapped);
            
            erosion.Erode(mappedMap, iterations);
        }
    }
}
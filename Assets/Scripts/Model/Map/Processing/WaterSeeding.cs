using UnityEngine;

namespace Model.Map.Processing
{
    [CreateAssetMenu(fileName = "Water Seeding", menuName = "ScriptableObjects/Water Seeding", order = 2)]
    public class WaterSeeding : BaseGeneralProcessing
    {
        [SerializeField]
        private float waterUnits = 100f;
        
        public override void ProcessMap(Map map)
        {
            map.AddWaterBody(new Vector2Int(100, 100), waterUnits);
        }

        public override void ProcessGeneratorData(float[,] map)
        {
            Debug.LogWarning("Water Seeding not implemented for Generators.");
        }
    }
}
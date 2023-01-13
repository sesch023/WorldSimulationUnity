using System;
using Model.Map.Feature;
using Model.Map.VirtualFeatureSelection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Model.Map.Processing
{
    [CreateAssetMenu(fileName = "Water Seeding", menuName = "ScriptableObjects/Water Seeding", order = 2)]
    public class WaterSeeding : BaseGeneralProcessing
    {
        [SerializeField]
        private float waterUnitsPerBody = 1000f;

        [SerializeField] 
        private int waterBodyPlacementAttempts = 10;
        
        public override void ProcessMap(Map map)
        {
            for(int i = 0; i < waterBodyPlacementAttempts; i++)
            {
                Vector2Int vec = new Vector2Int(Random.Range(0, map.SizeX), Random.Range(0, map.SizeY));
                if(map.GetBodyOfWaterByPosition(new Slope(vec, map.MapUnits).CalculatedSlope[^1]) == null)
                    map.AddWaterBody(vec, waterUnitsPerBody);
            }
            
        }

        public override void ProcessGeneratorData(float[,] map)
        {
            Debug.LogWarning("Water Seeding not implemented for Generators.");
        }
    }
}
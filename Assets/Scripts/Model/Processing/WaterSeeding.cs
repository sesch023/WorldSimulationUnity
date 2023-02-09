using System;
using Manager;
using Model.Feature;
using Model.VirtualFeatureSelection;
using UnityEngine;
using Random = System.Random;

namespace Model.Processing
{
    [CreateAssetMenu(fileName = "Water Seeding", menuName = "ScriptableObjects/Water Seeding", order = 2)]
    public class WaterSeeding : BaseGeneralProcessing
    {
        [SerializeField]
        private float waterUnitsPerBody = 1000f;

        [SerializeField] 
        private int waterBodyPlacementAttempts = 10;

        [SerializeField] 
        private int randomSeed = 0;

        private Random _random;
        
        public override void ProcessMap(Map.Map map)
        {
            _random = new Random(randomSeed);
            
            for(int i = 0; i < waterBodyPlacementAttempts; i++)
            {
                Vector2Int vec = new Vector2Int(_random.Next() % map.SizeX, _random.Next() % map.SizeY);
                if(map.GetBodyOfWaterByPosition(new Slope(vec, map.MapUnits).CalculatedSlope[^1]) == null)
                    map.AddWaterBody(vec, waterUnitsPerBody);
            }
            
        }

        public override void ProcessGeneratorData(float[,] map)
        {
            LoggingManager.GetInstance().LogWarning("Water Seeding not implemented for Generators.");
        }
    }
}
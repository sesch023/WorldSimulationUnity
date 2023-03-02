using System;
using Manager;
using Model.Feature;
using Model.VirtualFeatureSelection;
using UnityEngine;
using Random = System.Random;

namespace Model.Processing
{
    /// <summary>
    /// General Processor for Seeding with Water Bodies.
    /// </summary>
    [CreateAssetMenu(fileName = "Water Seeding", menuName = "ScriptableObjects/Water Seeding", order = 2)]
    public class WaterSeeding : BaseGeneralProcessing
    {
        /// Amount of water per body.
        [SerializeField]
        private float waterUnitsPerBody = 1000f;

        /// Amounts of attempts to place a water bodies.
        [SerializeField] 
        private int waterBodyPlacementAttempts = 10;

        /// Seed of the water bodies.
        [SerializeField] 
        private int randomSeed = 0;

        private Random _random;
        
        /// <summary>
        /// Tries to seed the map with water bodies with the given attempts and seed.
        /// </summary>
        /// <param name="map">Map to seed water bodies on.</param>
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

        /// <summary>
        /// This Algorithm is not executable for simple arrays. Because Unity cannot serialize different base
        /// types in their dependendies, it is not possible to use the IMapProcessing interface only.
        /// </summary>
        /// <param name="map"></param>
        public override void ProcessGeneratorData(float[,] map)
        {
            LoggingManager.GetInstance().LogWarning("Water Seeding not implemented for Generators.");
        }
    }
}
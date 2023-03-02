using System;
using Manager;
using Model.Feature;
using UnityEngine;
using Random = System.Random;

namespace Model.Processing
{
    /// <summary>
    /// General Processor for Seeding with Simple Water Bodies.
    /// </summary>
    [CreateAssetMenu(fileName = "Simple Water Seeding", menuName = "ScriptableObjects/Simple Water Seeding", order = 3)]
    public class SimpleWaterSeeding : BaseGeneralProcessing
    {
        /// Total attempts to seed water.
        [SerializeField] 
        private int waterBodyPlacementAttempts = 100;

        /// Seed of the random number generator.
        [SerializeField] 
        private int randomSeed = 0;

        /// Maximum elevation of the water bodies.
        [SerializeField] 
        private float maxWaterElevation = 10000f;

        private Random _random;
        
        /// <summary>
        /// Tries to seed the map with simple water bodies with the given attempts and seed.
        /// </summary>
        /// <param name="map">Map to seed water bodies on.</param>
        public override void ProcessMap(Map.Map map)
        {
            _random = new Random(randomSeed);
            
            for(int i = 0; i < waterBodyPlacementAttempts; i++)
            {
                int attemptCounter = 0;
                do
                {
                    Vector2Int vec = new Vector2Int(_random.Next() % map.SizeX, _random.Next() % map.SizeY);
                    if(map.MapUnits[vec.x, vec.y].Position.Elevation < maxWaterElevation)
                    {
                        try
                        {
                            new SimpleWaterBody(map, vec);
                            break;
                        }
                        catch(ArgumentException){}
                    }
                    attemptCounter++;
                } while (attemptCounter < 100);
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
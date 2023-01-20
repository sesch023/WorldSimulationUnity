using System;
using Manager;
using Model.Map.Feature;
using UnityEngine;
using Random = System.Random;

namespace Model.Map.Processing
{
    [CreateAssetMenu(fileName = "Simple Water Seeding", menuName = "ScriptableObjects/Simple Water Seeding", order = 3)]
    public class SimpleWaterSeeding : BaseGeneralProcessing
    {
        [SerializeField] 
        private int waterBodyPlacementAttempts = 100;

        [SerializeField] 
        private int randomSeed = 0;

        [SerializeField] 
        private float maxWaterElevation = 10000f;

        private Random _random;
        
        public void OnEnable()
        {
            _random = new Random(randomSeed);
        }
        
        public override void ProcessMap(Map map)
        {
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
                        catch(ArgumentException err){}
                    }
                    attemptCounter++;
                } while (attemptCounter < 100);
            }
        }

        public override void ProcessGeneratorData(float[,] map)
        {
            LoggingManager.GetInstance().LogWarning("Water Seeding not implemented for Generators.");
        }
    }
}
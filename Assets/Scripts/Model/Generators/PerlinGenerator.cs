using System;
using UnityEngine;
using Random = System.Random;

namespace Model.Generators
{
    [Serializable]
    [CreateAssetMenu(fileName = "PerlinGenerator", menuName = "ScriptableObjects/PerlinGenerator", order = 1)]
    public class PerlinGenerator : BaseGenerator
    {
        [SerializeField] 
        private int randomSeed = 0;
        [SerializeField]
        private float scale = 100.0f;
        [SerializeField] 
        private int octaves = 10;
        [SerializeField] 
        private float persistence = 0.5f;
        [SerializeField] 
        private float lacunarity = 1.6f;
        
        private Vector2[] _offsets;
        
        private void OnEnable()
        {
            if (randomSeed == 0)
            {
                randomSeed = (int)(DateTime.Now.Ticks % int.MaxValue);
            }

            Random random = new Random(randomSeed);
            _offsets = new Vector2[octaves];

            for (int i = 0; i < _offsets.GetLength(0); i++)
            {
                _offsets[i] = new Vector2(random.Next() % 10000, random.Next() % 10000);
            }
        }
        
        public override float[,] GenerateElevation(int sizeX, int sizeY)
        {
            (int sizeX, int sizeY) newSizes = LimitMapSizes(sizeX, sizeY);
            sizeX = newSizes.sizeX;
            sizeY = newSizes.sizeY;
            
            float[,] noise = new float[sizeX, sizeY];
            for (var y = 0; y < sizeY; y++) {
                for (var x = 0; x < sizeX; x++) {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseValue = 0;
                    for (int i = 0; i < octaves; i++) {
                        float sampleX = x / scale * frequency + _offsets[i].x;
                        float sampleY = y / scale * frequency + _offsets[i].y;
                        float rawNoise = Mathf.PerlinNoise (sampleX, sampleY);
                        noiseValue += rawNoise * amplitude;
                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }
                    noise[x, y] = noiseValue;
                }
            }

            return NormalizeElevation(noise, minHeight, maxHeight);
        }
    }
}
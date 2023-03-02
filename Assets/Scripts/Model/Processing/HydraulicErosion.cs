using System;
using UnityEngine;
using Utils.Arrays;
using Random = UnityEngine.Random;


namespace Model.Processing
{
    /// <summary>
    /// Particle based hydraulic erosion simulation. Can execute multiple iterations of erosion.
    /// 
    /// Based on:
    /// https://github.com/SebLague/Hydraulic-Erosion/blob/master/Assets/Scripts/Erosion.cs
    /// </summary>
    [Serializable]
    public class HydraulicErosion
    {
        /// Initial droplet water amount.
        [field: SerializeField]
        private float InitialWaterVolume { get; set; }= 1;
        
        /// Intial droplet speed.
        [field: SerializeField]
        private float InitialSpeed { get; set; } = 1;
        
        /// Maximum droplet lifetime.
        [field: SerializeField]
        private int MaxDropletLifetime { get; set; }= 30;
        
        /// Droplet inertia.
        [field: SerializeField, Range (0, 1)]
        private float Inertia { get; set; } = .05f;
        
        /// Multiplier for how much sediment a droplet can carry.
        [field: SerializeField]
        private float SedimentCapacityFactor { get; set; } = 4; 
        
        /// Used to prevent carry capacity getting too close to zero on flatter terrain.
        [field: SerializeField]
        private float MinSedimentCapacity { get; set; } = .01f; 
        
        /// How much sediment is eroded each iteration.
        [field: SerializeField, Range (0, 1)]
        private float ErodeSpeed { get; set; } = .3f;
        
        /// How much sediment is deposited each iteration.
        [field: SerializeField, Range (0, 1)]
        private float DepositSpeed { get; set; } = .3f;
        
        /// How much the particle evaporates each iteration.
        [field: SerializeField, Range (0, 1)]
        private float EvaporateSpeed { get; set; } = .01f;
        
        /// Gravity multiplier.
        [field: SerializeField]
        private float Gravity { get; set; } = 4;
        
        /// Radius of droplet erosion brush.
        [field: SerializeField, Range(2, 8)]
        private int ErosionRadius { get; set; } = 3;
        
        /// Elevation deposition scale.
        [SerializeField]
        private float AlgElevationScale = 10f;

        /// Height range of the terrain.
        [SerializeField] 
        private float HeightRange = 18000f;
        
        private static (int X, int Y)[,][] _erosionBrushIndices;
        private static float[,][] _erosionBrushWeights;
        private static int _currentErosionRadius;
        
        /// <summary>
        /// Struct for storing droplet height and gradient.
        /// </summary>
        private struct HeightAndGradient {
            public float Height;
            public float GradientX;
            public float GradientY;
        }
        
        /// <summary>
        /// Initializes the erosion brush indices and weights.
        /// </summary>
        /// <param name="mapSizeX">Width of the map.</param>
        /// <param name="mapSizeY">Height of the map.</param>
        /// <param name="radius">Radius of the erosion brush.</param>
        private void InitializeBrushIndices (int mapSizeX, int mapSizeY, int radius) {
            _erosionBrushIndices = new (int X, int Y)[mapSizeX, mapSizeY][];
            _erosionBrushWeights = new float[mapSizeX, mapSizeY][];

            int[] xOffsets = new int[radius * radius * 4];
            int[] yOffsets = new int[radius * radius * 4];
            float[] weights = new float[radius * radius * 4];
            float weightSum = 0;
            int addIndex = 0;

            for (int xI = 0; xI < _erosionBrushIndices.GetLength(0); xI++) {
                for (int yK = 0; yK < _erosionBrushIndices.GetLength(1); yK++)
                {
                    if (yK <= radius || yK >= mapSizeY - radius || xI <= radius || xI >= mapSizeX - radius) {
                        weightSum = 0;
                        addIndex = 0;
                        for (int y = -radius; y <= radius; y++) {
                            for (int x = -radius; x <= radius; x++) {
                                float sqrDst = x * x + y * y;
                                if (sqrDst < radius * radius) {
                                    int coordX = xI + x;
                                    int coordY = yK + y;

                                    if (coordX >= 0 && coordX < mapSizeX && coordY >= 0 && coordY < mapSizeY) {
                                        float weight = 1 - Mathf.Sqrt (sqrDst) / radius;
                                        weightSum += weight;
                                        weights[addIndex] = weight;
                                        xOffsets[addIndex] = x;
                                        yOffsets[addIndex] = y;
                                        addIndex++;
                                    }
                                }
                            }
                        }
                    }

                    int numEntries = addIndex;
                    _erosionBrushIndices[xI, yK] = new (int X, int Y)[numEntries];
                    _erosionBrushWeights[xI, yK] = new float[numEntries];

                    for (int j = 0; j < numEntries; j++) {
                        _erosionBrushIndices[xI, yK][j] = (xOffsets[j] + xI, yOffsets[j] + yK);
                        _erosionBrushWeights[xI, yK][j] = weights[j] / weightSum;
                    }
                }
            }
        }
        
        /// <summary>
        /// Erodes by the hydraulic erosion algorithm with the given iteartions.
        /// </summary>
        /// <param name="map">Data array to erode.</param>
        /// <param name="numIterations">Number of erode iterations.</param>
        public void Erode(I2DArray<float> map, int numIterations = 10000)
        {
            // Initialize brush indices if necessary
            if (_erosionBrushIndices == null || _currentErosionRadius != ErosionRadius) {
                InitializeBrushIndices (map.GetLength(0), map.GetLength(1), ErosionRadius);
                _currentErosionRadius = ErosionRadius;
            }
            
            // Erodes starting at a random point.
            for (int i = 0; i < numIterations; i++)
            {
                float xRand = Random.Range(0, map.GetLength(0) - 1);
                float yRand = Random.Range(0, map.GetLength(1) - 1);
                
                ErodeStep(map, xRand, yRand);
            }
        }

        private void ErodeStep(I2DArray<float> map, float posX, float posY)
        {
            float dirX = 0;
            float dirY = 0;
            float speed = InitialSpeed;
            float water = InitialWaterVolume;
            float sediment = 0;

            for (int lifetime = 0; lifetime < MaxDropletLifetime; lifetime++)
            {
                int nodeX = (int) posX;
                int nodeY = (int) posY;
                // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
                float cellOffsetX = posX - nodeX;
                float cellOffsetY = posY - nodeY;

                HeightAndGradient heightAndGradient = CalculateHeightAndGradient(map, posX, posY);

                dirX = (dirX * Inertia - heightAndGradient.GradientX * (1 - Inertia));
                dirY = (dirY * Inertia - heightAndGradient.GradientY * (1 - Inertia));

                float len = Mathf.Sqrt(dirX * dirX + dirY * dirY);
                if (len != 0)
                {
                    dirX /= len;
                    dirY /= len;
                }

                posX += dirX;
                posY += dirY;

                // Stop simulating droplet if it's not moving or has flowed over edge of map
                if ((dirX == 0 && dirY == 0) || posX < 0 || posX >= map.GetLength(0) - 1 || posY < 0 ||
                    posY >= map.GetLength(1) - 1)
                {
                    break;
                }

                // Find the droplet's new height and calculate the deltaHeight
                float newHeight = CalculateHeightAndGradient(map, posX, posY).Height;
                float deltaHeight = newHeight - heightAndGradient.Height;

                // Calculate the droplet's sediment capacity (higher when moving fast down a slope and contains lots of water)
                float sedimentCapacity = Mathf.Max(-deltaHeight * speed * water * SedimentCapacityFactor,
                    MinSedimentCapacity);

                // If carrying more sediment than capacity, or if flowing uphill:
                if (sediment > sedimentCapacity || deltaHeight > 0)
                {
                    // If moving uphill (deltaHeight > 0) try fill up to the current height, otherwise deposit a fraction of the excess sediment
                    float amountToDeposit = (deltaHeight > 0)
                        ? Mathf.Min(deltaHeight, sediment)
                        : (sediment - sedimentCapacity) * DepositSpeed;
                    sediment -= amountToDeposit;

                    // Add the sediment to the four nodes of the current cell using bilinear interpolation
                    // Deposition is not distributed over a radius (like erosion) so that it can fill small pits
                    amountToDeposit = (amountToDeposit / AlgElevationScale) * HeightRange;
                    map[nodeX, nodeY] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY);
                    map[nodeX + 1, nodeY] += amountToDeposit * cellOffsetX * (1 - cellOffsetY);
                    map[nodeX, nodeY + 1] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;
                    map[nodeX + 1, nodeY + 1] += amountToDeposit * cellOffsetX * cellOffsetY;

                }
                else
                {
                    // Erode a fraction of the droplet's current carry capacity.
                    // Clamp the erosion to the change in height so that it doesn't dig a hole in the terrain behind the droplet
                    float amountToErode = Mathf.Min((sedimentCapacity - sediment) * ErodeSpeed, -deltaHeight);

                    // Use erosion brush to erode from all nodes inside the droplet's erosion radius
                    for (int brushPointIndex = 0;
                         brushPointIndex < _erosionBrushIndices[nodeX, nodeY].Length;
                         brushPointIndex++)
                    {
                        (int x, int y) nodeIndex = _erosionBrushIndices[nodeX, nodeY][brushPointIndex];
                        float weighedErodeAmount = amountToErode * _erosionBrushWeights[nodeX, nodeY][brushPointIndex];
                        float deltaSediment =
                            (map[nodeIndex.x, nodeIndex.y] < weighedErodeAmount)
                                ? map[nodeIndex.x, nodeIndex.y]
                                : weighedErodeAmount;
                        map[nodeIndex.x, nodeIndex.y] -= ((deltaSediment / AlgElevationScale) * HeightRange);
                        sediment += deltaSediment;
                    }
                }
                
                // Update droplet's speed and water content
                speed = Mathf.Sqrt (speed * speed + deltaHeight * Gravity);
                water *= (1 - EvaporateSpeed);
            }
        }
        
        private HeightAndGradient CalculateHeightAndGradient (I2DArray<float> map, float posX, float posY)
        {
            int coordX = (int) posX;
            int coordY = (int) posY;

            // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
            float x = posX - coordX;
            float y = posY - coordY;

            // Calculate heights of the four nodes of the droplet's cell
            float heightNw = map[coordX, coordY];
            float heightNe = map[coordX + 1, coordY];
            float heightSw = map[coordX, coordY + 1];
            float heightSe = map[coordX + 1, coordY] + 1;

            // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
            float gradientX = (heightNe - heightNw) * (1 - y) + (heightSe - heightSw) * y;
            float gradientY = (heightSw - heightNw) * (1 - x) + (heightSe - heightNe) * x;

            // Calculate height with bilinear interpolation of the heights of the nodes of the cell
            float height = heightNw * (1 - x) * (1 - y) + heightNe * x * (1 - y) + heightSw * (1 - x) * y + heightSe * x * y;

            return new HeightAndGradient () { Height = height, GradientX = gradientX, GradientY = gradientY };
        }
    }
}
using UnityEngine;

namespace Model.Map.Preprocessing
{
    /// <summary>
    /// https://github.com/SebLague/Hydraulic-Erosion/blob/master/Assets/Scripts/Erosion.cs
    /// </summary>
    [CreateAssetMenu(fileName = "Hydraulic Erosion", menuName = "ScriptableObjects/Hydraulic Erosion", order = 1)]
    public class HydraulicErosion : BasePreprocessMapStep
    {
        [SerializeField]
        private int seed;
        [Range (2, 8)]
        [SerializeField]
        private int erosionRadius = 3;
        [Range (0, 1)]
        [SerializeField]
        private float inertia = .05f; // At zero, water will instantly change direction to flow downhill. At 1, water will never change direction. 
        [SerializeField]
        private float sedimentCapacityFactor = 4; // Multiplier for how much sediment a droplet can carry
        [SerializeField]
        private float minSedimentCapacity = .01f; // Used to prevent carry capacity getting too close to zero on flatter terrain
        [Range (0, 1)]
        [SerializeField]
        private float erodeSpeed = .3f;
        [Range (0, 1)]
        [SerializeField]
        private float depositSpeed = .3f;
        [Range (0, 1)]
        [SerializeField]
        private float evaporateSpeed = .01f;
        [SerializeField]
        private float gravity = 4;
        [SerializeField]
        private int maxDropletLifetime = 30;
        [SerializeField] 
        private int numIterations = 30;

        private float initialWaterVolume = 1;
        private float initialSpeed = 1;

        // Indices and weights of erosion brush precomputed for every node
        private int[][] _erosionBrushIndices;
        private float[][] _erosionBrushWeights;
        private System.Random _prng;

        private int _currentSeed;
        private int _currentErosionRadius;
        private int _currentMapSize;

        public override void Preprocess(Map map)
        {
            Initialize(map.SizeX, map.SizeY, false);
            Erode(map.MapUnits, map.SizeX, map.SizeY, numIterations, false);
        }

        // Initialization creates a System.Random object and precomputes indices and weights of erosion brush
        void Initialize (int mapSizeX, int mapSizeY, bool resetSeed) {
            int mapSize = mapSizeX * mapSizeY;
            
            if (resetSeed || _prng == null || _currentSeed != seed) {
                _prng = new System.Random (seed);
                _currentSeed = seed;
            }

            if (_erosionBrushIndices == null || _currentErosionRadius != erosionRadius || _currentMapSize != mapSize) {
                InitializeBrushIndices (mapSizeX, mapSizeY, erosionRadius);
                _currentErosionRadius = erosionRadius;
                _currentMapSize = mapSize;
            }
        }

        public void Erode (MapUnit[,] map, int mapSizeX, int mapSizeY, int numIts = 1, bool resetSeed = false)
        {
            int mapSize = mapSizeX * mapSizeY;
            Initialize (mapSizeX, mapSizeY, resetSeed);
            
            for (int iteration = 0; iteration < numIts; iteration++) {
                // Create water droplet at random point on map
                float posX = _prng.Next (0, mapSize - 1);
                float posY = _prng.Next (0, mapSize - 1);
                float dirX = 0;
                float dirY = 0;
                float speed = initialSpeed;
                float water = initialWaterVolume;
                float sediment = 0;

                for (int lifetime = 0; lifetime < maxDropletLifetime; lifetime++) {
                    int nodeX = (int) posX;
                    int nodeY = (int) posY;
                    int dropletIndex = nodeY * mapSize + nodeX;
                    // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
                    float cellOffsetX = posX - nodeX;
                    float cellOffsetY = posY - nodeY;

                    // Calculate droplet's height and direction of flow with bilinear interpolation of surrounding heights
                    HeightAndGradient heightAndGradient = CalculateHeightAndGradient (map, mapSize, posX, posY);

                    // Update the droplet's direction and position (move position 1 unit regardless of speed)
                    dirX = (dirX * inertia - heightAndGradient.gradientX * (1 - inertia));
                    dirY = (dirY * inertia - heightAndGradient.gradientY * (1 - inertia));
                    // Normalize direction
                    float len = Mathf.Sqrt (dirX * dirX + dirY * dirY);
                    if (len != 0) {
                        dirX /= len;
                        dirY /= len;
                    }
                    posX += dirX;
                    posY += dirY;

                    // Stop simulating droplet if it's not moving or has flowed over edge of map
                    if ((dirX == 0 && dirY == 0) || posX < 0 || posX >= mapSize - 1 || posY < 0 || posY >= mapSize - 1) {
                        break;
                    }

                    // Find the droplet's new height and calculate the deltaHeight
                    float newHeight = CalculateHeightAndGradient (map, mapSize, posX, posY).height;
                    float deltaHeight = newHeight - heightAndGradient.height;

                    // Calculate the droplet's sediment capacity (higher when moving fast down a slope and contains lots of water)
                    float sedimentCapacity = Mathf.Max (-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);

                    // If carrying more sediment than capacity, or if flowing uphill:
                    if (sediment > sedimentCapacity || deltaHeight > 0) {
                        // If moving uphill (deltaHeight > 0) try fill up to the current height, otherwise deposit a fraction of the excess sediment
                        float amountToDeposit = (deltaHeight > 0) ? Mathf.Min (deltaHeight, sediment) : (sediment - sedimentCapacity) * depositSpeed;
                        sediment -= amountToDeposit;
                        
                        // Add the sediment to the four nodes of the current cell using bilinear interpolation
                        // Deposition is not distributed over a radius (like erosion) so that it can fill small pits
                        map[dropletIndex / mapSizeY, dropletIndex % mapSizeY].Position.Elevation 
                            += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY);
                        map[dropletIndex / mapSizeY, (dropletIndex + 1) % mapSizeY].Position.Elevation
                            += amountToDeposit * cellOffsetX * (1 - cellOffsetY);
                        map[(dropletIndex / mapSizeY) + 1, dropletIndex % mapSizeY].Position.Elevation
                            += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;
                        map[(dropletIndex / mapSizeY) + 1, (dropletIndex + 1) % mapSizeY].Position.Elevation
                            += amountToDeposit * cellOffsetX * cellOffsetY;

                    } else {
                        // Erode a fraction of the droplet's current carry capacity.
                        // Clamp the erosion to the change in height so that it doesn't dig a hole in the terrain behind the droplet
                        float amountToErode = Mathf.Min ((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);

                        // Use erosion brush to erode from all nodes inside the droplet's erosion radius
                        for (int brushPointIndex = 0; brushPointIndex < _erosionBrushIndices[dropletIndex].Length; brushPointIndex++) {
                            int nodeIndex = _erosionBrushIndices[dropletIndex][brushPointIndex];
                            float weighedErodeAmount = amountToErode * _erosionBrushWeights[dropletIndex][brushPointIndex];
                            float deltaSediment = (map[nodeIndex / mapSizeY, nodeIndex % mapSizeY].Position.Elevation
                                                   < weighedErodeAmount) 
                                ? map[nodeIndex / mapSizeY, nodeIndex % mapSizeY].Position.Elevation : weighedErodeAmount;
                            map[nodeIndex / mapSizeY, nodeIndex % mapSizeY].Position.Elevation -= deltaSediment;
                            sediment += deltaSediment;
                        }
                    }

                    // Update droplet's speed and water content
                    speed = Mathf.Sqrt (speed * speed + deltaHeight * gravity);
                    water *= (1 - evaporateSpeed);
                }
            }
        }

        HeightAndGradient CalculateHeightAndGradient (MapUnit[,] nodes, int mapSize, float posX, float posY) {
            int coordX = (int) posX;
            int coordY = (int) posY;

            // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
            float x = posX - coordX;
            float y = posY - coordY;

            // Calculate heights of the four nodes of the droplet's cell
            float heightNW = nodes[coordX, coordY].Position.Elevation;
            float heightNE = nodes[coordX + 1, coordY].Position.Elevation;
            float heightSW = nodes[coordX, coordY + 1].Position.Elevation;
            float heightSE = nodes[coordX + 1, coordY + 1].Position.Elevation;

            // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
            float gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
            float gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;

            // Calculate height with bilinear interpolation of the heights of the nodes of the cell
            float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

            return new HeightAndGradient () { height = height, gradientX = gradientX, gradientY = gradientY };
        }

        void InitializeBrushIndices (int mapSizeX, int mapSizeY, int radius) {
            _erosionBrushIndices = new int[mapSizeX * mapSizeY][];
            _erosionBrushWeights = new float[mapSizeX * mapSizeY][];

            int[] xOffsets = new int[radius * radius * 4];
            int[] yOffsets = new int[radius * radius * 4];
            float[] weights = new float[radius * radius * 4];
            float weightSum = 0;
            int addIndex = 0;

            for (int i = 0; i < _erosionBrushIndices.GetLength (0); i++) {
                int centreX = i % mapSizeY;
                int centreY = i / mapSizeY;

                if (centreY <= radius || centreY >= mapSizeY - radius || centreX <= radius + 1 || centreX >= mapSizeX - radius) {
                    weightSum = 0;
                    addIndex = 0;
                    for (int y = -radius; y <= radius; y++) {
                        for (int x = -radius; x <= radius; x++) {
                            float sqrDst = x * x + y * y;
                            if (sqrDst < radius * radius) {
                                int coordX = centreX + x;
                                int coordY = centreY + y;

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
                _erosionBrushIndices[i] = new int[numEntries];
                _erosionBrushWeights[i] = new float[numEntries];

                for (int j = 0; j < numEntries; j++) {
                    _erosionBrushIndices[i][j] = (yOffsets[j] + centreY) * mapSizeY + xOffsets[j] + centreX;
                    _erosionBrushWeights[i][j] = weights[j] / weightSum;
                }
            }
        }

        struct HeightAndGradient {
            public float height;
            public float gradientX;
            public float gradientY;
        }
    }
}
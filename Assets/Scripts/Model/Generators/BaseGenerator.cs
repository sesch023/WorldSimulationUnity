using System;
using UnityEngine;
using Utils.BaseUtils;

namespace Model.Generators
{
    [Serializable]
    public abstract class BaseGenerator : ScriptableObject, IGenerator
    {
        [SerializeField]
        protected float maxHeight = 10000;
        [SerializeField]
        protected float minHeight = -8000;
        
        public abstract float[,] GenerateElevation(int sizeX, int sizeY);

        public virtual (int sizeX, int sizeY) LimitMapSizes(int sizeX, int sizeY)
        {
            return (sizeX, sizeY);
        }

        protected static float[,] NormalizeElevation(float[,] elevation, float min, float max)
        {
            float minGenerated = Util.MinIn2DArray(elevation);
            float maxGenerated = Util.MaxIn2DArray(elevation);
            
            float range = max - min;

            for (int x = 0; x < elevation.GetLength(0); x++)
            {
                for (int y = 0; y < elevation.GetLength(1); y++)
                {
                    elevation[x, y] = ((elevation[x, y] - minGenerated) / (maxGenerated - minGenerated)) * range + min;
                }
            }

            return elevation;
        }
    }
}
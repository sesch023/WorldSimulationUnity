using System;
using System.Linq;
using Model.Map.Preprocessing;
using Unity.VisualScripting;
using UnityEngine;
using Utils.BaseUtils;

namespace Model.Generators
{
    /// <summary>
    /// Abstract base class for all generators. Implements the elevation normalization and a default implementation of the
    /// LimitMapSizes method, which is used to limit the size of the generated map depending on the generator's algorithm
    /// and the given parameters.
    /// </summary>
    [Serializable]
    public abstract class BaseGenerator : ScriptableObject, IGenerator
    {

        [SerializeField] 
        protected float maxHeight = 10000;

        [SerializeField] 
        protected float minHeight = -8000;
        
        /// Max elevation of the map
        protected float MaxHeightValue;
        /// Min elevation of the map.
        protected float MinHeightValue;

        public void OnValidate()
        {
            MaxHeightValue = maxHeight;
            MinHeightValue = minHeight;
        }
        
        
        /// Preprocess the map, after it is generated with these rules.
        [SerializeReference] 
        private BasePreprocessMapStep[] preprocessMapSteps;
        
        /// <summary>
        /// Abstract method for generating the map as a 2D arrayImmutable of floats.
        /// </summary>
        /// <param name="sizeX">Width of the Map.</param>
        /// <param name="sizeY">Height of the Map.</param>
        /// <returns>2D Array of floats as generated elevation.</returns>
        public abstract (float[,] elevation, float min, float max) GenerateElevation(int sizeX, int sizeY);

        /// <summary>
        /// Limits the size of the generated map depending on the generator's algorithm
        /// and the given parameters. The default implementation just returns the given size.
        /// </summary>
        /// <param name="sizeX">Desired width of the map.</param>
        /// <param name="sizeY">Desired height of the map.</param>
        /// <returns>Agreed on size of the map which takes the desired sizes in mind.</returns>
        public virtual (int sizeX, int sizeY) LimitMapSizes(int sizeX, int sizeY)
        {
            return (sizeX, sizeY);
        }

        protected float[,] ApplyPreprocess(float[,] elevation)
        {
            foreach(var step in preprocessMapSteps)
            {
                step.Preprocess(elevation);
            }

            return elevation;
        }
        
        protected float[,] FinishGeneration(float[,] elevation)
        {
            elevation = NormalizeElevation(elevation, 0, 1);
            elevation = ApplyPreprocess(elevation);
            float newMax = Util.MaxIn2DArray(elevation) * MaxHeightValue; 
            float newMin = (1.0f - Util.MinIn2DArray(elevation)) * MinHeightValue; 
            
            Debug.Log(newMax);
            Debug.Log(MaxHeightValue);
            Debug.Log(elevation.Cast<float>().Max());
            
            Debug.Log(newMin);
            Debug.Log(MinHeightValue);
            Debug.Log(elevation.Cast<float>().Min());
			
            if(newMax > MaxHeightValue)
                MaxHeightValue = newMax;
            if(newMin < MinHeightValue)
                MinHeightValue = newMin;

            return NormalizeElevation(elevation, MinHeightValue, MaxHeightValue);
        }
        
        /// <summary>
        /// Normalizes the elevation of the map to the given min and max values.
        /// </summary>
        /// <param name="elevation">2D arrayImmutable of floats as elevation.</param>
        /// <param name="min">Minimum new value after normalization.</param>
        /// <param name="max">Maximum new value after normalization.</param>
        /// <returns>Normalized 2D arrayImmutable of floats.</returns>
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
using System;
using UnityEngine;

namespace Model.Map.Processing
{
    [Serializable]
    public abstract class BaseGeneratorProcessing : ScriptableObject, IDataArrayProcessing
    {
        public void ProcessGeneratorData(float[,] map)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using UnityEngine;

namespace Model.Map.Processing
{
    [Serializable]
    public abstract class BaseGeneratorProcessing : ScriptableObject, IDataArrayProcessing
    {
        public abstract void ProcessGeneratorData(float[,] map);
    }
}
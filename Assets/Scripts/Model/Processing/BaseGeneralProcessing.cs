using System;
using UnityEngine;

namespace Model.Processing
{
    [Serializable]
    public abstract class BaseGeneralProcessing : ScriptableObject, IMapProcessing, IDataArrayProcessing
    {
        public abstract void ProcessMap(Map.Map map);
        public abstract void ProcessGeneratorData(float[,] map);
    }
}
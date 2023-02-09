using System;
using UnityEngine;

namespace Model.Map.Processing
{
    [Serializable]
    public abstract class BaseGeneralProcessing : ScriptableObject, IMapProcessing, IDataArrayProcessing
    {
        public abstract void ProcessMap(Map map);
        public abstract void ProcessGeneratorData(float[,] map);
    }
}
using System;
using UnityEngine;

namespace Model.Map.Processing
{
    [Serializable]
    public abstract class BaseMapProcessing : ScriptableObject, IMapProcessing
    {
        public abstract void ProcessMap(Map map);
    }
}
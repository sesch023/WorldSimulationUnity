using System;
using UnityEngine;

namespace Model.Processing
{
    [Serializable]
    public abstract class BaseMapProcessing : ScriptableObject, IMapProcessing
    {
        public abstract void ProcessMap(Map map);
    }
}
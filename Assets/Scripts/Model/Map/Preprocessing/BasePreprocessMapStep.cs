using System;
using UnityEngine;

namespace Model.Map.Preprocessing
{
    [Serializable]
    public abstract class BasePreprocessMapStep : ScriptableObject, IPreprocessMapStep
    {
        public abstract void Preprocess(Map map);
    }
}
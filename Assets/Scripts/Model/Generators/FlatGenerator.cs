using System;
using UnityEngine;
using Utils;

namespace Model.Generators
{
    [Serializable]
    [CreateAssetMenu(fileName = "FlatGenerator", menuName = "ScriptableObjects/FlatGenerator", order = 1)]
    public class FlatGenerator : BaseGenerator
    {
        [SerializeField]
        private float elevationLevel = 0;
        
        public override float[,] GenerateElevation(int sizeX, int sizeY)
        {
            return Util.GetNew2DArray(sizeX, sizeY, elevationLevel);
        }
    }
}
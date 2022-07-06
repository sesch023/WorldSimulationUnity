using System;
using Base;
using UnityEngine;

namespace Model
{
    [Serializable]
    public class Map : IUpdatable
    {
        private Map(){}

        [SerializeField]
        private uint _sizeX = 2000;
        [SerializeField]
        private uint _sizeY = 1000;

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
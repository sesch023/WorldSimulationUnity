using System.Collections;
using System.Collections.Generic;
using Base;
using Model;
using UnityEngine;

namespace Manager
{
    public sealed class SimulationManager : MonoBehaviour
    {
        public Camera MainCamera { get; private set; }
        public static SimulationManager Instance { get; private set; }
        
        private SimulationManager(){}
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Init()
        {
            MainCamera = Camera.main;
            if (MainCamera == null)
            {
                throw new MissingReferenceException(
                    "MissingReferenceException: Reference on MainCamera in SimulationManager missing!");
            }
        }
    }
}

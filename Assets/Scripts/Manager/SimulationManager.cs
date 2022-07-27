using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Model;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Manager
{
    public sealed class SimulationManager : MonoBehaviour
    {
        public enum InteractionMode
        {
            SelectTile,
            ValleySelection,
            SlopeSelection
        } 
        
        public Camera MainCamera { get; private set; }

        public InteractionMode CurrentInteractionMode { get; set; } = InteractionMode.SelectTile;

        public bool PointerOverUI { get; private set; } = false;
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

        private void Update()
        {
            PointerOverUI = EventSystem.current.IsPointerOverGameObject();
        }
    }
}

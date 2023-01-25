using Base;
using Controllers;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// MapManager Singleton for controlling functions related to the map and its elements.
    /// </summary>
    public sealed class MapManager : MonoBehaviour
    {
        /// Instance of the MapManager.
        public static MapManager Instance { get; private set; }

        /// Instance of the MapController.
        [field: SerializeField]
        public TileMapController MapController { get; private set; }
        
        private MapManager(){}
        
        /// <summary>
        /// Enables the singleton with the current instance or destroys the game object if it already exists.
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initializes the MapManager.
        /// </summary>
        void Start()
        {
            MapController.Init();
            UpdateManager.Instance.RegisterUpdatable(MapController);
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

namespace Manager
{
    /// <summary>
    /// Singleton Manager class for handling interactions with the simulation.
    /// </summary>
    public sealed class SimulationManager : MonoBehaviour
    {
        /// <summary>
        /// Interaction Modes for clicks within the simulation.
        /// </summary>
        public enum InteractionMode
        {
            SelectTile,
            GroupSelection,
            SlopeSelection
        } 
        
        /// Main Camera for the simulation.
        public Camera MainCamera { get; private set; }
        
        /// Current Mode of interaction with the simulation.
        public InteractionMode CurrentInteractionMode { get; set; } = InteractionMode.SelectTile;

        // Is the Mouse currently over a UI element?
        public bool PointerOverUI { get; private set; } = false;
        
        /// Instance of the SimulationManager.
        public static SimulationManager Instance { get; private set; }
        
        private SimulationManager(){}
        
        /// <summary>
        /// Enables the singleton with the current instance or destroys the game object if it already exists.
        /// </summary>
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
        
        /// <summary>
        /// Initializes the SimulationManager. Requires the MainCamera to be set.
        /// </summary>
        /// <exception cref="MissingReferenceException">If the MainCamera is not set.</exception>
        private void Init()
        {
            MainCamera = Camera.main;
            if (MainCamera == null)
            {
                throw new MissingReferenceException($"MissingReferenceException: {GetType()} - MainCamera is not set.");
            }
        }
        
        /// <summary>
        /// Updatefunction for the SimulationManager. Checks if the mouse is over a UI element.
        /// </summary>
        private void Update()
        {
            PointerOverUI = EventSystem.current.IsPointerOverGameObject();
        }
    }
}

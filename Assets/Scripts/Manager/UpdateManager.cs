using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// Update Manager Singleton of the Simulation. It deals with every Updatable in the Simulation that is
    /// not dealt with by the unity engine. It also manages the removal of single time events.
    /// </summary>
    public class UpdateManager : MonoBehaviour
    {
        /// All the updatables in the simulation.
        private HashSet<IUpdatable> _updatables;
        /// All Updateables that are to be removed.
        protected Queue<IUpdatable> MarkedForRemoval;
        /// Instance of the UpdateManager.
        public static UpdateManager Instance { get; private set; }

        protected UpdateManager(){}
        
        /// <summary>
        /// Enables the singleton with the current instance or destroys the game object if it already exists.
        /// </summary>
        void Awake()
        {
            if (Instance == null)
            {
                _updatables = new HashSet<IUpdatable>();
                MarkedForRemoval = new Queue<IUpdatable>();
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initializes the UpdateManager.
        /// </summary>
        private void Start()
        {
            InitUpdatables();
        }
        
        /// <summary>
        /// Initializes the updatables that are statically coded. In the current implementation, this method
        /// only initializes test updatables.
        /// </summary>
        private void InitUpdatables()
        {
            /*
            UpdatableEvent action = new KeyboardAction(Keys.Enter, ActionType.KeyDown, 
                triggeredBy => { LoggingManager.Instance.Info("Hello Enter!"); });
                */
            
            var spacedEvent = new TimeSpacedEvent(5000,
                triggeredBy =>
                    LoggingManager.GetInstance().LogInfo("This Message should be shown every 5000ms!"));

            var timeEvent = new TimeEvent(40000,
                triggeredBy =>  {
                    LoggingManager.GetInstance().LogInfo("This Message should be shown after 40000ms!");
                    for (int x = 0; x < 0.1 * MapManager.Instance.MapController.UnitMap.MapUnits.GetLength(0); x++)
                    {
                        for (int y = 0; y < 0.1 * MapManager.Instance.MapController.UnitMap.MapUnits.GetLength(1); y++)
                        {
                            MapManager.Instance.MapController.UnitMap.MapUnits[x, y].Position.Elevation = 0;
                            MapManager.Instance.MapController.UnitMap.MapUnits[x, y].Temperature = 10;
                        }
                    }
                },
                instance =>
                {
                    MarkRemovableForRemoval(instance);
                    LoggingManager.GetInstance().LogInfo("Bye Time Event!");
                });
            
            var tickEvent = new TickEvent(10,
                triggeredBy =>
                    LoggingManager.GetInstance().LogInfo("This Message should be shown once after 10 ticks!"),
                instance =>
                {
                    MarkRemovableForRemoval(instance);
                    LoggingManager.GetInstance().LogInfo("Bye Tick Event!");
                });
        }
        
        /// <summary>
        /// Update method of the UpdateManager. It updates all updatables and removes the updatables that are marked for removal
        /// if the queue is not empty.
        /// </summary>
        void Update()
        {
            foreach (var variableUpdatable in _updatables) 
                variableUpdatable.Update();
            if (MarkedForRemoval.Count > 0)
            {
                RemovalRun();
            }
        }

        /// <summary>
        /// Removes the updatables that are marked for removal.
        /// </summary>
        protected virtual void RemovalRun()
        {
            foreach (var updatable in MarkedForRemoval)
            {
                RemoveUpdatable(updatable);
            }

            MarkedForRemoval.Clear();
        }

        /// <summary>
        /// Registers a new updatable to the UpdateManager.
        /// </summary>
        /// <param name="updatable">New updatable to be registered.</param>
        public void RegisterUpdatable(IUpdatable updatable)
        {
            Debug.Log(updatable);
            _updatables.Add(updatable);
        }

        /// <summary>
        /// Removes an updatable from the UpdateManager by address.
        /// </summary>
        /// <param name="updatable">Updatable to be removed.</param>
        public void RemoveUpdatable(IUpdatable updatable)
        {
            _updatables.Remove(updatable);
        }
        
        /// <summary>
        /// Marks an updatable for removal.
        /// </summary>
        /// <param name="updatable">Updatable to be marked for removal.</param>
        /// <exception cref="ArgumentException">Thrown if the updatable is not registered.</exception>
        public void MarkUpdatableForRemoval(IUpdatable updatable)
        {
            if(_updatables.Contains(updatable))
                MarkedForRemoval.Enqueue(updatable);
            else
                throw new ArgumentException($"ArgumentException: {GetType()}::MarkUpdatableForRemoval - Updatable {updatable} is not registered!");
        }
        
        /// <summary>
        /// Marks an removable updatable for removal.
        /// </summary>
        /// <param name="managedRemoval">Removable Updatable to be marked for removal. Can only be executed if the IManagedRemoval is an IUpdatable.</param>
        /// <exception cref="ArgumentException">The given IManagedRemoval is not an IUpdatable.</exception>
        public void MarkRemovableForRemoval(IManagedRemoval managedRemoval)
        {
            if(managedRemoval is IUpdatable removal)
                MarkUpdatableForRemoval(removal);
            else
                throw new ArgumentException($"ArgumentException: {GetType()}::MarkRemovableForRemoval - Removable is not an updatable!");
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;

namespace Manager
{
    public sealed class UpdateManager : MonoBehaviour
    {
        private List<IUpdatable> _updatables;
        private Queue<IUpdatable> _markedForRemoval;
        public static UpdateManager Instance { get; private set; }

        private UpdateManager(){}
        
        void Awake()
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

        private void Start()
        {
            Init();
        }
        
        private void Init()
        {
            _updatables = new List<IUpdatable>();
            _markedForRemoval = new Queue<IUpdatable>();
            InitUpdatables();
        }
        
        private void InitUpdatables()
        {
            /*
            UpdatableEvent action = new KeyboardAction(Keys.Enter, ActionType.KeyDown, 
                triggeredBy => { LoggingManager.Instance.Info("Hello Enter!"); });
                */
            Debug.Log("Hello!");
            
            var spacedEvent = new TimeSpacedEvent(5000,
                triggeredBy =>
                    Debug.Log("This Message should be shown every 5000ms!"));

            var timeEvent = new TimeEvent(5000,
                triggeredBy =>
                    Debug.Log("This Message should be shown once at 5s!"),
                instance =>
                {
                    MarkRemovableForRemoval(instance);
                    Debug.Log("Bye Time Event!");
                });
            
            var tickEvent = new TickEvent(10,
                triggeredBy =>
                    Debug.Log("This Message should be shown once after 10 ticks!"),
                instance =>
                {
                    MarkRemovableForRemoval(instance);
                    Debug.Log("Bye Tick Event!");
                });
        }
        
        void Update()
        {
            foreach (var variableUpdatable in _updatables.ToList()) variableUpdatable.Update();
            RemovalRun();
        }

        private void RemovalRun()
        {
            foreach(var updatable in _markedForRemoval)
            {
                Debug.Log("Clearing Updatable: " + updatable);
                RemoveUpdatable(updatable);
            }
            
            _markedForRemoval.Clear();
        }

        public void RegisterUpdatable(IUpdatable updatable)
        {
            _updatables.Add(updatable);
        }

        public void RemoveUpdatable(IUpdatable updatable)
        {
            _updatables.Remove(updatable);
        }

        public void MarkUpdatableForRemoval(IUpdatable updatable)
        {
            _markedForRemoval.Enqueue(updatable);
        }

        public void MarkRemovableForRemoval(IManagedRemoval managedRemoval)
        {
            if(managedRemoval is IUpdatable removal)
                MarkUpdatableForRemoval(removal);
        }
    }
}
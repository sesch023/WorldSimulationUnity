using System.Collections;
using System.Collections.Generic;
using Base;
using Model;
using UnityEngine;

namespace Manager
{
    public sealed class SimulationManager : MonoBehaviour
    {
        public static SimulationManager Instance { get; private set; }
        
        private static IList<IUpdatable> _updatables;

        [SerializeField]
        private Map _map;
        
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
            _updatables = new List<IUpdatable>();
        }

        public void RegisterUpdatable(IUpdatable updatable)
        {
            _updatables.Add(updatable);
        }

        public void UnregisterUpdatable(IUpdatable updatable)
        {
            _updatables.Remove(updatable);
        }

        public void ClearUpdatables()
        {
            _updatables.Clear();
        }
        
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            foreach(IUpdatable updatable in _updatables)
            {
                updatable.Update();
            }
        }
    }
}

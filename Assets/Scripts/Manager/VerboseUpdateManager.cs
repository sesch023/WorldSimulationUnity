using UnityEngine;

namespace Manager
{
    public sealed class VerboseUpdateManager : UpdateManager
    {
        [field: SerializeField]
        public bool EnableDebug { get; set; } = false;

        protected override void RemovalRun()
        {
            foreach(var updatable in MarkedForRemoval)
            {
                if(EnableDebug)
                    Debug.Log("Clearing Updatable: " + updatable);
                RemoveUpdatable(updatable);
            }
            
            MarkedForRemoval.Clear();
        }
    }
}
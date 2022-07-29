using UnityEngine;

namespace Manager
{
    /// <summary>
    /// Verbose Version of the UpdateManager, which logs the clearing of for removal marked updatables.
    /// </summary>
    public sealed class VerboseUpdateManager : UpdateManager
    {
        /// If the debug log is enabled, it enables logging of the clearing of for removal marked updatables.
        [field: SerializeField]
        public bool EnableDebug { get; set; } = false;

        /// <summary>
        /// Removes the updatables that are marked for removal. If the debug log is enabled, it logs the clearing
        /// of for removal marked updatables.
        /// </summary>
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
using Controllers;
using Model;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Manager
{
    public sealed class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        [field: SerializeField]
        public TileMapController MapController { get; private set; }

        private MapManager(){}
        
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
        
        // Start is called before the first frame update
        void Start()
        {
            MapController.Init();
        }

        // Update is called once per frame
        void Update()
        {
            MapController.Update();
        }
    }
}

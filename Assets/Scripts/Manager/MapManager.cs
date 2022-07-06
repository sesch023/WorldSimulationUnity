using ScriptableObjects;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Manager
{
    public sealed class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        [SerializeField]
        private MapTiles tiles;
        [SerializeField]
        private Tilemap tileMap;
        
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
            tileMap.SetTile(new(1, 1, 0), tiles.TestTile);
            tileMap.SetTile(new(1, 2, 0), tiles.TestTile);
            Debug.Log(tiles.TestTile);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

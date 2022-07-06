using Model;
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

        [field: SerializeField] public Tilemap TileMap { get; private set; }

        [SerializeField] 
        private Map map;

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
            /*
            tileMap.SetTile(new(1, 1, 0), tiles.TestTile);
            tileMap.SetTile(new(1, 2, 0), tiles.TestTile);
            */
            for (var x = 0; x < map.SizeX; x++)
            {
                for (var y = 0; y < map.SizeY; y++)
                {
                    TileMap.SetTile(new(x, y, 0), tiles.TestTile);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            map.Update();
        }
    }
}

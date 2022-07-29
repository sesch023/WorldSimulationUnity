using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Views.GameViews
{
    public class TileGroupView : MonoBehaviour
    {
        public Vector2Int[] tiles;
        public GameObject tileHighlight;

        private List<GameObject> _childs = new();
        
        public void Awake()
        {
            if (tileHighlight == null)
            {
                throw new MissingReferenceException(
                    $"MissingReferenceException: {GetType()} - Missing Tile Highlight!");
            }
            
            tileHighlight.SetActive(false);
        }

        public void Start()
        {
            Enable();
        }

        public void Enable()
        {
            Disable();
            Vector3 tileScale = MapManager.Instance.MapController.TileMap.transform.localScale;
            tileHighlight.transform.localScale = tileScale;
            
            if (tiles is {Length: > 0})
            {
                foreach (Vector2Int tile in tiles)
                {
                    Vector3 pos = new Vector3(tileScale.x / 2, tileScale.y / 2, 0) +
                                  MapManager.Instance.MapController.TileMapPositionToGlobalPosition(tile);
                    GameObject go = Instantiate(tileHighlight, pos, Quaternion.identity,transform);
                    go.SetActive(true);
                    _childs.Add(go);
                }
            }
        }
        
        public void OnDisable()
        {
            Disable();
        }

        public void Disable()
        {
            foreach (var go in _childs)
            {
                Destroy(go);
            }
            _childs.Clear();
        }
    }
}
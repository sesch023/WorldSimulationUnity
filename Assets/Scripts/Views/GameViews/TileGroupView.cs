using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Views.GameViews
{
    /// <summary>
    /// View of a group of tiles.
    /// </summary>
    public class TileGroupView : MonoBehaviour
    {
        /// Tiles of the group.
        [field: SerializeField]
        public Vector2Int[] Tiles { get; set; }
        
        /// Highlight for a tile.
        [SerializeField]
        private GameObject tileHighlight;

        /// Highlight children.
        private readonly List<GameObject> _children = new();
        
        /// <summary>
        /// Initialize the view.
        /// </summary>
        /// <exception cref="MissingReferenceException">If the highlight is not set.</exception>
        public void Awake()
        {
            if (tileHighlight == null)
            {
                throw new MissingReferenceException(
                    $"MissingReferenceException: {GetType()} - Missing Tile Highlight!");
            }
            
            tileHighlight.SetActive(false);
        }

        /// <summary>
        /// Start the view and enables the group view, if the group is not empty.
        /// </summary>
        public void Start()
        {
            Enable();
        }
        
        /// <summary>
        /// Enables the group view, if the group is not empty.
        /// </summary>
        public void Enable()
        {
            Disable();
            Vector3 tileScale = MapManager.Instance.MapController.TileMap.transform.localScale;
            tileHighlight.transform.localScale = tileScale;
            
            if (Tiles is {Length: > 0})
            {
                foreach (Vector2Int tile in Tiles)
                {
                    Vector3 pos = new Vector3(tileScale.x / 2, tileScale.y / 2, 0) +
                                  MapManager.Instance.MapController.TileMapPositionToGlobalPosition(tile);
                    GameObject go = Instantiate(tileHighlight, pos, Quaternion.identity,transform);
                    go.SetActive(true);
                    _children.Add(go);
                }
            }
        }
        
        /// <summary>
        /// If the group is disabled, remove all children.
        /// </summary>
        public void OnDisable()
        {
            Disable();
        }

        /// <summary>
        /// If the group is disabled, remove all children.
        /// </summary>
        public void Disable()
        {
            foreach (var go in _children)
            {
                Destroy(go);
            }
            _children.Clear();
        }
    }
}
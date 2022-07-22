using Controllers;
using Manager;
using UnityEngine;

namespace Views.GameViews
{
    public class TileLineView : MapLineView
    {
        public void SetTileLinePoints(Vector2Int[] points)
        {
            DisableLine();
            Vector3 scale = MapManager.Instance.MapController.TileMap.transform.localScale;
            Points = new Vector2[points.Length + 1];
            Points[0] = new Vector2(scale.x / 2, scale.y / 2) +
                        (Vector2)MapManager.Instance.MapController.TileMapPositionToGlobalPosition(points[0]);
            for(int i = 0; i < points.Length; i++)
            {
                Points[i + 1] = new Vector2(scale.x / 2, scale.y / 2) + (Vector2)MapManager.Instance.MapController.TileMapPositionToGlobalPosition(points[i]);
            }
                
            DoLinePositionChange();
            EnableLine();
        }
    }
}
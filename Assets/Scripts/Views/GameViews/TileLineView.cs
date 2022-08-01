using Manager;
using UnityEngine;
using Utils.BaseUtils;

namespace Views.GameViews
{
    /// <summary>
    /// LineView for displaying a line between tile coordinates.
    /// </summary>
    public class TileLineView : LineView
    {
        /// <summary>
        /// Set the coordinates of the line.
        /// </summary>
        /// <param name="points">Integer Coordinates of the line.</param>
        public void SetTileLinePoints(Vector2Int[] points)
        {
            SetTileLinePoints(MathUtil.Vector2IntArrayToVector2Array(points));
        }
        
        /// <summary>
        /// Sets the coordinates of the line from tile coordinates to world coordinates.
        /// </summary>
        /// <param name="points">Points to set.</param>
        public void SetTileLinePoints(Vector2[] points)
        {
            DisableLine();
            Vector3 scale = MapManager.Instance.MapController.TileMap.transform.localScale;
            Points = new Vector2[points.Length + 1];
            Points[0] = new Vector2(scale.x / 2, scale.y / 2) +
                        (Vector2)MapManager.Instance.MapController.TileMapPositionToGlobalPosition(points[0]);
            for(int i = 0; i < points.Length; i++)
            {
                Points[i + 1] = new Vector2(scale.x / 2, scale.y / 2) 
                                + (Vector2)MapManager.Instance.MapController.TileMapPositionToGlobalPosition(points[i]);
            }
                
            DoLinePositionChange();
            EnableLine();
        }
    }
}
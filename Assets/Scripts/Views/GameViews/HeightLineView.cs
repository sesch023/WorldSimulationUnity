using UnityEngine;

namespace Views.GameViews
{
    public class HeightLineView : MapLineView
    {
        public void SetHeightLinePoints(Vector2Int[] points)
        {
            DisableLine();
            Points = new Vector2[points.Length];
            for(int i = 0; i < points.Length; i++)
            {
                Points[i].Set(points[i].x + 0.5f, points[i].y + 0.5f);
            }
                
            DoLinePositionChange();
            EnableLine();
        }
    }
}
﻿using System.Collections.Generic;
using UnityEngine;

namespace Utils.BaseUtils
{
    /// <summary>
    /// Class of calculating a convex hull of a list of points.
    /// </summary>
    public static class ConvexHull
    {
        /// <summary>
        /// Calculates a convex hull of a list of points.
        ///
        /// https://gist.github.com/dLopreiato/7fd142d0b9728518552188794b8a750c
        /// </summary>
        /// <param name="points">Points to build a convex hull around.</param>
        /// <param name="sortInPlace">Should be sorted in place?</param>
        /// <returns>Convex hull of the given points.</returns>
        public static IList<Vector2> ComputeConvexHull(List<Vector2> points, bool sortInPlace = false)
        {
            if (!sortInPlace)
                points = new List<Vector2>(points);
            points.Sort((a, b) =>
                a.x == b.x ? a.y.CompareTo(b.y) : (a.x > b.x ? 1 : -1));

            // Importantly, DList provides O(1) insertion at beginning and end
            CircularList<Vector2> hull = new CircularList<Vector2>();
            int L = 0, U = 0; // size of lower and upper hulls

            // Builds a hull such that the output polygon starts at the leftmost Vector2.
            for (int i = points.Count - 1; i >= 0; i--)
            {
                Vector2 p = points[i], p1;

                // build lower hull (at end of output list)
                while (L >= 2 && (p1 = hull.Last).Sub(hull[hull.Count - 2]).Cross(p.Sub(p1)) >= 0)
                {
                    hull.PopLast();
                    L--;
                }
                hull.PushLast(p);
                L++;

                // build upper hull (at beginning of output list)
                while (U >= 2 && (p1 = hull.First).Sub(hull[1]).Cross(p.Sub(p1)) <= 0)
                {
                    hull.PopFirst();
                    U--;
                }
                if (U != 0) // when U=0, share the Vector2 added above
                    hull.PushFirst(p);
                U++;
                Debug.Assert(U + L == hull.Count + 1);
            }
            hull.PopLast();
            return hull;
        }

        private static Vector2 Sub(this Vector2 a, Vector2 b)
        {
            return a - b;
        }

        private static float Cross(this Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
    }
}
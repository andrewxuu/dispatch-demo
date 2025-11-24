using UnityEngine;
using System.Collections.Generic;

public static class RadarMath
{
    public static Vector2[] CalculateVertices(int[] stats, float maxRadius)
    {
        Vector2[] points = new Vector2[5];
        float angleStep = 360f / 5f;
        float currentAngle = 90f; // Start at top

        for (int i = 0; i < 5; i++)
        {
            float statPercent = Mathf.Clamp01(stats[i] / 10f) + 0.1f; // +0.1f buffer
            float radius = maxRadius * statPercent;
            float rad = currentAngle * Mathf.Deg2Rad;
            points[i] = new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
            currentAngle -= angleStep;
        }
        return points;
    }

    public static Vector2[] CalculateComplexIntersection(Vector2[] polyA, Vector2[] polyB)
    {
        List<Vector2> newShape = new List<Vector2>();
        int count = polyA.Length;

        for (int i = 0; i < count; i++)
        {
            // Get the current segment for both shapes
            int next = (i + 1) % count;
            
            Vector2 A1 = polyA[i];
            Vector2 A2 = polyA[next];
            Vector2 B1 = polyB[i];
            Vector2 B2 = polyB[next];

            // 1. Add the starting point (Whichever is closer to center is the "inner" boundary)
            if (A1.sqrMagnitude < B1.sqrMagnitude)
                newShape.Add(A1);
            else
                newShape.Add(B1);

            // 2. Check if the lines cross between these two angles
            Vector2 intersection;
            if (GetLineIntersection(A1, A2, B1, B2, out intersection))
            {
                // If they cross, we MUST add that crossing point to fill the gap
                newShape.Add(intersection);
            }
        }

        return newShape.ToArray();
    }

    private static bool GetLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        float d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0) return false;

        float u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        float v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u > 0f && u < 1f && v > 0f && v < 1f)
        {
            intersection.x = p1.x + u * (p2.x - p1.x);
            intersection.y = p1.y + u * (p2.y - p1.y);
            return true;
        }

        return false;
    }

}
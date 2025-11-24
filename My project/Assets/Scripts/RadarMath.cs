using UnityEngine;

public static class RadarMath
{
    public static Vector2[] CalculateVertices(int[] stats, float maxRadius)
    {
        Vector2[] points = new Vector2[5];
        float angleStep = 360f / 5f;
        float currentAngle = 90f; // Start at top

        for (int i = 0; i < 5; i++)
        {

            float statPercent = Mathf.Clamp01(stats[i] / 10f);
            float radius = maxRadius * (0.2f + (0.8f * statPercent));

            float rad = currentAngle * Mathf.Deg2Rad;
            points[i] = new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
            currentAngle -= angleStep;
        }
        return points;
    }
}
using UnityEngine;
public class RadarGrid : MonoBehaviour {

    [Header("Settings")]
    public MinigameManager manager;
    
    [Header("Grid Lines")]
    public Material lineMaterial; // Thin, transparent grey
    public float lineWidth = 0.05f;
    public int steps = 5; // Rings (2, 4, 6, 8, 10)

    void Start()
    {
        float radius = (manager != null) ? manager.graphRadius : 4f;

        // 1. Draw the Solid Background (The Max Size Pentagon)
        GenerateBackground(radius);

        // 2. Draw the Grid Lines (The Rings)
        GenerateGridLines(radius);
    }

    void GenerateBackground(float radius)
    {
        // Max stats (10) creates the full arena shape
        int[] maxStats = { 10, 10, 10, 10, 10 };
        Vector2[] maxPoints = RadarMath.CalculateVertices(maxStats, radius);

        // Use the PolygonDisplay attached to THIS object to draw the solid shape
        GetComponent<PolygonDisplay>().UpdateShape(maxPoints);
    }

    void GenerateGridLines(float radius)
    {
        for (int i = 1; i <= steps; i++)
        {
            // Calculate Stats (2, 4, 6, 8, 10)
            int statValue = i * (10 / steps);
            int[] currentStats = { statValue, statValue, statValue, statValue, statValue };

            // Create Child Object
            GameObject ringObj = new GameObject($"GridRing_Stat{statValue}");
            ringObj.transform.SetParent(transform, false);
            ringObj.transform.localPosition = new Vector3(0, 0, -0.1f); // Move lines slightly forward so they sit ON TOP of background
            
            // Add Line Renderer
            LineRenderer lr = ringObj.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.material = lineMaterial;
            lr.loop = true; 
            lr.positionCount = 5;

            // Calculate Points
            Vector2[] points = RadarMath.CalculateVertices(currentStats, radius);
            
            for (int p = 0; p < 5; p++)
            {
                lr.SetPosition(p, points[p]);
            }
        }
    }
}
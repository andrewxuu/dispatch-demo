using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PolygonDisplay : MonoBehaviour
{
    private Mesh mesh;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void UpdateShape(Vector2[] ringPoints)
    {
        Vector3[] vertices = new Vector3[ringPoints.Length + 1];
        vertices[0] = Vector3.zero; // Center point
        
        for (int i = 0; i < ringPoints.Length; i++)
        {
            vertices[i + 1] = ringPoints[i];
        }

        int[] triangles = new int[ringPoints.Length * 3];

        for (int i = 0; i < ringPoints.Length; i++)
        {
            int start = i + 1;
            int end = (i + 1) % ringPoints.Length + 1; 
            if (end == 1) end = ringPoints.Length; 


            triangles[i * 3] = 0;     // Center
            triangles[i * 3 + 1] = start; // Point A
            triangles[i * 3 + 2] = (i == ringPoints.Length - 1) ? 1 : i + 2; // Point B 
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
using UnityEngine;
public class ArenaSetup : MonoBehaviour {

    public MinigameManager manager;

    void Start() {
        // Create a  pentagon with Max Stats (10,10,10,10,10)
        int[] maxStats = {10,10,10,10,10};

        float currentSize = 4f; // Default fallback
        if (manager != null)
        {
            currentSize = manager.graphRadius;
        }
        GetComponent<PolygonDisplay>().UpdateShape(RadarMath.CalculateVertices(maxStats, currentSize));
    }
}
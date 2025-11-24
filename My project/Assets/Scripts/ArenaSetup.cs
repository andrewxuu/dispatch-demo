using UnityEngine;
public class ArenaSetup : MonoBehaviour {
    void Start() {
        // Create a  pentagon with Max Stats (10,10,10,10,10)
        int[] maxStats = {10,10,10,10,10};
        GetComponent<PolygonDisplay>().UpdateShape(RadarMath.CalculateVertices(maxStats, 4f));
    }
}
using UnityEngine;

public class DotController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Collider2D targetMissionZone;
    private float finishTime;
    private bool isRunning = false;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public void StartMission(float duration, Collider2D missionZone)
    {
        targetMissionZone = missionZone;
        finishTime = Time.time + duration;
        isRunning = true;

        transform.position = Vector3.zero; // Reset center
        rb.linearVelocity = Random.insideUnitCircle.normalized * speed; // Launch
    }

    void Update()
    {
        if (isRunning && Time.time >= finishTime)
        {
            FinishMission();
        }
    }

    void FinishMission()
    {
        isRunning = false;
        rb.linearVelocity = Vector2.zero;

        // CHECK: Is Dot in Mission Zone

        if (targetMissionZone.OverlapPoint(transform.position))
        {
            Debug.Log("<color=green>SUCCESS! Landed in zone.</color>");
        }
        else
        {
            Debug.Log("<color=red>FAILED. Landed outside.</color>");
        }
    }
}
using UnityEngine;

public class DotController : MonoBehaviour
{
    public float speed = 5f;
    public MinigameManager manager; 
    
    private Rigidbody2D rb;
    private Collider2D targetZone;
    private float finishTime;
    private bool isRunning = false;

    [HideInInspector] public bool forceWin = false;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public void StartMission(float duration, Collider2D zone)
    {
        targetZone = zone;
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
        rb.linearVelocity = Vector2.zero; //Stop Ball 

        bool isSuccess = forceWin || targetZone.OverlapPoint(transform.position);

        if (manager != null)
        {
            manager.OnMissionFinished(isSuccess);
        }
        else
        {
            Debug.LogError("Manager is not assigned on DotController!");
        }
    }
}
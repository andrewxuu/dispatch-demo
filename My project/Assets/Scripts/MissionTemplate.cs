using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "Dispatch/Mission")]
public class MissionTemplate : ScriptableObject
{
    public string missionName;
    public float durationSeconds = 3f; // Short for demo

    [Header("Team Settings")]
    [Range(1, 5)] public int teamSize = 3; // <--- ADD THIS

    [Range(0, 10)] public int reqCombat;
    [Range(0, 10)] public int reqVigor;
    [Range(0, 10)] public int reqSpeed;
    [Range(0, 10)] public int reqCharisma;
    [Range(0, 10)] public int reqIntelligence;
}
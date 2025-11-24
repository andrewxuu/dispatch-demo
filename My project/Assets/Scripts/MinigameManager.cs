using UnityEngine;
using TMPro;

public class MinigameManager : MonoBehaviour
{
    [Header("Mission")]
    public EdgeCollider2D boundaryCollider;
    public PolygonDisplay boundaryDisplay; 

    [Header("Hero")]
    public PolygonCollider2D successZoneCollider;
    public PolygonDisplay successZoneDisplay;

    public PolygonDisplay overlapDisplay; // Drag your 'OverlapZone' here

    [Header("Materials")]
    public Material defaultMissionMat; 
    public Material successMissionMat; 
    public Material failureOverlapMat; 

    [Header("Setup")]
    public float graphRadius = 2.5f; 
    public DotController dot;

    [Header("UI")]
    public TextMeshProUGUI percentageText; 

    private int[] currentHeroStats;
    private int[] currentMissionStats;
    
    public HeroStats testHero;
    public MissionTemplate testMission;

    private HeroStats pendingHero;
    private MissionTemplate pendingMission;

    /*void Start()
    {
        if(testHero != null && testMission != null) 
            StartGame(testHero, testMission);
    }
    */

    public void SetupPreview(HeroStats hero, MissionTemplate mission)
    {
        pendingHero = hero;
        pendingMission = mission;

        // Save stats for math later
        currentHeroStats = new int[] { hero.combat, hero.vigor, hero.speed, hero.charisma, hero.intelligence };
        currentMissionStats = new int[] { mission.reqCombat, mission.reqVigor, mission.reqSpeed, mission.reqCharisma, mission.reqIntelligence };

        // Calculate Visuals ONLY
        Vector2[] missionPoints = RadarMath.CalculateVertices(currentMissionStats, graphRadius);
        Vector2[] heroPoints = RadarMath.CalculateVertices(currentHeroStats, graphRadius);
        Vector2[] overlapPoints = RadarMath.CalculateComplexIntersection(heroPoints, missionPoints);

        // Update Shapes
        boundaryDisplay.UpdateShape(missionPoints);
        successZoneDisplay.UpdateShape(heroPoints);
        overlapDisplay.UpdateShape(overlapPoints);
        
        // Reset Visuals
        boundaryDisplay.GetComponent<MeshRenderer>().material = defaultMissionMat;
        overlapDisplay.gameObject.SetActive(false);
        
        // HIDE THE BALL for now (It's just a preview)
        dot.gameObject.SetActive(false);
        
        // Update Percentage Text
        float missionArea = RadarMath.CalculatePolygonArea(missionPoints);
        float overlapArea = RadarMath.CalculatePolygonArea(overlapPoints);
        float chance = (missionArea > 0.01f) ? (overlapArea / missionArea) * 100f : 0f;
        if(percentageText != null) percentageText.text = $"{Mathf.Clamp(chance, 0, 100):F1}%";
    }

    // 2. NEW FUNCTION: Actually start the bouncing
    public void ConfirmAndStart()
    {
        if (pendingHero == null || pendingMission == null) return;

        // Guaranteed Win Check
        if (CheckIfGuaranteedWin(currentHeroStats, currentMissionStats))
        {
            Debug.Log("Guaranteed Win!");
            OnMissionFinished(true); 
        }
        else
        {
            // Start the Ball
            dot.gameObject.SetActive(true);
            
            // Setup Physics
            Vector2[] missionPoints = RadarMath.CalculateVertices(currentMissionStats, graphRadius);
            Vector2[] overlapPoints = RadarMath.CalculateComplexIntersection(RadarMath.CalculateVertices(currentHeroStats, graphRadius), missionPoints);
            
            Vector2[] loopedMission = new Vector2[6];
            System.Array.Copy(missionPoints, loopedMission, 5);
            loopedMission[5] = missionPoints[0];
            boundaryCollider.points = loopedMission;
            successZoneCollider.SetPath(0, overlapPoints);

            dot.StartMission(pendingMission.durationSeconds, successZoneCollider);
        }
    }

    public void OnMissionFinished(bool isSuccess)
    {
        Vector2[] mPoints = RadarMath.CalculateVertices(currentMissionStats, graphRadius);
        Vector2[] hPoints = RadarMath.CalculateVertices(currentHeroStats, graphRadius);

        //PERFECT intersection
        Vector2[] overlapPoints = RadarMath.CalculateComplexIntersection(hPoints, mPoints);
        
        //Set Color & Message based on outcome
        if (isSuccess)
        {
            Debug.Log("<color=green>MISSION SUCCESS!</color>");
            overlapDisplay.GetComponent<MeshRenderer>().material = successMissionMat; // Green
        }
        else
        {
            Debug.Log("<color=red>MISSION FAILED.</color>");
            overlapDisplay.GetComponent<MeshRenderer>().material = failureOverlapMat; // Red
        }

        overlapDisplay.gameObject.SetActive(true);
    }

    bool CheckIfGuaranteedWin(int[] heroStats, int[] missionStats)
    {
        for (int i = 0; i < 5; i++)
        {
            if (heroStats[i] < missionStats[i])
            {
                return false; // Not a guaranteed success
            }
        }
        return true;
    }
}
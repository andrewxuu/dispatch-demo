using UnityEngine;

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
    public float graphRadius = 4f; 
    public DotController dot;

    private int[] currentHeroStats;
    private int[] currentMissionStats;
    
    public HeroStats testHero;
    public MissionTemplate testMission;

    void Start()
    {
        if(testHero != null && testMission != null) 
            StartGame(testHero, testMission);
    }

    public void StartGame(HeroStats hero, MissionTemplate mission)
    {
        //Get Stats
        int[] hStats = { hero.combat, hero.vigor, hero.speed, hero.charisma, hero.intelligence };
        int[] mStats = { mission.reqCombat, mission.reqVigor, mission.reqSpeed, mission.reqCharisma, mission.reqIntelligence };
        
        currentHeroStats = hStats; 
        currentMissionStats = mStats; 
        
        //Calculate Shapes
        Vector2[] heroPoints = RadarMath.CalculateVertices(hStats, graphRadius);
        Vector2[] missionPoints = RadarMath.CalculateVertices(mStats, graphRadius);

        Vector2[] overlapPoints = RadarMath.CalculateComplexIntersection(heroPoints, missionPoints);

        // Update Visuals
        boundaryDisplay.UpdateShape(missionPoints);
        successZoneDisplay.UpdateShape(heroPoints);

        overlapDisplay.UpdateShape(overlapPoints);
        overlapDisplay.gameObject.SetActive(false);

        //CHECK: Guaranteed Win (Hero Stats > Mission Stats)
        if (CheckIfGuaranteedWin(hStats, mStats))        
        {
            Debug.Log("Guaranteed Win!");
            OnMissionFinished(true); // Trigger win immediately
            dot.gameObject.SetActive(false);
        }
        else
        {
            // Reset Objects
            dot.gameObject.SetActive(true);
            boundaryDisplay.GetComponent<MeshRenderer>().material = defaultMissionMat;

            // Update Physics
            Vector2[] loopedMission = new Vector2[6];
            System.Array.Copy(missionPoints, loopedMission, 5);
            loopedMission[5] = missionPoints[0];
            boundaryCollider.points = loopedMission;

            successZoneCollider.SetPath(0, overlapPoints);

            dot.StartMission(mission.durationSeconds, successZoneCollider);
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
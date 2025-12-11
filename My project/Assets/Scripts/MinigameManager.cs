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

    [Header("Results UI")]
    public GameObject resultsPanel;      // Drag 'ResultsPanel' here
    public TextMeshProUGUI resultsTitle; 

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

    void Awake()
    {
        // 1. Force the Percentage Text OFF at startup
        if (percentageText != null) 
        {
            percentageText.text = "";
            percentageText.gameObject.SetActive(false);
        }

        // 2. Force the Mission Cage OFF at startup (Optional, if you want it hidden too)
        if (boundaryDisplay != null) 
        {
            boundaryDisplay.gameObject.SetActive(false);
        }

        // 3. Force the Ball OFF
        if (dot != null)
        {
            dot.gameObject.SetActive(false);
        }
    }

    public void SetupPreview(HeroStats hero, MissionTemplate mission)
    {
        pendingHero = hero;
        pendingMission = mission;

        // Save stats for math later
        currentHeroStats = new int[] { hero.combat, hero.vigor, hero.speed, hero.charisma, hero.intelligence };
        currentMissionStats = new int[] { mission.reqCombat, mission.reqVigor, mission.reqSpeed, mission.reqCharisma, mission.reqIntelligence };

        // 1. Calculate Visuals
        Vector2[] missionPoints = RadarMath.CalculateVertices(currentMissionStats, graphRadius);
        Vector2[] heroPoints = RadarMath.CalculateVertices(currentHeroStats, graphRadius);
        Vector2[] overlapPoints = RadarMath.CalculateComplexIntersection(heroPoints, missionPoints);

        // 2. Update Shapes
        // We still UPDATE the shape data, but we hide the object immediately after
        boundaryDisplay.UpdateShape(missionPoints);
        successZoneDisplay.UpdateShape(heroPoints);
        overlapDisplay.UpdateShape(overlapPoints);
        
        // --- VISIBILITY CHANGES ---
        // SHOW the Hero (Yellow)
        successZoneDisplay.gameObject.SetActive(true);

        // HIDE the Mission Cage (Red/Grey) until dispatch
        boundaryDisplay.gameObject.SetActive(false); 
        boundaryDisplay.GetComponent<MeshRenderer>().material = defaultMissionMat;

        // HIDE the Overlap Zone
        overlapDisplay.gameObject.SetActive(false);
        
        // HIDE the Ball
        dot.gameObject.SetActive(false);
        
        // HIDE the Text
        if(percentageText != null) 
        {
            percentageText.text = ""; // Clear text
            percentageText.gameObject.SetActive(false); // Hide object
        }
    }

    public void ConfirmAndStart()
    {
        if (pendingHero == null || pendingMission == null) return;

        // --- VISIBILITY CHANGES ---
        // 1. REVEAL the Mission Cage now
        boundaryDisplay.gameObject.SetActive(true);

        // 2. REVEAL and Calculate the Text now
        if (percentageText != null)
        {
            Vector2[] mPoints = RadarMath.CalculateVertices(currentMissionStats, graphRadius);
            Vector2[] hPoints = RadarMath.CalculateVertices(currentHeroStats, graphRadius);
            Vector2[] oPoints = RadarMath.CalculateComplexIntersection(hPoints, mPoints);

            float missionArea = RadarMath.CalculatePolygonArea(mPoints);
            float overlapArea = RadarMath.CalculatePolygonArea(oPoints);
            float chance = (missionArea > 0.01f) ? (overlapArea / missionArea) * 100f : 0f;
            
            percentageText.text = $"{Mathf.Clamp(chance, 0, 100):F1}%";
            percentageText.gameObject.SetActive(true);
        }

        // ---------------------------

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
        // Stop the ball physics (Optional, keeps things clean)
        dot.gameObject.SetActive(false);

        // Update the Overlap Visuals (Green/Red)
        Vector2[] mPoints = RadarMath.CalculateVertices(currentMissionStats, graphRadius);
        Vector2[] hPoints = RadarMath.CalculateVertices(currentHeroStats, graphRadius);
        Vector2[] overlapPoints = RadarMath.CalculateComplexIntersection(hPoints, mPoints);
        overlapDisplay.UpdateShape(overlapPoints);
        overlapDisplay.gameObject.SetActive(true);

        // Show the Results Panel
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
            
            if (isSuccess)
            {
                resultsTitle.text = "MISSION SUCCESS";
                resultsTitle.color = Color.green;
                overlapDisplay.GetComponent<MeshRenderer>().material = successMissionMat;
            }
            else
            {
                resultsTitle.text = "MISSION FAILED";
                resultsTitle.color = Color.red;
                overlapDisplay.GetComponent<MeshRenderer>().material = failureOverlapMat;
            }
        }
    }

    public void ResetGameVisuals()
    {
        // Hide Results
        if (resultsPanel != null) resultsPanel.SetActive(false);

        // Hide Game Objects
        boundaryDisplay.gameObject.SetActive(false);
        overlapDisplay.gameObject.SetActive(false);
        dot.gameObject.SetActive(false);
        if(percentageText != null) percentageText.gameObject.SetActive(false);
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
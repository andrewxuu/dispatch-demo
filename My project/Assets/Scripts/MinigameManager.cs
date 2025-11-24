using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [Header("Visuals")]
    public EdgeCollider2D heroBoundary;
    public LineRenderer heroLine;
    public PolygonCollider2D missionZone;
    public LineRenderer missionLine;

    [Header("Setup")]
    public float graphRadius = 4f; 
    public DotController dot;
    
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

        //Calculate Shapes
        Vector2[] heroPoints = RadarMath.CalculateVertices(hStats, graphRadius);
        Vector2[] missionPoints = RadarMath.CalculateVertices(mStats, graphRadius);

        //Update Hero Boundary (Needs loop closed for EdgeCollider)
        Vector2[] loopedHero = new Vector2[6];
        System.Array.Copy(heroPoints, loopedHero, 5);
        loopedHero[5] = heroPoints[0];
        
        heroBoundary.points = loopedHero;
        heroLine.positionCount = 6;
        for(int i=0; i<6; i++) heroLine.SetPosition(i, loopedHero[i]);

        //Update Mission Zone
        missionZone.SetPath(0, missionPoints);
        missionLine.positionCount = 5; 
        for(int i=0; i<5; i++) missionLine.SetPosition(i, missionPoints[i]);

        dot.StartMission(mission.durationSeconds, missionZone);
    }
}
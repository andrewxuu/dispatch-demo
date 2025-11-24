using UnityEngine;

[CreateAssetMenu(fileName = "New Hero", menuName = "Dispatch/Hero")]
public class HeroStats : ScriptableObject
{
    public string heroName;
    public Sprite heroPortrait; 

    [Range(0, 10)] public int combat;
    [Range(0, 10)] public int vigor;
    [Range(0, 10)] public int speed;
    [Range(0, 10)] public int charisma;
    [Range(0, 10)] public int intelligence;
}
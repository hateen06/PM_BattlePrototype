using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillData", menuName = "Battle/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public int damage;
    public bool isGuard;
}
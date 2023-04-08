using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Attack",menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;//技能范围
    public float coolDown;//冷却时间
    public int minDamage;
    public int maxDamage;
    public float criticalMultiplier;//暴击伤害加成
    public float criticalChance;//暴击几率
}

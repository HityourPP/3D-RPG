using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Attack",menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;//���ܷ�Χ
    public float coolDown;//��ȴʱ��
    public int minDamage;
    public int maxDamage;
    public float criticalMultiplier;//�����˺��ӳ�
    public float criticalChance;//��������
}

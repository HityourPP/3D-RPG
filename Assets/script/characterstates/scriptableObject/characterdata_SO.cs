using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName ="New Data",menuName ="character States/Data")]//ʹ������ڴ����ļ��˵�
public class characterdata_SO : ScriptableObject//�޸���̳е���
{
    private readonly object get;
    [Header("States Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefense;
    [Header("Kill")]
    public int killpoint;
    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;//����ʱ�����İٷֱ�
    public float LevelMultiplier//����ʹ�����ٷֱ���ȼ���߶����Ӹ���
    {
        get{ return 1 + (currentLevel - 1) * levelBuff; }
    }
    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
            LevelUp();
    }
    private void LevelUp()
    {//������������������������
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        //Mathf.ClampΪ�ж�currentLevel + 1��Χ��0��maxLevel֮��
        baseExp += (int)(baseExp * LevelMultiplier);//ʹ���������������辭����
        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;//Ѫ������
        //baseDefence = (int)(baseDefence * LevelMultiplier);
        Debug.Log("Level Up!" + currentLevel + "Max Health" + maxHealth);
    }
}

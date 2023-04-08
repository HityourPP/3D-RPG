using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName ="New Data",menuName ="character States/Data")]//使其出现在创建文件菜单
public class characterdata_SO : ScriptableObject//修改其继承的类
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
    public float levelBuff;//升级时提升的百分比
    public float LevelMultiplier//这里使提升百分比随等级提高而增加更多
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
    {//发挥想象提升想提升的数据
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        //Mathf.Clamp为判断currentLevel + 1范围在0到maxLevel之间
        baseExp += (int)(baseExp * LevelMultiplier);//使升级后再升级所需经验变多
        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;//血量回满
        //baseDefence = (int)(baseDefence * LevelMultiplier);
        Debug.Log("Level Up!" + currentLevel + "Max Health" + maxHealth);
    }
}

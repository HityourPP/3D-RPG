using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarAttack;//创建事件来实现血条对应减少
    public characterdata_SO templateData;//作为copy模板
    public characterdata_SO characterdata;
    public AttackData_SO attackData;
    [HideInInspector]//使其不出现在Inspector窗口处
    public bool isCritical;//判断是否暴击
    void Awake()
    {
        if(templateData != null)
        {
            characterdata = Instantiate(templateData);//将copy模板生成一个副本来使用
        }
    }
    #region read from Data_SO    //让冗长的代码折叠起来
    public int MaxHealth { //get为可读，set为可写        
        get{if (characterdata != null)return characterdata.maxHealth;else return 0;}//读取其数据
        set{characterdata.maxHealth = value;}//对其修改
    }
    public int CurrentHealth
    { //get为可读，set为可写        
        get { if (characterdata != null) return characterdata.currentHealth; else return 0; }//读取其数据
        set { characterdata.currentHealth = value; }//对其修改
    }
    public int BaseDefence
    { //get为可读，set为可写        
        get { if (characterdata != null) return characterdata.baseDefence; else return 0; }//读取其数据
        set { characterdata.baseDefence = value; }//对其修改
    }
    public int CurrentDefence
    { //get为可读，set为可写        
        get { if (characterdata != null) return characterdata.currentDefense; else return 0; }//读取其数据
        set { characterdata.currentDefense = value; }//对其修改
    }
    #endregion
    #region Character Combat
    public void TakeDamage(CharacterStats attacker,CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,1);//Mathf.max确保伤害最小为1
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);//Mathf.max确保生命最小为0
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("hit");//获得其animator并修改hit值
        }
        //TODO：添加生命值UI
        UpdateHealthBarAttack?.Invoke(CurrentHealth, MaxHealth);//?.Invoke为判断是否为空
        //TODO：人物属性升级（经验）
        if (CurrentHealth <= 0)
            attacker.characterdata.UpdateExp(characterdata.killpoint);//获取击杀敌人的经验
    }
    public void TakeDamage(int damage,CharacterStats defener)//函数重载
    {
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdateHealthBarAttack?.Invoke(CurrentHealth, MaxHealth);//反击石头人也需要执行血条减少
        if (CurrentHealth <= 0)
        {//用石头反击时也能获得经验
            GameManager.Instance.playerStats.characterdata.UpdateExp(characterdata.killpoint);
        }
    }
    private int CurrentDamage()//获取目前的伤害，在最大伤害与最小伤害之间选择一个随机数
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)//暴击伤害加成
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击！"+coreDamage);//测试是否暴击
        }
        return (int)coreDamage;
    }


    #endregion
}

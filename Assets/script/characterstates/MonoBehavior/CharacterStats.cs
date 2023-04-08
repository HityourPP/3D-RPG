using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarAttack;//�����¼���ʵ��Ѫ����Ӧ����
    public characterdata_SO templateData;//��Ϊcopyģ��
    public characterdata_SO characterdata;
    public AttackData_SO attackData;
    [HideInInspector]//ʹ�䲻������Inspector���ڴ�
    public bool isCritical;//�ж��Ƿ񱩻�
    void Awake()
    {
        if(templateData != null)
        {
            characterdata = Instantiate(templateData);//��copyģ������һ��������ʹ��
        }
    }
    #region read from Data_SO    //���߳��Ĵ����۵�����
    public int MaxHealth { //getΪ�ɶ���setΪ��д        
        get{if (characterdata != null)return characterdata.maxHealth;else return 0;}//��ȡ������
        set{characterdata.maxHealth = value;}//�����޸�
    }
    public int CurrentHealth
    { //getΪ�ɶ���setΪ��д        
        get { if (characterdata != null) return characterdata.currentHealth; else return 0; }//��ȡ������
        set { characterdata.currentHealth = value; }//�����޸�
    }
    public int BaseDefence
    { //getΪ�ɶ���setΪ��д        
        get { if (characterdata != null) return characterdata.baseDefence; else return 0; }//��ȡ������
        set { characterdata.baseDefence = value; }//�����޸�
    }
    public int CurrentDefence
    { //getΪ�ɶ���setΪ��д        
        get { if (characterdata != null) return characterdata.currentDefense; else return 0; }//��ȡ������
        set { characterdata.currentDefense = value; }//�����޸�
    }
    #endregion
    #region Character Combat
    public void TakeDamage(CharacterStats attacker,CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,1);//Mathf.maxȷ���˺���СΪ1
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);//Mathf.maxȷ��������СΪ0
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("hit");//�����animator���޸�hitֵ
        }
        //TODO���������ֵUI
        UpdateHealthBarAttack?.Invoke(CurrentHealth, MaxHealth);//?.InvokeΪ�ж��Ƿ�Ϊ��
        //TODO�������������������飩
        if (CurrentHealth <= 0)
            attacker.characterdata.UpdateExp(characterdata.killpoint);//��ȡ��ɱ���˵ľ���
    }
    public void TakeDamage(int damage,CharacterStats defener)//��������
    {
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdateHealthBarAttack?.Invoke(CurrentHealth, MaxHealth);//����ʯͷ��Ҳ��Ҫִ��Ѫ������
        if (CurrentHealth <= 0)
        {//��ʯͷ����ʱҲ�ܻ�þ���
            GameManager.Instance.playerStats.characterdata.UpdateExp(characterdata.killpoint);
        }
    }
    private int CurrentDamage()//��ȡĿǰ���˺���������˺�����С�˺�֮��ѡ��һ�������
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)//�����˺��ӳ�
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("������"+coreDamage);//�����Ƿ񱩻�
        }
        return (int)coreDamage;
    }


    #endregion
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    TextMeshProUGUI LeveText;
    Image healthSlider;
    Image expSlider;
    void Awake()
    {
        LeveText=transform.GetChild(2).GetComponent<TextMeshProUGUI>();//��ȡ���Ӷ��󣬰���˳��0��1��2
        healthSlider=transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider=transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }
    void Update()
    {
        //�޸ĵȼ��ı���ToString("00")������Ϊ��λ��
        LeveText.text = "Level:" + GameManager.Instance.playerStats.characterdata.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }
    void UpdateHealth()
    {//��ȡ�ٷֱ�
        float sliderPercent = (float)GameManager.Instance.playerStats.CurrentHealth / GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }
    void UpdateExp()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.characterdata.currentExp / GameManager.Instance.playerStats.characterdata.baseExp;
        expSlider.fillAmount = sliderPercent;
    }
}

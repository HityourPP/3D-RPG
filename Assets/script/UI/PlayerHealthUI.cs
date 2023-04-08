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
        LeveText=transform.GetChild(2).GetComponent<TextMeshProUGUI>();//获取其子对象，按照顺序0，1，2
        healthSlider=transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider=transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }
    void Update()
    {
        //修改等级文本，ToString("00")将其设为两位数
        LeveText.text = "Level:" + GameManager.Instance.playerStats.characterdata.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }
    void UpdateHealth()
    {//获取百分比
        float sliderPercent = (float)GameManager.Instance.playerStats.CurrentHealth / GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }
    void UpdateExp()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.characterdata.currentExp / GameManager.Instance.playerStats.characterdata.baseExp;
        expSlider.fillAmount = sliderPercent;
    }
}

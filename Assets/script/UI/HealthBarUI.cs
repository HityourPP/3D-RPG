using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIprefabs;//获取其血条UI对象
    public Transform barPoint;//获取血条位置
    Image healthSlider;//获取到可以滑动的血条
    Transform UIbar;//与上面血条位置保持一致
    Transform cam;//获取摄像机位置，使血条与摄像机一直保持一致
    CharacterStats currentStats;
    public bool alawaysVisible;//控制其血条是否在场景中一直存在或可以消失
    public float visibleTime;//可视时间
    public float timeLeft;//显示的剩余时间
    void Awake()
    {
        currentStats = GetComponent<CharacterStats>();//获取自身对象，初始化
        currentStats.UpdateHealthBarAttack += UpdateHealthBar;//事件的调用格式
    }
    void OnEnable()//当人物启动时会调用该函数
    {
        cam = Camera.main.transform;//获取主摄像机的位置
        foreach(Canvas canvas in FindObjectsOfType<Canvas>())//Objects加s,这里寻找到场景中所有的canvas
        {
            if (canvas.renderMode == RenderMode.WorldSpace)//这里若有其他worldspace需要创建变量获取而非直接使用
            {
                UIbar = Instantiate(healthUIprefabs, canvas.transform).transform;
                healthSlider= UIbar.GetChild(0).GetComponent<Image>();//获取其子对象的image
                UIbar.gameObject.SetActive(alawaysVisible);//初始化使其不可见
            }
        }
    }
    void UpdateHealthBar(int currentHealth,int maxHealth)
    {
        if (currentHealth <= 0)  
            Destroy(UIbar.gameObject);//摧毁血条
        UIbar.gameObject.SetActive(true);
        timeLeft = visibleTime;

        float sliderPercent = (float)currentHealth / maxHealth;//获取百分比
        healthSlider.fillAmount = sliderPercent;//更新其滑动条
    }
    void LateUpdate()//该函数会在上一帧渲染之后才执行
    {
        if(UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward=-cam.forward;//使血条面向摄像机
            if (timeLeft < 0 && !alawaysVisible)
                UIbar.gameObject.SetActive(false);//时间结束后使血条消失
            else timeLeft -= Time.deltaTime;
        }
    }
}

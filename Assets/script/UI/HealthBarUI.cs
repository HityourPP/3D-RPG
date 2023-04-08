using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIprefabs;//��ȡ��Ѫ��UI����
    public Transform barPoint;//��ȡѪ��λ��
    Image healthSlider;//��ȡ�����Ի�����Ѫ��
    Transform UIbar;//������Ѫ��λ�ñ���һ��
    Transform cam;//��ȡ�����λ�ã�ʹѪ���������һֱ����һ��
    CharacterStats currentStats;
    public bool alawaysVisible;//������Ѫ���Ƿ��ڳ�����һֱ���ڻ������ʧ
    public float visibleTime;//����ʱ��
    public float timeLeft;//��ʾ��ʣ��ʱ��
    void Awake()
    {
        currentStats = GetComponent<CharacterStats>();//��ȡ������󣬳�ʼ��
        currentStats.UpdateHealthBarAttack += UpdateHealthBar;//�¼��ĵ��ø�ʽ
    }
    void OnEnable()//����������ʱ����øú���
    {
        cam = Camera.main.transform;//��ȡ���������λ��
        foreach(Canvas canvas in FindObjectsOfType<Canvas>())//Objects��s,����Ѱ�ҵ����������е�canvas
        {
            if (canvas.renderMode == RenderMode.WorldSpace)//������������worldspace��Ҫ����������ȡ����ֱ��ʹ��
            {
                UIbar = Instantiate(healthUIprefabs, canvas.transform).transform;
                healthSlider= UIbar.GetChild(0).GetComponent<Image>();//��ȡ���Ӷ����image
                UIbar.gameObject.SetActive(alawaysVisible);//��ʼ��ʹ�䲻�ɼ�
            }
        }
    }
    void UpdateHealthBar(int currentHealth,int maxHealth)
    {
        if (currentHealth <= 0)  
            Destroy(UIbar.gameObject);//�ݻ�Ѫ��
        UIbar.gameObject.SetActive(true);
        timeLeft = visibleTime;

        float sliderPercent = (float)currentHealth / maxHealth;//��ȡ�ٷֱ�
        healthSlider.fillAmount = sliderPercent;//�����们����
    }
    void LateUpdate()//�ú���������һ֡��Ⱦ֮���ִ��
    {
        if(UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward=-cam.forward;//ʹѪ�����������
            if (timeLeft < 0 && !alawaysVisible)
                UIbar.gameObject.SetActive(false);//ʱ�������ʹѪ����ʧ
            else timeLeft -= Time.deltaTime;
        }
    }
}

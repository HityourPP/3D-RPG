using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    CanvasGroup canvasGroup;//��ȡ�����

    public float fadeInDuration;
    public float fadeOutDuration;//��ȡ���뽥���������ֵ
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();//��ʼ��
        DontDestroyOnLoad(gameObject);//ʹ�����л�����ʱ��Ȼ����
    }
    public IEnumerator FadeOutIn()//������������Э��
    {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }
    public IEnumerator FadeOut(float time)//����Э��
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha+=Time.deltaTime/time;//����ʱ�����
            yield return null;
        }
    }
    public IEnumerator FadeIn(float time)//����Э��
    {
        while (canvasGroup.alpha !=0)
        {
            canvasGroup.alpha -= Time.deltaTime / time;//����ʱ��ݼ�
            yield return null;
        }
        Destroy(gameObject);//������ɺ󽫸û�����ȥ
    }

}

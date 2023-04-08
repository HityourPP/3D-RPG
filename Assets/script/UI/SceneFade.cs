using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    CanvasGroup canvasGroup;//获取该组件

    public float fadeInDuration;
    public float fadeOutDuration;//获取渐入渐出的相关数值
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();//初始化
        DontDestroyOnLoad(gameObject);//使其在切换场景时依然存在
    }
    public IEnumerator FadeOutIn()//整合下面两个协程
    {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }
    public IEnumerator FadeOut(float time)//创建协程
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha+=Time.deltaTime/time;//根据时间递增
            yield return null;
        }
    }
    public IEnumerator FadeIn(float time)//创建协程
    {
        while (canvasGroup.alpha !=0)
        {
            canvasGroup.alpha -= Time.deltaTime / time;//根据时间递减
            yield return null;
        }
        Destroy(gameObject);//加载完成后将该画布撤去
    }

}

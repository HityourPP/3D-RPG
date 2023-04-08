using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:Singleton <T>
//这里学习一下泛型单例模式的创建方法,T用来表示其类型,where后为约束类型
{
    private static  T instance;
    public static T Instance//通过该函数可从外部访问
    {
        get { return instance; }//对其只进行读操作
    }
    protected virtual void Awake()//这里利用类的继承，virtual表示该函数可在继承的类中重写
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
            instance = (T)this;//由于可能由不同的类继承，所以加上泛型T类型，最终使其返回自己的类型
    }
    public static bool IsInitialized//用来显示当前泛型单例模式是否已经生成
    {
        get { return (instance != null); }
    }
    protected virtual void OnDestory()//当场景中有多个单例模式时，需要将使用过的或多余的销毁掉
    {
        if(instance == this)
        {
            instance = null;
        }
    }

}

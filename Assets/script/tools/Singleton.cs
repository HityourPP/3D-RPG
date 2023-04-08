using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:Singleton <T>
//����ѧϰһ�·��͵���ģʽ�Ĵ�������,T������ʾ������,where��ΪԼ������
{
    private static  T instance;
    public static T Instance//ͨ���ú����ɴ��ⲿ����
    {
        get { return instance; }//����ֻ���ж�����
    }
    protected virtual void Awake()//����������ļ̳У�virtual��ʾ�ú������ڼ̳е�������д
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
            instance = (T)this;//���ڿ����ɲ�ͬ����̳У����Լ��Ϸ���T���ͣ�����ʹ�䷵���Լ�������
    }
    public static bool IsInitialized//������ʾ��ǰ���͵���ģʽ�Ƿ��Ѿ�����
    {
        get { return (instance != null); }
    }
    protected virtual void OnDestory()//���������ж������ģʽʱ����Ҫ��ʹ�ù��Ļ��������ٵ�
    {
        if(instance == this)
        {
            instance = null;
        }
    }

}

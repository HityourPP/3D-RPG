using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;//������ȡ��ɫ״̬
    private CinemachineFreeLook followCamera;//�õ��ý�ɫ�����Cinemachine
    //�����б��ռ����м���IEndGameObserver�ӿڵĺ��������ڻᴴ��������ˣ��ʻ��ж������
    List<IEndGameObserver> endGameObservers=new List<IEndGameObserver>();
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public void RegisterPlayer(CharacterStats player)//ͨ���۲���ģʽ������ע��ķ�������player�����ɵ�ʱ���ȡ��״̬
    {
        playerStats = player;
        followCamera=FindObjectOfType<CinemachineFreeLook>();//��ȡ�����е�cinemachine
        if (followCamera != null)//��ʼ�������ʹ������ɫ
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }
    public void AddObserver(IEndGameObserver observer)//���뵽�б���
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)//���б����Ƴ�
    {
        endGameObservers.Remove(observer);
    }
    public void NoitifyObserver()//ʹ���뵽�б��е��������ӿڵĶ���ִ�иú���
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
    public Transform GetEntrance()
    {
        foreach(var item in FindObjectsOfType<TransitionDestination>())//Ѱ�ҽ��볡���ĵ�
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.Enter)
                return item.transform;//���ظõ�λ��
        }
        return null;
    }
}

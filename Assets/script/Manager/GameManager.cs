using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;//用来获取角色状态
    private CinemachineFreeLook followCamera;//拿到该角色跟随的Cinemachine
    //创建列表收集所有加载IEndGameObserver接口的函数，由于会创建多个敌人，故会有多个调用
    List<IEndGameObserver> endGameObservers=new List<IEndGameObserver>();
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public void RegisterPlayer(CharacterStats player)//通过观察者模式，反向注册的方法，让player在生成的时候获取其状态
    {
        playerStats = player;
        followCamera=FindObjectOfType<CinemachineFreeLook>();//获取场景中的cinemachine
        if (followCamera != null)//初始化摄像机使其跟随角色
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }
    public void AddObserver(IEndGameObserver observer)//加入到列表中
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)//从列表中移除
    {
        endGameObservers.Remove(observer);
    }
    public void NoitifyObserver()//使加入到列表中调用上述接口的都会执行该函数
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
    public Transform GetEntrance()
    {
        foreach(var item in FindObjectsOfType<TransitionDestination>())//寻找进入场景的点
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.Enter)
                return item.transform;//返回该点位置
        }
        return null;
    }
}

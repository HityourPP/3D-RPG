using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
public class SceneController : Singleton<SceneController>,IEndGameObserver//使用单例模式,同时获得之前的广播
{
    public GameObject playerFrefab;//获取角色预制体的对象

    public SceneFade sceneFaderPrefab;//获取渐变画布的预制体

    bool fadeFinish;//避免多次广播而重复执行函数

    GameObject player;//获取角色对象
    NavMeshAgent playeragent;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        GameManager.Instance.AddObserver(this);//在GameManager中注册观察者以能收取广播
        fadeFinish = true;
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)//判断是否在同一场景
        {
            case TransitionPoint.TransitionType.SameScene:
                //括号内为获取当前激活的场景的名字，以及要传送到的目标标签
                StartCoroutine(Transition(SceneManager.GetActiveScene().name,transitionPoint.destinationTag));//开始下面的协程
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        //TODO:添加Fader
        SaveManager.Instance.SavePlayerData();
        if(SceneManager.GetActiveScene().name != sceneName)//当前场景与要传送场景不同时
        {//LoadSceneAsync(sceneName)为异步加载新的场景
            yield return SceneManager.LoadSceneAsync(sceneName);//yield return 为等待该事件完成后在执行下面的流程
            //在对应位置初始化形成一个角色预制体
            yield return Instantiate(playerFrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();//加载数据
            yield break;//中断协程
        }
        else
        {
        player = GameManager.Instance.playerStats.gameObject;
        playeragent=player.GetComponent<NavMeshAgent>();
        playeragent.enabled= false;//传送前关闭移动
        //SetPositionAndRotation为设置变换组件的世空间位置和旋转，括号中为传送的位置和旋转角度
        player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
        playeragent.enabled = true;//传送后再启动
        yield return null;
        }
       
    }
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();//获取场景中的对象Objects注意加s
        for (int i = 0; i < entrances.Length;i++)
        {
            if (entrances[i].destinationTag== destinationTag)//找到对应标签返回该对象
                return entrances[i];
        }
        return null;
    }
    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());//启用协程
    }
    public void TransitionToLoadGame()//创建加载对应场景的函数
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }
    public void TransitionToFirstLevel()//创建函数启用协程，同时也使其他代码文件能调用函数
    {
        StartCoroutine(LoadLevel("SampleScene"));
    }
    IEnumerator LoadLevel(string scene)
    {
        SceneFade fade = Instantiate(sceneFaderPrefab);//生成该画布
        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2.5f));//开始进行白屏，在白屏后创建角色加载场景
            yield return SceneManager.LoadSceneAsync(scene);//加载场景
            //在初始位置生成角色
            yield return player = Instantiate(playerFrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
            //保存游戏
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2.5f));//从白屏显示出来
            yield break;//结束协程
        }
    }
    IEnumerator LoadMain()
    {
        SceneFade fade = Instantiate(sceneFaderPrefab);//返回主菜单实现渐入渐出效果
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("MainMenu");//异步加载到主菜单场景
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    public void EndNotify()
    {
        if (fadeFinish)
        {
            StartCoroutine(LoadMain());//角色死亡后返回主菜单
            fadeFinish = false;
        }
        
    }
}

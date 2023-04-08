using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
public class SceneController : Singleton<SceneController>,IEndGameObserver//ʹ�õ���ģʽ,ͬʱ���֮ǰ�Ĺ㲥
{
    public GameObject playerFrefab;//��ȡ��ɫԤ����Ķ���

    public SceneFade sceneFaderPrefab;//��ȡ���仭����Ԥ����

    bool fadeFinish;//�����ι㲥���ظ�ִ�к���

    GameObject player;//��ȡ��ɫ����
    NavMeshAgent playeragent;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        GameManager.Instance.AddObserver(this);//��GameManager��ע��۲���������ȡ�㲥
        fadeFinish = true;
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)//�ж��Ƿ���ͬһ����
        {
            case TransitionPoint.TransitionType.SameScene:
                //������Ϊ��ȡ��ǰ����ĳ��������֣��Լ�Ҫ���͵���Ŀ���ǩ
                StartCoroutine(Transition(SceneManager.GetActiveScene().name,transitionPoint.destinationTag));//��ʼ�����Э��
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        //TODO:���Fader
        SaveManager.Instance.SavePlayerData();
        if(SceneManager.GetActiveScene().name != sceneName)//��ǰ������Ҫ���ͳ�����ͬʱ
        {//LoadSceneAsync(sceneName)Ϊ�첽�����µĳ���
            yield return SceneManager.LoadSceneAsync(sceneName);//yield return Ϊ�ȴ����¼���ɺ���ִ�����������
            //�ڶ�Ӧλ�ó�ʼ���γ�һ����ɫԤ����
            yield return Instantiate(playerFrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();//��������
            yield break;//�ж�Э��
        }
        else
        {
        player = GameManager.Instance.playerStats.gameObject;
        playeragent=player.GetComponent<NavMeshAgent>();
        playeragent.enabled= false;//����ǰ�ر��ƶ�
        //SetPositionAndRotationΪ���ñ任��������ռ�λ�ú���ת��������Ϊ���͵�λ�ú���ת�Ƕ�
        player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
        playeragent.enabled = true;//���ͺ�������
        yield return null;
        }
       
    }
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();//��ȡ�����еĶ���Objectsע���s
        for (int i = 0; i < entrances.Length;i++)
        {
            if (entrances[i].destinationTag== destinationTag)//�ҵ���Ӧ��ǩ���ظö���
                return entrances[i];
        }
        return null;
    }
    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());//����Э��
    }
    public void TransitionToLoadGame()//�������ض�Ӧ�����ĺ���
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }
    public void TransitionToFirstLevel()//������������Э�̣�ͬʱҲʹ���������ļ��ܵ��ú���
    {
        StartCoroutine(LoadLevel("SampleScene"));
    }
    IEnumerator LoadLevel(string scene)
    {
        SceneFade fade = Instantiate(sceneFaderPrefab);//���ɸû���
        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2.5f));//��ʼ���а������ڰ����󴴽���ɫ���س���
            yield return SceneManager.LoadSceneAsync(scene);//���س���
            //�ڳ�ʼλ�����ɽ�ɫ
            yield return player = Instantiate(playerFrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
            //������Ϸ
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2.5f));//�Ӱ�����ʾ����
            yield break;//����Э��
        }
    }
    IEnumerator LoadMain()
    {
        SceneFade fade = Instantiate(sceneFaderPrefab);//�������˵�ʵ�ֽ��뽥��Ч��
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("MainMenu");//�첽���ص����˵�����
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    public void EndNotify()
    {
        if (fadeFinish)
        {
            StartCoroutine(LoadMain());//��ɫ�����󷵻����˵�
            fadeFinish = false;
        }
        
    }
}

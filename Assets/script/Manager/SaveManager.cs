using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveManager : Singleton<SaveManager>//���͵���ģʽ
{
    string sceneName="";//��Ϊ���泡���ļ�ֵ,Ĭ��Ϊ��
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }//��ȡ���泡��������
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);//���л�����ʱ�����ٸô���
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))//����esc���ص���������
        {
            SceneController.Instance.TransitionToMain();
        }
        if (Input.GetKeyDown(KeyCode.S))//����S������
        {
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L))//����L����ȡ
        {
           LoadPlayerData(); 
        }
    }
    public void SavePlayerData()
    {
      //�����ɫ���ݣ�Ҳ�������Ƶķ���������Ҫ���������
        Save(GameManager.Instance.playerStats.characterdata, GameManager.Instance.playerStats.characterdata.name);
    }
    public void LoadPlayerData()
    {//��������
        Load(GameManager.Instance.playerStats.characterdata, GameManager.Instance.playerStats.characterdata.name);
    }
    public void Save(Object data,string key)
    {
        var jsonData=JsonUtility.ToJson(data,true);//����һ��Json����������ת��ΪString����
        PlayerPrefs.SetString(key, jsonData);//������������
        PlayerPrefs.SetString(sceneName,SceneManager.GetActiveScene().name);//������������������
        PlayerPrefs.Save();//��������
    }
    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))//�жϸü�ֵ���Ƿ񱣴�������
        {//����Json�������string���͵�����ת������
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}

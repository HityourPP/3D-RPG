using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveManager : Singleton<SaveManager>//泛型单例模式
{
    string sceneName="";//作为保存场景的键值,默认为空
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }//获取保存场景的名字
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);//在切换场景时不销毁该代码
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))//按下esc返回到主场景中
        {
            SceneController.Instance.TransitionToMain();
        }
        if (Input.GetKeyDown(KeyCode.S))//按下S键保存
        {
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L))//按下L键读取
        {
           LoadPlayerData(); 
        }
    }
    public void SavePlayerData()
    {
      //保存角色数据，也可用类似的方法保存想要保存的数据
        Save(GameManager.Instance.playerStats.characterdata, GameManager.Instance.playerStats.characterdata.name);
    }
    public void LoadPlayerData()
    {//加载数据
        Load(GameManager.Instance.playerStats.characterdata, GameManager.Instance.playerStats.characterdata.name);
    }
    public void Save(Object data,string key)
    {
        var jsonData=JsonUtility.ToJson(data,true);//创建一个Json变量，将其转换为String类型
        PlayerPrefs.SetString(key, jsonData);//创建保存数据
        PlayerPrefs.SetString(sceneName,SceneManager.GetActiveScene().name);//保存所属场景的名字
        PlayerPrefs.Save();//保存数据
    }
    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))//判断该键值中是否保存有数据
        {//利用Json将保存的string类型的数据转换回来
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}

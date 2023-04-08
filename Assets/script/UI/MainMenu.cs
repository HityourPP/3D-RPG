using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;
    PlayableDirector director;//获取时间轴负载的组件
    void Awake()
    {//依次获得该对象子对象按钮，按照排序0，1，2，3，0为创建的Text
        newGameBtn=transform.GetChild(1).GetComponent<Button>();
        continueBtn=transform.GetChild(2).GetComponent<Button>();
        quitBtn=transform.GetChild(3).GetComponent<Button>();

        quitBtn.onClick.AddListener(QuitGame);//addListener是用于鼠标，键盘等特殊元素的一些监听
        newGameBtn.onClick.AddListener(PlayTimeline);
        continueBtn.onClick.AddListener(ContinueGame);
        director=FindObjectOfType<PlayableDirector>();//获取组件
        director.stopped += NewGame;//动画播放完后启用newGame函数
    }
    void PlayTimeline()
    {
        director.Play();//播放时间轴中的动画
    }
    void NewGame(PlayableDirector S)//不使用该参数，但是不添加会报错
    {//转换场景
        PlayerPrefs.DeleteAll();//先删除所有保存数据
        SceneController.Instance.TransitionToFirstLevel();//载入场景
    }
    void ContinueGame()
    {//转换场景，读取进度
        if(SaveManager.Instance.SceneName!="")
        SceneController.Instance.TransitionToLoadGame();
    }
    void QuitGame()
    {
        Application.Quit();//退出游戏
        Debug.Log("退出游戏");
    }
}

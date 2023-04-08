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
    PlayableDirector director;//��ȡʱ���Ḻ�ص����
    void Awake()
    {//���λ�øö����Ӷ���ť����������0��1��2��3��0Ϊ������Text
        newGameBtn=transform.GetChild(1).GetComponent<Button>();
        continueBtn=transform.GetChild(2).GetComponent<Button>();
        quitBtn=transform.GetChild(3).GetComponent<Button>();

        quitBtn.onClick.AddListener(QuitGame);//addListener��������꣬���̵�����Ԫ�ص�һЩ����
        newGameBtn.onClick.AddListener(PlayTimeline);
        continueBtn.onClick.AddListener(ContinueGame);
        director=FindObjectOfType<PlayableDirector>();//��ȡ���
        director.stopped += NewGame;//���������������newGame����
    }
    void PlayTimeline()
    {
        director.Play();//����ʱ�����еĶ���
    }
    void NewGame(PlayableDirector S)//��ʹ�øò��������ǲ���ӻᱨ��
    {//ת������
        PlayerPrefs.DeleteAll();//��ɾ�����б�������
        SceneController.Instance.TransitionToFirstLevel();//���볡��
    }
    void ContinueGame()
    {//ת����������ȡ����
        if(SaveManager.Instance.SceneName!="")
        SceneController.Instance.TransitionToLoadGame();
    }
    void QuitGame()
    {
        Application.Quit();//�˳���Ϸ
        Debug.Log("�˳���Ϸ");
    }
}

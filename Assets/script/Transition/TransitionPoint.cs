using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType//创建枚举变量判断是同场景传送还是异场景传送
    {
        SameScene,DifferentScene
    };
    [Header("Transition Info")]
    public string sceneName;//获取场景名字
    public TransitionType transitionType;//定义变量
    public TransitionDestination.DestinationTag destinationTag;
    private bool canTrans;//用于判断是否能传送
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canTrans)//当按下F键并且canTrans为true时实现传送
        {
            //TODO:SceneController传送
            SceneController.Instance.TransitionToDestination(this);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))//在点击过程中使其变为能传送状态
        {
            canTrans = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))//角色离开后变为不可传送
        {
            canTrans = false;
        }
    }
}

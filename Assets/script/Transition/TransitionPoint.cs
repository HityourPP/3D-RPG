using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType//����ö�ٱ����ж���ͬ�������ͻ����쳡������
    {
        SameScene,DifferentScene
    };
    [Header("Transition Info")]
    public string sceneName;//��ȡ��������
    public TransitionType transitionType;//�������
    public TransitionDestination.DestinationTag destinationTag;
    private bool canTrans;//�����ж��Ƿ��ܴ���
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canTrans)//������F������canTransΪtrueʱʵ�ִ���
        {
            //TODO:SceneController����
            SceneController.Instance.TransitionToDestination(this);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))//�ڵ��������ʹ���Ϊ�ܴ���״̬
        {
            canTrans = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))//��ɫ�뿪���Ϊ���ɴ���
        {
            canTrans = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.Events;
[System .Serializable]
//public class EventsVector3 : UnityEvent<Vector3> { };
public class MouseManager : Singleton<MouseManager>//�̳з��͵���ģʽ
{
    public static MouseManager instance;
    public Texture2D point, doorway, attack, target, arrow;//�������ָ�����
    RaycastHit hitInfo;//�������߲���
     //   public EventsVector3 OnMouseClicked;
    public event Action<Vector3> OnMouseClicked;//������������ʱ���ͻᴥ������OnMouseClicked()�Ӷ�ʵ�ֺ�������
    public event Action<GameObject> OnEnemyClicked;//ʵ�ֵ������ʱ�����ú���
    //�̳з��͵�����Ͳ���Ҫ�ú�����
    //void Awake()
    //{
    //    if(instance != null)
    //    {
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        instance = this;    
    //    }
    //}
    //������ת������ʱҲϣ������ģʽ�������ٵģ�����������溯��
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad (this);//����ҲҪ���һЩ�����ķ���
    }
    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    void SetCursorTexture()
    {
        Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);//�������߻�ȡ�������λ��
        if(Physics .Raycast(ray, out hitInfo))
        {
            //�л�����ͼ
            switch(hitInfo .collider .gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.ForceSoftware);//vector2������һ�����ָ���ͼƬƫ����
                    //ͼƬƫ��������Ϊ��ͨ����꣬��ƫ������Ϊ���ͷ�ļ���λ��
                    //��ΪԲ�ε����ָ�룬�Ǿ�Ӧ����ͼƬ������λ��
                    break;
                case "enemy":
                    Cursor.SetCursor(attack , new Vector2(16, 16), CursorMode.ForceSoftware);
                    break;
                case "portal":
                    Cursor.SetCursor(doorway , new Vector2(16, 16), CursorMode.ForceSoftware);
                    break;
                    default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.ForceSoftware);
                    break;
            }
        }
    }
    void MouseControl()
    {
        if(Input.GetMouseButtonDown(0)&&hitInfo.collider != null)//GetMouseButtonDown(0)��ȡ���������
        {
            if(hitInfo.collider.gameObject.CompareTag("Ground"))//�жϵ������ı�ǩ
            {
                OnMouseClicked?.Invoke(hitInfo.point);//��������,����ʺ�ǰΪtrue����ִ���ʺź����
            }
            if (hitInfo.collider.gameObject.CompareTag("enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("attackable"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("portal"))//�жϵ������ı�ǩ
            {
                OnMouseClicked?.Invoke(hitInfo.point);//��������
            }
        }
    }
}

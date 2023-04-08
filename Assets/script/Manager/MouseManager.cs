using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.Events;
[System .Serializable]
//public class EventsVector3 : UnityEvent<Vector3> { };
public class MouseManager : Singleton<MouseManager>//继承泛型单例模式
{
    public static MouseManager instance;
    public Texture2D point, doorway, attack, target, arrow;//创建鼠标指针变量
    RaycastHit hitInfo;//创建射线参数
     //   public EventsVector3 OnMouseClicked;
    public event Action<Vector3> OnMouseClicked;//这样当点击鼠标时，就会触发函数OnMouseClicked()从而实现后续功能
    public event Action<GameObject> OnEnemyClicked;//实现点击敌人时触发该函数
    //继承泛型单例后就不需要该函数了
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
    //但是在转换场景时也希望单例模式不被销毁的，所以添加下面函数
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad (this);//可能也要添加一些其他的方法
    }
    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    void SetCursorTexture()
    {
        Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);//创建射线获取鼠标点击的位置
        if(Physics .Raycast(ray, out hitInfo))
        {
            //切换鼠标截图
            switch(hitInfo .collider .gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.ForceSoftware);//vector2是设置一个鼠标指针的图片偏移量
                    //图片偏移量，若为普通的鼠标，那偏移量就为其箭头的尖尖的位置
                    //若为圆形的鼠标指针，那就应该在图片的中心位置
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
        if(Input.GetMouseButtonDown(0)&&hitInfo.collider != null)//GetMouseButtonDown(0)获取鼠标左键点击
        {
            if(hitInfo.collider.gameObject.CompareTag("Ground"))//判断点击对象的标签
            {
                OnMouseClicked?.Invoke(hitInfo.point);//传送坐标,如果问号前为true，就执行问号后代码
            }
            if (hitInfo.collider.gameObject.CompareTag("enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("attackable"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("portal"))//判断点击对象的标签
            {
                OnMouseClicked?.Invoke(hitInfo.point);//传送坐标
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class ExtensionMethod//扩展方法不会继承其他类，而且是一个静态类
{
    private const float dotThreshold = 0.5f;//由于是静态类，所以无法设置变量，只能将其设置为不可更改的量
    public static bool IsFacingTarget(this Transform transform, Transform target)//this后面的为要扩展的类名，逗号隔开的是函数的变量
    {
        var vectorToTarget=target.position - transform.position;//获取攻击目标的朝向
        vectorToTarget.Normalize();//将朝向量化
        float dot=Vector3.Dot(transform .forward,vectorToTarget);//获取敌人朝向与角色朝向夹角的cos值
        return dot >= dotThreshold;
    }
}

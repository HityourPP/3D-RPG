using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class ExtensionMethod//��չ��������̳������࣬������һ����̬��
{
    private const float dotThreshold = 0.5f;//�����Ǿ�̬�࣬�����޷����ñ�����ֻ�ܽ�������Ϊ���ɸ��ĵ���
    public static bool IsFacingTarget(this Transform transform, Transform target)//this�����ΪҪ��չ�����������Ÿ������Ǻ����ı���
    {
        var vectorToTarget=target.position - transform.position;//��ȡ����Ŀ��ĳ���
        vectorToTarget.Normalize();//����������
        float dot=Vector3.Dot(transform .forward,vectorToTarget);//��ȡ���˳������ɫ����нǵ�cosֵ
        return dot >= dotThreshold;
    }
}

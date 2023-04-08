using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Grunt : enemycontroller
{
    [Header("Skill")]
    public float kickForce = 10;//����һ���ƿ�����
    public void KickOff()
    {
        if (attackTarget!=null)
        {//����Ҫ������enemycontroller�����е�attackTarget������private��Ϊprotected������������ܵ���
            transform .LookAt(attackTarget.transform);//����ʱʹ�������ɫ
            Vector3 direction=attackTarget.transform.position-transform.position;//������¼��Ҫ�ƿ��ķ���
            direction.Normalize();//���䷽���������������Ϊ1��0��-1��ע�����ô�д��Normalize��Сд��Ϊ��ȡ�����ֵ
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//���ƿ�ʱ����ɫ�������ƶ�����Ҫ����ͣ���ƶ���������
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;//�����ƿ�����
            attackTarget.GetComponent<Animator>().SetTrigger("dizzy");//����ѣ��Ч��
        }
        
    }
}

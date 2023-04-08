using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Golem : enemycontroller//�̳�enemycontroller��
{
    [Header("Skill")]
    public float kickForce = 25;//����һ�����ɵ���
    public GameObject rockPrefab;//��ȡʯͷ����
    public Transform handPos;//��ȡʯͷ��Ͷʯʱ���ֵ�λ��
    //Animation Event
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        //�����ж��˹��������Ƿ�Ϊ��,�����жϽ�ɫ�Ƿ�������
        {
            var targentStats = attackTarget.GetComponent<CharacterStats>();//��ȡ����Ŀ�����ֵ
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            //direction.normalized;
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//���ƿ�ʱ����ɫ�������ƶ�����Ҫ����ͣ���ƶ���������
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;//�����ƿ�����
            //ѣ��Ч�����ݸ���ϲ�����
            //attackTarget.GetComponent<Animator>().SetTrigger("dizzy");//����ѣ��Ч��
            targentStats.TakeDamage(characterStats, targentStats);//���ù�������,ͬʱΪ���ܵ���characterStats���ڸ����н����Ϊprotected����
        }
    }
    //Animation Event
    public void ThrowRock()
    {
        if(attackTarget != null)
        {//���ֲ���λ������һ��ʯͷ��Quaternion.identityΪ�������ʼ����ת�Ƕ�
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target= attackTarget;//ʯͷ��ȡ��������
        }
    }
}

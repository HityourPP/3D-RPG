using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer,HitEnemy,HitNothing}//ʯͷ�Ķ��״̬
    private Rigidbody rb;//�����������
    public RockStates rockstate;
    [Header("Basic Setting")]
    public float force;//���׳�������
    public GameObject target;//��ȡ�׳���Ŀ��
    private Vector3 direction;//��ȡ�׳�Ŀ���λ��
    public int damage;//��¼ʯͷ��ɵ��˺�
    public GameObject breakEffect;
    void Start()
    {
        rb = GetComponent<Rigidbody>();//��ȡ�������
        rb.velocity = Vector3.one;//Ϊ�˱���ճ���ʯͷʱ�ٶ�Ϊ0�ͱ�ת��״̬
        rockstate=RockStates.HitPlayer;//��ʼ��״̬       
        FlyToTarget();
    }
    void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)//sqrMagnitude�ܸ���Ľ���ת��Ϊfloat��
        {
            rockstate = RockStates.HitNothing;//ת��״̬
        }
            //Debug.Log(rb.velocity.sqrMagnitude);
    }
    public void FlyToTarget()
    {//��ȡĿ��λ�ã�Vector3������ʯͷ���ھ���ʯͷʱ�������ϵķ��ȣ�����ʹʯͷ�����Ϸ��γ�һ������
        if (target == null)
            target = FindObjectOfType<playercontraller>().gameObject;//����ɫ��սʱ�ֲ���ʯͷ���������Լ����򵽽�ɫ
        direction = (target.transform.position- transform.position+Vector3.up).normalized;
        rb.AddForce(direction*force,ForceMode.Impulse);//������ȣ�ForceMode.ImpulseΪ���ķ�ʽΪ�������������������
    }
    public void OnCollisionEnter(Collision collision)//���������ײ���Ķ���
    {
        switch (rockstate)
        {
            case RockStates.HitPlayer:
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;//ֹͣ���ƶ�
                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;//����ɫ����
                    collision.gameObject.GetComponent<Animator>().SetTrigger("dizzy");//ʵ��ѣ��Ч��
                    //����˺�
                    collision.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, collision.gameObject.GetComponent<CharacterStats>());
                    //��������ת��״̬
                    rockstate = RockStates.HitNothing;
                }
                break   ;
            case RockStates.HitEnemy:
                if (collision.gameObject.GetComponent<Golem>())//���ﷴ������ֻ�����ʯͷ�ˣ��ж϶����Ƿ���ʯͷ�˵ĺ���
                {
                    var otherStats = collision.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);//����˺�
                    Instantiate(breakEffect, transform.position, Quaternion.identity);//����ʧ������Ч��
                    Destroy(gameObject);//�ݻ�ʯͷ
                }
                break;
            case RockStates.HitNothing:
                break;
        }
    }
}

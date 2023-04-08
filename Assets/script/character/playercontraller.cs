using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class playercontraller : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject attackTarget;
    private float attackTime;//���ƹ���Ƶ��
    private CharacterStats CharacterStats;
    private bool isDead;
    private float stopDistance;//��¼��ʼֹͣ����
    void Awake()//���齫һЩ�����ĸ�ֵ������������ȼ���������start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        CharacterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;//��ȡ��ʼ��ֹͣ����
    }
    void OnEnable()
    {//����ʱ�������ƶ�����ע��
        MouseManager.Instance.OnMouseClicked += MoveToTarget;//�¼��ĵ��ø�ʽ���൱��ί�е�ʹ�ã�
        MouseManager .Instance .OnEnemyClicked +=EventAttack;
        GameManager.Instance.RegisterPlayer(CharacterStats);//��ȡ��ɫ״̬���ڽ�ɫ����ʱ��ȡ
    }
    void Start()
    {
        // MouseManager.Instance.OnMouseClicked += MoveToTarget;//�¼��ĵ��ø�ʽ���൱��ί�е�ʹ�ã�
        //MouseManager .Instance .OnEnemyClicked +=EventAttack;
        SaveManager.Instance.LoadPlayerData();//��ɫ����ʱ���������볡��

    }
    private void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;//
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;//������ʱȡ������
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }
    void Update()
    {
        isDead = CharacterStats.CurrentHealth == 0;//�ж�Ѫ���Ƿ�Ϊ0
        if (isDead)
            GameManager.Instance.NoitifyObserver();//��ɫ����ʱ���й㲥��֪ͨ���й���IEndGameObserver�ӿڵĴ��뺯��
        SwitchAnim();
        attackTime -= Time.deltaTime;//������ȴʱ���˥��
    }
    private void SwitchAnim()
    {
        anim.SetFloat("speed", agent.velocity.sqrMagnitude);//sqrMagnitude����ת��Ϊ������
        anim.SetBool("death",isDead);
    }
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();  //��Ͻ�ɫ�Ĺ���
        if(isDead)return;
        agent.stoppingDistance=stopDistance;//����ɫ�ƶ�ʱֹͣ���뷵�ص�Ĭ��ֵ
        agent.isStopped = false; 
        agent.destination = target;
    }
    private void EventAttack(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            CharacterStats.isCritical = Random.value < CharacterStats.attackData.criticalChance;
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget());
        }
    }
    IEnumerator MoveToAttackTarget()//��������Э�̷���
    {
        agent.isStopped = false;
        agent.stoppingDistance = CharacterStats.attackData.attackRange;//����ɫֹͣ�����빥����Χ����
        transform.LookAt(attackTarget.transform);//����ɫת�򹥻�Ŀ��
        //�жϾ������Զ������Զ�����ƶ�������������������ֱ�ӹ���
        while (Vector3 .Distance (attackTarget .transform.position,transform .position) > CharacterStats.attackData.attackRange)
        {//�жϵ������ɫ�����빥����Χ��Ƚϣ����﹥����ΧԤ��Ϊ1,֮����Ҫ�޸�
            agent.destination = attackTarget.transform.position; //��ɫ������ƶ�
            yield return null;//ʵ������һ֡����ִ�и�����  
        }
        agent.isStopped = true;//isStoppedΪtrueʱ��ɫֹͣ�ƶ�
        //ʵ�ֹ���
        if(attackTime < 0)
        {//��ɫ��������
            //CharacterStats.isCritical = Random.value < CharacterStats.attackData.criticalChance;
            //�ɽ�����������ӵ�EventAttack������
            anim.SetBool("critical", CharacterStats.isCritical);//��ӱ�������
            anim.SetTrigger("attack");
            attackTime = CharacterStats.attackData.coolDown;//������ȴʱ��
        }
    }
    //Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("attackable")){
            if (attackTarget.GetComponent<Rock>())//�ж��Ƿ�Ϊʯͷ
           // && attackTarget.GetComponent<Rock>().rockstate == Rock.RockStates.HitNothing����Ӹ����ʹ��ֻ����HitNothing״̬������������ѡ��
            {//�ı�ʯͷ״̬������Ҫ��rockstate��Ϊpublic����
                attackTarget.GetComponent<Rock>().rockstate = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;//������״̬ת��ΪHitNothing
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward*20,ForceMode.Impulse);//��ɫ����ʯͷ
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();//��ȡ����Ŀ�����ֵ
            targetStats.TakeDamage(CharacterStats, targetStats);//���ù�������
        }
        

    }
}

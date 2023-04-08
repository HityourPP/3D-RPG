using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyStates{ Guard,Patrol,Chase,Dead}//ö�ٷ�ʽ�������˵Ķ���״̬
[RequireComponent(typeof(NavMeshAgent))]//�������ļ���ӵ�ĳһ����ʱ���Զ�Ϊ�����NavMeshAgent���
[RequireComponent(typeof(CharacterStats))]//�Զ�Ϊ�����characterStats���
public class enemycontroller : MonoBehaviour,IEndGameObserver
{
    private NavMeshAgent agent;
    private EnemyStates enemystates;
    private Animator anim;
    protected CharacterStats characterStats;
    private Collider coll;
    [Header("Basic Setting")]//�ô���������Inspector����ϸ�֮�����puclic������һ������
    public float sightRadius;//���˵�׷����Χ
    public bool isGuard;//�жϵ������ͣ��Ƿ�Ϊվ׮��
    private float speed;//��¼����ԭ�����ٶ�
    protected  GameObject attackTarget;
    public float lookAtTime;//Ѳ����ֹͣʱ��
    private float remainLookAtTime;
    private float lastAttackTime;//������ȴʱ��
    private Quaternion guardRotation;//��¼�䳯����ת�Ƕ�
    //[Header ("Patrol State")]
    [Header("Ѳ��״̬")]
    public float patrolRange;//Ѳ�߷�Χ����
    private Vector3 wayPoint;//Ѳ�߷�Χ��������ֵ�Ѳ�ߵ�
    private Vector3 guardPos;//��ȡ���ʼλ��
    //��϶���
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        speed =agent.speed;
        anim = GetComponent<Animator>();   
        guardPos = transform.position;
        remainLookAtTime = lookAtTime;
        characterStats = GetComponent<CharacterStats>();
        guardRotation = transform.rotation;//��ȡ��ԭ����ת��Ƕ�
        coll=GetComponent<Collider>();//��ȡ����ײ��
    }
    void Start()
    {
        if (isGuard)//����Ϸ��ʼʱ�ж���Ϊ��������
        {
            enemystates = EnemyStates.Guard;
        }
        else
        {
            enemystates = EnemyStates.Patrol;
            GetNewWayPoint();//ΪwayPoint����ʼֵ
        }
        //FIXME:�����л����޸ĵ�
        GameManager.Instance.AddObserver(this);
    }
    //�л�����ʱ����
    //void OnEnable()//���������ӵ��۲����б���
    //{
    //    GameManager.Instance.AddObserver(this);
    //}
    void OnDisable() {
        if(!GameManager.IsInitialized)return;
        GameManager.Instance.RemoveObserver(this);
    }
    void Update()
    {
        if (characterStats.CurrentHealth == 0)
        {
            isDead = true;
        }
        SwitchStates();
        SwitchAnimation();
        lastAttackTime-=Time.deltaTime;
    }
    void SwitchAnimation()
    {
        anim.SetBool("walk", isWalk);
        anim.SetBool("chase", isChase);
        anim.SetBool("follow", isFollow);
        anim.SetBool("critical", characterStats.isCritical);
        anim.SetBool("death",isDead);
    }
    void SwitchStates()
   {
        if (isDead)//�л�������״̬
        {
            enemystates = EnemyStates.Dead;
        }
        //������ֽ�ɫ���л���׷��״̬Chase
        else if(FindPlayer())
        {
            enemystates = EnemyStates.Chase;
        }
        switch (enemystates)
        {
            case EnemyStates.Guard:
                Guard();
                break;
            case EnemyStates.Patrol:
                Patrol();
                break;
            case EnemyStates.Chase:
                Chase();
                break;
            case EnemyStates.Dead:
                Dead();
                break;
        }
   }
    void Guard()
    {
        isChase = false;
        if (transform.position != guardPos)
        {//�ж��Ƿ�λ����վ׮��ԭ����λ��
        //�������ߵ�ԭ����λ��
            isWalk = true;
            agent.isStopped = false;
            agent.destination = guardPos;
            if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
            {//SqrMagnitude��Distanceһ�����ж϶��߼�ľ��룬��ǰ�߿�����С
             //stoppingDistanceΪ�ö���Nav Mesh Agent����е�ֹͣ����
             isWalk=false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                //Lerp()����Ϊԭ���Ƕȣ�Ҫת��ĽǶȣ��Լ�ת��ʱ�䣬ʱ��Ϊ0.01~1��0.01Ϊ0.01Ϊ�ɸı���ֵ
                //����1ֱ��ת�䣬����Ҫʵ�ֽ����Ч��
            }
        }
    }
    void Patrol()
    {
        isChase = false;
        agent.speed = speed * 0.5f;//ʹѲ��ʱ�ٶ�С��׷��ʱ�ٶȣ���˷����бȳ�������Щ
       //�ж��Ƿ��ߵ���Ѳ�ߵ�
        if(Vector3.Distance(wayPoint,transform.position) <= agent.stoppingDistance)
        {//Distance�����ܼ�������߾��룬stoppingDistanceΪ�ö���Nav Mesh Agent����е�ֹͣ����
            isWalk = false;
            if (remainLookAtTime > 0)//�ߵ�Ѳ�ߵ�����һ��
            {
                remainLookAtTime -= Time.deltaTime;
            }else
            GetNewWayPoint();//�ߵ�Ѳ�ߵ�֮��Ѱ����һ��Ѳ�ߵ�
        }
        else
        {
            isWalk =true;
            agent.destination=wayPoint;
        }
    }
    void Chase()
    {
        isWalk = false;
        isChase = true;
        agent.speed = speed;//��׷��ʱ����Ϊ�����ٶȣ�Ѳ���ƶ�ʱ�ٶȿ���СЩ
        //1��׷����ɫ 2�����ѻص���һ��״̬ 3���ڹ�����Χ�ڹ��� 4����Ӧ����
        if (!FindPlayer())
        {
            //���ѻص���һ��״̬
            isFollow = false;
            if (remainLookAtTime > 0)
            {
                agent.destination = transform.position;//������սʱͣ��ԭ��һ��
                remainLookAtTime -= Time.deltaTime;
            }//ֹͣ�󷵻ص�ԭ��״̬
            else if (isGuard)
                enemystates = EnemyStates.Guard;
            else if (!isGuard)
                enemystates = EnemyStates.Patrol;
        }
        else
        {
            //׷����ɫ
            isFollow = true;
            agent.isStopped = false;//����������Ϊtrue������תΪfalse�������ƶ�
            agent.destination = attackTarget.transform.position;
        }
        //�ڹ�����Χ���򹥻�
        if (TargetInAttackRange() || TargetInSkillRange())
        {
            isFollow=false;
            agent.isStopped = true;
            if (lastAttackTime < 0)
            {
                lastAttackTime = characterStats.attackData.coolDown;
                //�����ж�
                characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                //ִ�й���
                Attack();
            }
        }
    }
    void Dead()
    {
        coll.enabled = false;//����collider�ص�ʹ��ɫ�޷��ٹ�������
       // agent.enabled = false;//�������������Ϊ���ص�
        agent.radius = 0;//���ｫagent��Χ��СΪ0ʵ�ֺ���һ�д�����ͬ��Ч��
        Destroy(gameObject,2f);//2������ٵ���
    }
    bool TargetInAttackRange()//�ж��Ƿ��ڽ�ս������Χ��
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }
    bool TargetInSkillRange()//�ж��Ƿ���Զ�̹�����Χ��
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }
    void Attack()
    {
        transform.LookAt(attackTarget.transform);//���������ɫ
        if (TargetInAttackRange())
        {
            //����������
            anim.SetTrigger("attack");
        }
        if (TargetInSkillRange())
        {
            //���ܹ�������
            anim.SetTrigger("skill");
        }
    }
    bool FindPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);//�ڵ��˰뾶��Χ��Ѱ�ҽ�ɫ
        foreach (var target in colliders)//foreach��һѭ��
        {
            if(target .CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        return false;
    }
    void GetNewWayPoint()//��ȡ�����Ѳ�ߵ�
    {
        remainLookAtTime = lookAtTime;
        float randomX=Random.Range(-patrolRange ,patrolRange);
        float randomZ=Random.Range(-patrolRange ,patrolRange);
        Vector3 randomPoint =new Vector3(guardPos.x+randomX , transform.position.y, guardPos.z+randomZ);
        NavMeshHit hit;
        //if(NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1))//�ж�����������Ƿ�Ϊ�ɵ���ģ�1Ϊwalkable
        //{
        //    wayPoint = hit.position;
        //}
        //else
        //{
        //    wayPoint = transform.position;
        //} 
        //��������ʽ���ɣ����濴�Ÿ����Щ
        wayPoint= NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1)?hit.position:transform.position;
    }
    void OnDrawGizmosSelected()//Ϊ���ڳ����л������˵�Ѳ�߷�Χ
    {
        Gizmos.color = Color.red;
        Gizmos .DrawWireSphere(transform .position , sightRadius);
    }
    //Animation Event
    void Hit()
    {
        if(attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        //������ж��˹��������Ƿ�Ϊ��,�����жϽ�ɫ�Ƿ�������
        {
            var targentStats = attackTarget.GetComponent<CharacterStats>();//��ȡ����Ŀ�����ֵ
            targentStats.TakeDamage(characterStats, targentStats);//���ù�������
        }
    }
    public void EndNotify()//����ʵ�ֽӿ��еĺ���������Ϸ�Ĺ㲥
    {
        //��ʤ����
        anim.SetBool("win", true);
        //ֹͣ����
        isChase = false;
        isWalk = false;
        //ֹͣ�����ƶ�
        //ֹͣAgent
        attackTarget = null;
    }
}

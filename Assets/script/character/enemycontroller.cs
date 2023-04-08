using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyStates{ Guard,Patrol,Chase,Dead}//枚举方式创建敌人的多种状态
[RequireComponent(typeof(NavMeshAgent))]//当代码文件添加到某一对象时，自动为其添加NavMeshAgent组件
[RequireComponent(typeof(CharacterStats))]//自动为其添加characterStats组件
public class enemycontroller : MonoBehaviour,IEndGameObserver
{
    private NavMeshAgent agent;
    private EnemyStates enemystates;
    private Animator anim;
    protected CharacterStats characterStats;
    private Collider coll;
    [Header("Basic Setting")]//该代码用于在Inspector面板上给之后定义的puclic变量加一个标题
    public float sightRadius;//敌人的追击范围
    public bool isGuard;//判断敌人类型，是否为站桩怪
    private float speed;//记录自身原来的速度
    protected  GameObject attackTarget;
    public float lookAtTime;//巡逻中停止时间
    private float remainLookAtTime;
    private float lastAttackTime;//攻击冷却时间
    private Quaternion guardRotation;//记录其朝向，旋转角度
    //[Header ("Patrol State")]
    [Header("巡逻状态")]
    public float patrolRange;//巡逻范围参数
    private Vector3 wayPoint;//巡逻范围内随机出现的巡逻点
    private Vector3 guardPos;//获取其初始位置
    //配合动画
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
        guardRotation = transform.rotation;//获取其原来的转向角度
        coll=GetComponent<Collider>();//获取该碰撞体
    }
    void Start()
    {
        if (isGuard)//在游戏开始时判断其为何种类型
        {
            enemystates = EnemyStates.Guard;
        }
        else
        {
            enemystates = EnemyStates.Patrol;
            GetNewWayPoint();//为wayPoint赋初始值
        }
        //FIXME:场景切换后修改掉
        GameManager.Instance.AddObserver(this);
    }
    //切换场景时启用
    //void OnEnable()//具体调用添加到观察者列表中
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
        if (isDead)//切换到死亡状态
        {
            enemystates = EnemyStates.Dead;
        }
        //如果发现角色，切换到追击状态Chase
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
        {//判断是否位于其站桩的原来的位置
        //若否，则走到原来的位置
            isWalk = true;
            agent.isStopped = false;
            agent.destination = guardPos;
            if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
            {//SqrMagnitude与Distance一样能判断二者间的距离，但前者开销更小
             //stoppingDistance为该对象Nav Mesh Agent组件中的停止距离
             isWalk=false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                //Lerp()里面为原来角度，要转变的角度，以及转向时间，时间为0.01~1，0.01为0.01为可改变数值
                //大于1直接转变，这里要实现渐变的效果
            }
        }
    }
    void Patrol()
    {
        isChase = false;
        agent.speed = speed * 0.5f;//使巡逻时速度小于追击时速度，另乘法运行比除法更快些
       //判断是否走到了巡逻点
        if(Vector3.Distance(wayPoint,transform.position) <= agent.stoppingDistance)
        {//Distance函数能计算出二者距离，stoppingDistance为该对象Nav Mesh Agent组件中的停止距离
            isWalk = false;
            if (remainLookAtTime > 0)//走到巡逻点后观望一会
            {
                remainLookAtTime -= Time.deltaTime;
            }else
            GetNewWayPoint();//走到巡逻点之后寻找下一个巡逻点
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
        agent.speed = speed;//当追击时设置为正常速度，巡逻移动时速度可设小些
        //1，追击角色 2，拉脱回到上一个状态 3，在攻击范围内攻击 4，对应动画
        if (!FindPlayer())
        {
            //拉脱回到上一个状态
            isFollow = false;
            if (remainLookAtTime > 0)
            {
                agent.destination = transform.position;//让其脱战时停在原地一会
                remainLookAtTime -= Time.deltaTime;
            }//停止后返回到原来状态
            else if (isGuard)
                enemystates = EnemyStates.Guard;
            else if (!isGuard)
                enemystates = EnemyStates.Patrol;
        }
        else
        {
            //追击角色
            isFollow = true;
            agent.isStopped = false;//攻击后将其设为true，这里转为false才能再移动
            agent.destination = attackTarget.transform.position;
        }
        //在攻击范围内则攻击
        if (TargetInAttackRange() || TargetInSkillRange())
        {
            isFollow=false;
            agent.isStopped = true;
            if (lastAttackTime < 0)
            {
                lastAttackTime = characterStats.attackData.coolDown;
                //暴击判断
                characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                //执行攻击
                Attack();
            }
        }
    }
    void Dead()
    {
        coll.enabled = false;//将其collider关掉使角色无法再攻击它了
       // agent.enabled = false;//将其相关其他行为都关掉
        agent.radius = 0;//这里将agent范围缩小为0实现和上一行代码相同的效果
        Destroy(gameObject,2f);//2秒后销毁敌人
    }
    bool TargetInAttackRange()//判断是否在近战攻击范围内
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }
    bool TargetInSkillRange()//判断是否在远程攻击范围内
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }
    void Attack()
    {
        transform.LookAt(attackTarget.transform);//将其面向角色
        if (TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("attack");
        }
        if (TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("skill");
        }
    }
    bool FindPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);//在敌人半径范围内寻找角色
        foreach (var target in colliders)//foreach逐一循环
        {
            if(target .CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        return false;
    }
    void GetNewWayPoint()//获取随机的巡逻点
    {
        remainLookAtTime = lookAtTime;
        float randomX=Random.Range(-patrolRange ,patrolRange);
        float randomZ=Random.Range(-patrolRange ,patrolRange);
        Vector3 randomPoint =new Vector3(guardPos.x+randomX , transform.position.y, guardPos.z+randomZ);
        NavMeshHit hit;
        //if(NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1))//判断所表随机点是否为可到达的，1为walkable
        //{
        //    wayPoint = hit.position;
        //}
        //else
        //{
        //    wayPoint = transform.position;
        //} 
        //这两种形式都可，下面看着更简便些
        wayPoint= NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1)?hit.position:transform.position;
    }
    void OnDrawGizmosSelected()//为了在场景中画出敌人的巡逻范围
    {
        Gizmos.color = Color.red;
        Gizmos .DrawWireSphere(transform .position , sightRadius);
    }
    //Animation Event
    void Hit()
    {
        if(attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        //这里多判断了攻击对象是否为空,并且判断角色是否在正面
        {
            var targentStats = attackTarget.GetComponent<CharacterStats>();//获取攻击目标的数值
            targentStats.TakeDamage(characterStats, targentStats);//调用攻击函数
        }
    }
    public void EndNotify()//用来实现接口中的函数结束游戏的广播
    {
        //获胜动画
        anim.SetBool("win", true);
        //停止动画
        isChase = false;
        isWalk = false;
        //停止所有移动
        //停止Agent
        attackTarget = null;
    }
}

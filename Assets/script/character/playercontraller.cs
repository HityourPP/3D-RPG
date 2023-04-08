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
    private float attackTime;//控制攻击频率
    private CharacterStats CharacterStats;
    private bool isDead;
    private float stopDistance;//记录初始停止距离
    void Awake()//建议将一些变量的赋值放在这里，其优先级甚至高于start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        CharacterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;//获取初始的停止距离
    }
    void OnEnable()
    {//启用时对下面移动进行注册
        MouseManager.Instance.OnMouseClicked += MoveToTarget;//事件的调用格式（相当于委托的使用）
        MouseManager .Instance .OnEnemyClicked +=EventAttack;
        GameManager.Instance.RegisterPlayer(CharacterStats);//获取角色状态，在角色生成时获取
    }
    void Start()
    {
        // MouseManager.Instance.OnMouseClicked += MoveToTarget;//事件的调用格式（相当于委托的使用）
        //MouseManager .Instance .OnEnemyClicked +=EventAttack;
        SaveManager.Instance.LoadPlayerData();//角色生成时载入数据与场景

    }
    private void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;//
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;//不可用时取消订阅
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }
    void Update()
    {
        isDead = CharacterStats.CurrentHealth == 0;//判断血量是否为0
        if (isDead)
            GameManager.Instance.NoitifyObserver();//角色死亡时进行广播，通知所有挂载IEndGameObserver接口的代码函数
        SwitchAnim();
        attackTime -= Time.deltaTime;//攻击冷却时间的衰减
    }
    private void SwitchAnim()
    {
        anim.SetFloat("speed", agent.velocity.sqrMagnitude);//sqrMagnitude将其转化为浮点型
        anim.SetBool("death",isDead);
    }
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();  //打断角色的攻击
        if(isDead)return;
        agent.stoppingDistance=stopDistance;//当角色移动时停止距离返回到默认值
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
    IEnumerator MoveToAttackTarget()//这里利用协程方法
    {
        agent.isStopped = false;
        agent.stoppingDistance = CharacterStats.attackData.attackRange;//将角色停止距离与攻击范围适配
        transform.LookAt(attackTarget.transform);//将角色转向攻击目标
        //判断距离敌人远近，若远，则移动过来攻击，若近，则直接攻击
        while (Vector3 .Distance (attackTarget .transform.position,transform .position) > CharacterStats.attackData.attackRange)
        {//判断敌人与角色距离与攻击范围相比较，这里攻击范围预设为1,之后需要修改
            agent.destination = attackTarget.transform.position; //角色向敌人移动
            yield return null;//实现在下一帧继续执行该命令  
        }
        agent.isStopped = true;//isStopped为true时角色停止移动
        //实现攻击
        if(attackTime < 0)
        {//角色攻击动画
            //CharacterStats.isCritical = Random.value < CharacterStats.attackData.criticalChance;
            //可将上述代码添加到EventAttack函数中
            anim.SetBool("critical", CharacterStats.isCritical);//添加暴击概率
            anim.SetTrigger("attack");
            attackTime = CharacterStats.attackData.coolDown;//重置冷却时间
        }
    }
    //Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("attackable")){
            if (attackTarget.GetComponent<Rock>())//判断是否为石头
           // && attackTarget.GetComponent<Rock>().rockstate == Rock.RockStates.HitNothing可添加该语句使其只能在HitNothing状态反击，可自由选择
            {//改变石头状态，这里要将rockstate改为public类型
                attackTarget.GetComponent<Rock>().rockstate = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;//避免其状态转换为HitNothing
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward*20,ForceMode.Impulse);//角色击退石头
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();//获取攻击目标的数值
            targetStats.TakeDamage(CharacterStats, targetStats);//调用攻击函数
        }
        

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer,HitEnemy,HitNothing}//石头的多个状态
    private Rigidbody rb;//创建刚体变量
    public RockStates rockstate;
    [Header("Basic Setting")]
    public float force;//被抛出的力度
    public GameObject target;//获取抛出的目标
    private Vector3 direction;//获取抛出目标的位置
    public int damage;//记录石头造成的伤害
    public GameObject breakEffect;
    void Start()
    {
        rb = GetComponent<Rigidbody>();//获取对象刚体
        rb.velocity = Vector3.one;//为了避免刚出现石头时速度为0就被转换状态
        rockstate=RockStates.HitPlayer;//初始化状态       
        FlyToTarget();
    }
    void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)//sqrMagnitude能更快的将其转化为float型
        {
            rockstate = RockStates.HitNothing;//转换状态
        }
            //Debug.Log(rb.velocity.sqrMagnitude);
    }
    public void FlyToTarget()
    {//获取目标位置，Vector3是由于石头人在举起石头时会有向上的幅度，所以使石头先向上飞形成一定弧度
        if (target == null)
            target = FindObjectOfType<playercontraller>().gameObject;//当角色脱战时又产生石头，这样可以继续打到角色
        direction = (target.transform.position- transform.position+Vector3.up).normalized;
        rb.AddForce(direction*force,ForceMode.Impulse);//添加力度，ForceMode.Impulse为力的方式为冲击力，即立即将其冲出
    }
    public void OnCollisionEnter(Collision collision)//用来获得碰撞到的对象
    {
        switch (rockstate)
        {
            case RockStates.HitPlayer:
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;//停止其移动
                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;//将角色击退
                    collision.gameObject.GetComponent<Animator>().SetTrigger("dizzy");//实现眩晕效果
                    //造成伤害
                    collision.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, collision.gameObject.GetComponent<CharacterStats>());
                    //攻击过后转换状态
                    rockstate = RockStates.HitNothing;
                }
                break   ;
            case RockStates.HitEnemy:
                if (collision.gameObject.GetComponent<Golem>())//这里反击敌人只针对于石头人，判断对象是否含有石头人的函数
                {
                    var otherStats = collision.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);//造成伤害
                    Instantiate(breakEffect, transform.position, Quaternion.identity);//在消失处出现效果
                    Destroy(gameObject);//摧毁石头
                }
                break;
            case RockStates.HitNothing:
                break;
        }
    }
}

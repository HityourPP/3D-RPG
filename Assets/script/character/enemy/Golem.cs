using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Golem : enemycontroller//继承enemycontroller类
{
    [Header("Skill")]
    public float kickForce = 25;//创建一个击飞的力
    public GameObject rockPrefab;//获取石头对象
    public Transform handPos;//获取石头人投石时举手的位置
    //Animation Event
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        //这里判断了攻击对象是否为空,并且判断角色是否在正面
        {
            var targentStats = attackTarget.GetComponent<CharacterStats>();//获取攻击目标的数值
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            //direction.normalized;
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//当推开时，角色可能在移动这是要先暂停其移动，将其打断
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;//设置推开距离
            //眩晕效果根据个人喜好添加
            //attackTarget.GetComponent<Animator>().SetTrigger("dizzy");//设置眩晕效果
            targentStats.TakeDamage(characterStats, targentStats);//调用攻击函数,同时为了能调用characterStats，在父类中将其改为protected类型
        }
    }
    //Animation Event
    public void ThrowRock()
    {
        if(attackTarget != null)
        {//在手部的位置生成一个石头，Quaternion.identity为设置其初始的旋转角度
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target= attackTarget;//石头获取攻击对象
        }
    }
}

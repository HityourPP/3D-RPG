using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Grunt : enemycontroller
{
    [Header("Skill")]
    public float kickForce = 10;//创建一个推开的力
    public void KickOff()
    {
        if (attackTarget!=null)
        {//这里要将父类enemycontroller代码中的attackTarget参数由private改为protected，这样子类才能调用
            transform .LookAt(attackTarget.transform);//攻击时使其面向角色
            Vector3 direction=attackTarget.transform.position-transform.position;//用来记录其要推开的方向
            direction.Normalize();//将其方向进行量化，即化为1，0，-1，注意是用大写的Normalize，小写的为获取其参数值
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//当推开时，角色可能在移动这是要先暂停其移动，将其打断
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;//设置推开距离
            attackTarget.GetComponent<Animator>().SetTrigger("dizzy");//设置眩晕效果
        }
        
    }
}

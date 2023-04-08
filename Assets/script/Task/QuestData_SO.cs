using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]//实现类的可序列化
    public class RequireQuest
    {
        public string name;//需要的事物的名字
        public int requireAmount;
        public int currentAmount;
    }
    public string questName;//任务名字
    [TextArea]
    public string description;

    public bool isStarted;
    public bool isCompleted;
    public bool isFinished;

    public List<RequireQuest> requireQuests = new List<RequireQuest>();//建立任务列表
}

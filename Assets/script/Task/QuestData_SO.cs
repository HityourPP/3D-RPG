using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]//ʵ����Ŀ����л�
    public class RequireQuest
    {
        public string name;//��Ҫ�����������
        public int requireAmount;
        public int currentAmount;
    }
    public string questName;//��������
    [TextArea]
    public string description;

    public bool isStarted;
    public bool isCompleted;
    public bool isFinished;

    public List<RequireQuest> requireQuests = new List<RequireQuest>();//���������б�
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItem
{
    public int ItemId;
    public int itemCount;
}

public class QuestData
{
    public int id;
    public string titleName;
    public Define.QuestType questType;
    public int minLevel;
    public int targetId;
    public int targetCount;
    public int rewardGold;
    public int rewardExp;
    public List<RewardItem> rewardItems;
    public string description;
    public string targetDescription;

    public bool isAccept = false;   // 수락 상태
    public bool isClear = false;    // 클리어 상태

    // 퀘스트 초기화 
    public void Clear()
    {
        isAccept = false;
        isClear = false;
        
        targetCount = 0;
    }
}

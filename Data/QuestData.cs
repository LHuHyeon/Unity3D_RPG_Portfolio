using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardItem
{
    public int ItemId;
    public int itemCount;
}

[Serializable]
public class QuestData
{
    public int id;
    public string titleName;
    public Define.QuestType questType;
    public int minLevel;
    public int targetId;
    public int targetCount;
    public int currnetTargetCount;
    public int rewardGold;
    public int rewardExp;
    public List<RewardItem> rewardItems;
    public string description;
    public string targetDescription;

    public bool isAccept = false;   // 수락 상태
    public bool isClear = false;    // 클리어 상태

    // 퀘스트 성공
    public void QuestClear()
    {
        isClear = true;

        // 보상 지급
        foreach(RewardItem rewardItem in rewardItems)
            Managers.Game._playScene._inventory.AcquireItem(Managers.Data.CallItem(rewardItem.ItemId), rewardItem.itemCount);

        Managers.Game.Gold += rewardGold;
        Managers.Game.Exp += rewardExp;
    }
}

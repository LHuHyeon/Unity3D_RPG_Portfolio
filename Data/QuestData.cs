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
}

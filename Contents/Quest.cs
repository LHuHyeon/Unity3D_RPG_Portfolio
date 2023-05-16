using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reward
{
    public ItemData item;       // 또는 id 사용
    public int itemCount;
}

public class TargetID
{
    public int id;              // 오브젝트 번호
    public int maxTargetValue;  // 목표 개수
    public int achieveValue;    // 달성한 개수
}

public class Quest
{
    public string title;        // 퀘스트 제목

    [TextArea]
    public string description;  // 내용 설명

    [TextArea]
    public string achieveDesc;  // 목표 설명

    // 목표 리스트
    public List<TargetID> targetQuest = new List<TargetID>();

    // 아이템 보상
    public List<Reward> itemReward = new List<Reward>();
    public int gold;            // 골드 보상
    public int exp;             // 경험치 보상

    public bool isAccept = false;   // 수락 상태
    public bool isClear = false;    // 클리어 상태

    // 퀘스트 초기화 
    public void Clear()
    {
        isAccept = false;
        isClear = false;
        
        for(int i=0; i<targetQuest.Count; i++)
            targetQuest[i].achieveValue = 0;
    }
}

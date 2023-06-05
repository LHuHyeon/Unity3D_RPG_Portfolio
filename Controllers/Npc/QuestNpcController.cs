using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNpcController : NpcController
{
    public Define.QuestType questType = Define.QuestType.Unknown;

    List<int> questIdList;
    List<int> talkIdList;

    public int currentQuestId;
    public int currentTalkId;

    public override void Init()
    {
        base.Init();
    }

    public override void Interact()
    {
        if (Managers.Game.IsInteract)
        {
            Managers.Game.StopPlayer();
        }
    }

    // 대화 시작
    void Talk()
    {

    }
}

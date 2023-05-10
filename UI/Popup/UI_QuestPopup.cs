using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuestPopup : UI_Popup
{
    /*
    1. 퀘스트 목록을 관리
    2. Quest를 받으면 Content는 Height를 53 더해주기 
    3. Quest를 Scene에서 보고 싶을 경우 최대 5개 제한 (나중에 상황 따라서 변경)
    4. 
    */

    enum Texts
    {
        QuestTitleText,
        QuestDescText,
        QuestTargetText,
        QuestRewardGoldText,
        QuestRewardExpText,
        QuestSceneCountText,
    }

    int maxQuestSceneCount = 5;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        


        return true;
    }

    public void SetInfo()
    {

    }

    void RefreshUI()
    {

    }
}

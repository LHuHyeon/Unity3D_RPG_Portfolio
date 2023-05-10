using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TitleScene : UI_Scene
{
    /*
    1. 게임 시작 버튼
    2. 이어 하기 버튼
    3. 게임 종료 버튼
    */
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

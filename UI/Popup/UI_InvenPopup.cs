using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InvenPopup : UI_Popup
{
    /*
    1. 타이틀 잡으면 ui 이동 가능 (해상도 밖으로는 못나감.)
    2. 인벤 슬롯 초기화
    3. 인벤토리 관련 여기서 모두 관리.
    4. 스크롤 뷰 마우스 휠 속도 올리기
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

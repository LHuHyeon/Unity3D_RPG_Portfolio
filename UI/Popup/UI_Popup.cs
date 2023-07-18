using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ Popup 스크립트 ]
1. 모든 Popup의 부모이다.
*/

public class UI_Popup : UI_Base
{
    public Define.Popup popupType = Define.Popup.Unknown;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UI.SetCanvas(gameObject, true);
        return true;
    }

    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
}

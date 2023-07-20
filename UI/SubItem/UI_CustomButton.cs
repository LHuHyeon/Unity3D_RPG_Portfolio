using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ Custom 버튼 스크립트 ]
1. CustomScene에서 사용되며 커스텀 부위를 변경할 때 사용된다.
*/

public class UI_CustomButton : UI_Base
{
    enum Buttons
    {
        NextButton,
        BackButton,
    }

    public Define.DefaultPart partType;

    CharacterCustom _custom;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.NextButton).onClick.AddListener(()=>{ _custom.NextPart(partType, true); });
        GetButton((int)Buttons.BackButton).onClick.AddListener(()=>{ _custom.NextPart(partType, false); });

        return true;
    }

    public void SetInfo(CharacterCustom custom)
    {
        _custom = custom;
    }
}

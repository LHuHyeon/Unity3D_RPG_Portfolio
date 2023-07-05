using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

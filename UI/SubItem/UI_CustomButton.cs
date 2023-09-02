using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   UI_CustomButton.cs
 * Desc :   UI_CustomScene.cs에서 생성되며 파츠 교체하는 버튼을 담당
 *
 & Functions
 &  [Public]
 &  : Init()    - 초기 설정
 &  : SetInfo() - 기능 설정
 *
 */

public class UI_CustomButton : UI_Base
{
    enum Buttons
    {
        NextButton,
        BackButton,
    }

    public Define.DefaultPart   partType;   // 기본 파츠 타입

    private CharacterCustom     _custom;    // 커스텀 캐릭터 Object

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        // ▶ 클릭 버튼
        GetButton((int)Buttons.NextButton).onClick.AddListener(()=>{ _custom.NextPart(partType, true); });

        // ◀ 클릭 버튼
        GetButton((int)Buttons.BackButton).onClick.AddListener(()=>{ _custom.NextPart(partType, false); });

        return true;
    }

    public void SetInfo(CharacterCustom custom) { _custom = custom; }
}
